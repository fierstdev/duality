using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Duality.Compiler
{
    public record ParamInfo(string Name, string Type);
    public record MethodInfo(string Name, string ReturnType, List<ParamInfo> Parameters);

    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.AdditionalTextsProvider
                .Collect()
                .Combine(context.AnalyzerConfigOptionsProvider);
            
            context.RegisterSourceOutput(provider, (spc, source) =>
            {
                var files = source.Left;
                var options = source.Right;

                // Read property "build_property.Duality_Target"
                var target = EmitMode.Client;
                if (options.GlobalOptions.TryGetValue("build_property.Duality_Target", out var targetVal) && targetVal?.Equals("Server", System.StringComparison.OrdinalIgnoreCase) == true)
                {
                    target = EmitMode.Server;
                }

                var sbCssBundle = new StringBuilder();

                foreach (var file in files)
                {
                    if (!file.Path.EndsWith(".csx")) continue;

                    var sourceCodeMaybe = file.GetText(spc.CancellationToken)?.ToString();
                    if (sourceCodeMaybe is not { } sourceCode || string.IsNullOrWhiteSpace(sourceCode)) continue;

                    // 1. Lex & Parse
                    var lexer = new Lexer(sourceCode);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens);
                    var component = parser.ParseComponent();
                    if (component == null) continue; 
                    
                    var componentName = component.Name ?? "Unknown";

                    // 2. CSS Handling
                    var cssPath = file.Path.Substring(0, file.Path.Length - 4) + ".css";
                    var cssFile = files.FirstOrDefault(f => f.Path == cssPath);
                    string? cssMembersCode = null;

                    if (cssFile != null)
                    {
                        var cssContent = cssFile.GetText(spc.CancellationToken)?.ToString();
                        if (!string.IsNullOrEmpty(cssContent))
                        {
                             // SCSS/CSS Regex: dot followed by letter/underscore, then alphanumeric
                             var classMatches = System.Text.RegularExpressions.Regex.Matches(cssContent, @"\.([a-zA-Z_][a-zA-Z0-9_-]*)");
                             var uniqueHash = System.Guid.NewGuid().ToString("N").Substring(0, 6);
                             
                             var sbCssClass = new StringBuilder();
                             sbCssClass.AppendLine("    public static class Css");
                             sbCssClass.AppendLine("    {");
                             
                             var processedCss = cssContent;
                             var addedProps = new System.Collections.Generic.HashSet<string>();
                             
                             foreach (System.Text.RegularExpressions.Match match in classMatches)
                             {
                                 var className = match.Groups[1].Value;
                                 var scopedName = $"{className}_{componentName}_{uniqueHash}";
                                 
                                 // Property: public static string ClassName => "scopedName";
                                 var propName = ToPascalCase(className);
                                 
                                 if (!addedProps.Contains(propName))
                                 {
                                     sbCssClass.AppendLine($"        public static string {propName} => \"{scopedName}\";");
                                     addedProps.Add(propName);
                                 }
                                 
                                 // Replace in content
                                 processedCss = System.Text.RegularExpressions.Regex.Replace(processedCss, @"\." + className + @"\b", "." + scopedName);
                             }
                             sbCssClass.AppendLine("    }");
                             cssMembersCode = sbCssClass.ToString();
                             
                             sbCssBundle.AppendLine($"/* {componentName} */");
                             sbCssBundle.AppendLine(processedCss);
                        }
                    }

                    // 3. Analyze Body
                    string membersCode = "";
                    string renderCode = "";
                    List<MethodInfo> serverActions = new List<MethodInfo>();

                    if (!string.IsNullOrEmpty(component.BodyRaw))
                    {
                         var sanitizedBody = System.Text.RegularExpressions.Regex.Replace(component.BodyRaw, @"return\s*<", "// return <");
                         
                         var tree = CSharpSyntaxTree.ParseText(sanitizedBody, options: CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
                         var root = tree.GetRoot();
                        
                         var sbMembers = new StringBuilder();
                         if (cssMembersCode != null) sbMembers.AppendLine(cssMembersCode);

                         var sbRender = new StringBuilder();
                         
                         foreach (var node in root.ChildNodes())
                         {
                             if (node is MethodDeclarationSyntax method)
                             {
                                 if (method.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Server")))
                                 {
                                     var pList = method.ParameterList.Parameters
                                         .Select(p => new ParamInfo(p.Identifier.Text, p.Type?.ToString() ?? "object"))
                                         .ToList();
                                     serverActions.Add(new MethodInfo(method.Identifier.Text, method.ReturnType.ToString(), pList));
                                     if (target == EmitMode.Server) sbMembers.AppendLine(method.ToFullString());
                                 }
                                 else sbRender.AppendLine(node.ToFullString()); // Treat as local function in Render
                             }
                             else if (node is FieldDeclarationSyntax field)
                             {
                                 if (field.Modifiers.Any(m => m.Text == "static" || m.Text == "public" || m.Text == "private")) sbMembers.AppendLine(node.ToFullString());
                                 else sbRender.AppendLine(node.ToFullString());
                             }
                             else if (node is PropertyDeclarationSyntax || node is ClassDeclarationSyntax || node is StructDeclarationSyntax || node is EnumDeclarationSyntax || node is InterfaceDeclarationSyntax || node is DelegateDeclarationSyntax)
                             {
                                 sbMembers.AppendLine(node.ToFullString());
                             }
                             else sbRender.AppendLine(node.ToFullString());
                         }

                         membersCode = sbMembers.ToString();
                         renderCode = sbRender.ToString();
                    }
                    else if (cssMembersCode != null)
                    {
                        membersCode = cssMembersCode;
                    }

                    // 4. Emit
                    var emitter = new Emitter(target);
                    var csharp = emitter.Emit(component, serverActions, membersCode, renderCode);
                    spc.AddSource($"{component.Name}.g.cs", SourceText.From(csharp, Encoding.UTF8));
                }
                
                // 5. App Bundle & Router (Server Only)
                if (target == EmitMode.Server)
                {
                    spc.AddSource("CssBundle.g.cs", SourceText.From($@"
public static class CssBundle 
{{ 
    public static string AllCss = @""{sbCssBundle.ToString().Replace("\"", "\"\"")}""; 
}}", Encoding.UTF8));
                
                    GenerateAppRouter(spc, files);
                }
            });
        }

        private static string ToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var parts = s.Split(new[] {'-', '_'}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var p in parts)
            {
                 if (p.Length > 0) sb.Append(char.ToUpper(p[0]) + p.Substring(1));
            }
            return sb.ToString();
        }

        private void GenerateAppRouter(SourceProductionContext spc, System.Collections.Immutable.ImmutableArray<AdditionalText> files)
        {
                 var sb = new StringBuilder();
                 sb.AppendLine("using Microsoft.AspNetCore.Builder;");
                 sb.AppendLine("using Microsoft.AspNetCore.Http;");
                 sb.AppendLine("using System.Text;");
                 sb.AppendLine();
                 sb.AppendLine("public static class AppRouter");
                 sb.AppendLine("{");
                 sb.AppendLine("    public static void Map(WebApplication app)");
                 sb.AppendLine("    {");
                 sb.AppendLine("        app.MapGet(\"/app.css\", () => Results.Text(CssBundle.AllCss, \"text/css\"));");

                 // Pass 1: Pages (Routes)
                 foreach (var file in files)
                 {
                    var path = file.Path.Replace("\\", "/");
                    if (!path.EndsWith(".csx")) continue;

                     var pagesIndex = path.LastIndexOf("/Pages/", StringComparison.OrdinalIgnoreCase);
                     if (pagesIndex == -1) continue;
 
                     var relativePath = path.Substring(pagesIndex + 7);
                     var name = Path.GetFileNameWithoutExtension(relativePath);
                     var route = "/" + relativePath.Replace(".csx", "").ToLower();
                     if (name.Equals("Index", StringComparison.OrdinalIgnoreCase)) 
                     {
                          if (route.EndsWith("/index")) route = route.Substring(0, route.Length - 6);
                          if (string.IsNullOrEmpty(route)) route = "/";
                     }
 
                     var hasParams = route.Contains("[") && route.Contains("]");
                     if (hasParams) route = route.Replace("[", "{").Replace("]", "}");
                     
                     var sourceCode = file.GetText()?.ToString();
                     if (string.IsNullOrEmpty(sourceCode)) continue;
                     
                     var lexer = new Lexer(sourceCode!);
                     var tokens = lexer.Tokenize();
                     var parser = new Parser(tokens);
                     var comp = parser.ParseComponent();
                     if (comp == null) continue;
                     
                     var componentName = comp.Name ?? "";
                     if (componentName.Equals("Layout", StringComparison.OrdinalIgnoreCase) || name.Equals("_layout", StringComparison.OrdinalIgnoreCase)) continue;
 
                     sb.AppendLine($"        app.MapGet(\"{route}\", (HttpContext ctx) =>");
                     sb.AppendLine("        {");
                     sb.AppendLine("            var sb = new StringBuilder();");
                     
                     var hasLayout = files.Any(f => f.Path.EndsWith("/_layout.csx", StringComparison.OrdinalIgnoreCase) || f.Path.EndsWith("/Layout.csx", StringComparison.OrdinalIgnoreCase));
 
                     if (hasLayout)
                     {
                          if (hasParams)
                          {
                               sb.AppendLine("            var routeParams = new System.Collections.Generic.Dictionary<string, object>();");
                               sb.AppendLine("            foreach(var kvp in ctx.Request.RouteValues) { routeParams[kvp.Key] = kvp.Value; }");
                               sb.AppendLine($"            Layout_Impl.Render(sb, routeParams, (s) => {componentName}_Impl.Render(s, routeParams));");
                          }
                          else
                          {
                               sb.AppendLine($"            Layout_Impl.Render(sb, null, (s) => {componentName}_Impl.Render(s));");
                          }
                     }
                     else
                     {
                         sb.AppendLine("            sb.Append(\"<!DOCTYPE html><html><head><link rel='stylesheet' href='/app.css' /></head><body>\");");
                         if (hasParams)
                         {
                              sb.AppendLine("            var routeParams = new System.Collections.Generic.Dictionary<string, object>();");
                              sb.AppendLine("            foreach(var kvp in ctx.Request.RouteValues) { routeParams[kvp.Key] = kvp.Value; }");
                              sb.AppendLine($"            {componentName}_Impl.Render(sb, routeParams);");
                         }
                         else
                         {
                              sb.AppendLine($"            {componentName}_Impl.Render(sb);");
                         }
                         sb.AppendLine("            sb.Append(\"</body></html>\");");
                     }
                     
                     sb.AppendLine("            return Results.Content(sb.ToString(), \"text/html\");");
                     sb.AppendLine("        });");
                 }

                 // Pass 2: RPC (All Components)
                 foreach (var file in files)
                 {
                    var path = file.Path.Replace("\\", "/");
                    if (!path.EndsWith(".csx")) continue;

                     var sourceCode = file.GetText()?.ToString();
                     if (string.IsNullOrEmpty(sourceCode)) continue;
                     
                     var lexer = new Lexer(sourceCode!);
                     var tokens = lexer.Tokenize();
                     var parser = new Parser(tokens);
                     var comp = parser.ParseComponent();
                     if (comp == null) continue;

                     var componentName = comp.Name ?? "";

                     if (!string.IsNullOrEmpty(comp.BodyRaw))
                     {
                         var sanitizedBody = System.Text.RegularExpressions.Regex.Replace(comp.BodyRaw, @"return\s*<", "// return <");
                         var tree = CSharpSyntaxTree.ParseText(sanitizedBody, options: CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
                         var root = tree.GetRoot();
                         
                         // Detect Namespace from Original Parse (comp.Namespace is not available? Check Parser)
                         // Actually comp.Namespace is not exposed in the simplified Parser logic here? 
                         // Check file content for namespace. 
                         // Better: Parser.cs handles namespace, let's trust it BUT "comp" structure here is just {Name, BodyRaw}. 
                         // Let's re-parse using our Lexer/Parser to get Namespace?
                         // "comp" was returned by ParseComponent. We can check comp.Namespace if I add it or if it exists.
                         // Checking Parser.cs... The Parser logic might not fully populate namespace for me here or I ignored it.
                         // Wait! The "comp" object ALREADY exists from line 278. 
                         // "var comp = parser.ParseComponent();"
                         // Does 'comp' have Namespace property?
                         // Let's check Parser.cs or just look at Generator.cs logic.
                         // Generator.cs imports CSX.Generator.Parser.
                         // If I look at the loop above, I see "comp" is used.
                         
                         // I will define namespace here by Regex/Parsing quick check or reuse Parser.
                         
                         var namespaceName = "";
                         var fileNamespaceMatch = System.Text.RegularExpressions.Regex.Match(sourceCode ?? "", @"^\s*namespace\s+([\w\.]+)\s*[;\{]", System.Text.RegularExpressions.RegexOptions.Multiline);
                         if (fileNamespaceMatch.Success)
                         {
                             namespaceName = fileNamespaceMatch.Groups[1].Value + ".";
                         }

                         var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                         var methods = allMethods
                                 .Where(m => m.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Server" || a.Name.ToString() == "ServerAttribute")))
                                 .ToList();
 
                         foreach (var m in methods)
                         {
                             var methodName = m.Identifier.Text;
                             var rpcRoute = $"/_rpc/{componentName}/{methodName}";
                             var parameters = m.ParameterList.Parameters;
                             var asyncKeyword = parameters.Count > 0 ? "async" : "";
                             
                             sb.AppendLine($"        app.MapPost(\"{rpcRoute}\", {asyncKeyword} (HttpRequest req) =>");
                             sb.AppendLine("        {");
                             if (parameters.Count > 0)
                             {
                                 sb.AppendLine("            var args = await req.ReadFromJsonAsync<System.Text.Json.JsonElement[]>();");
                                 sb.AppendLine("            if (args == null || args.Length != " + parameters.Count + ") return Results.BadRequest(\"Invalid arguments\");");
                                 var argCallList = new List<string>();
                                 for(int i=0; i<parameters.Count; i++)
                                 {
                                     var p = parameters[i];
                                     var pType = p.Type?.ToString() ?? "object";
                                     sb.AppendLine($"            var arg{i} = System.Text.Json.JsonSerializer.Deserialize<{pType}>(args[{i}]);");
                                     argCallList.Add($"arg{i}");
                                 }
                                 sb.AppendLine($"            {namespaceName}{componentName}_Impl.{methodName}({string.Join(", ", argCallList)});");
                             }
                             else
                             {
                                     sb.AppendLine($"            {namespaceName}{componentName}_Impl.{methodName}();");
                             }
                             sb.AppendLine("            var sb = new StringBuilder();");
                             sb.AppendLine($"            {namespaceName}{componentName}_Impl.Render(sb);");
                             sb.AppendLine("            return Results.Content(sb.ToString(), \"text/html\");");
                             sb.AppendLine("        });");
                         }
                     }
                 }
                 sb.AppendLine("    }");
                 sb.AppendLine("}");
                 spc.AddSource("AppRouter.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
