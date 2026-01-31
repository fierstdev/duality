using System.Text;

namespace CSX.Generator
{
    public class Emitter
    {
        private List<(string Id, string Body, string ClosureName, List<string> Captures)> _hoistedHandlers = new List<(string, string, string, List<string>)>();

        public string Emit(ComponentNode component)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using CSX.Runtime;");
            sb.AppendLine("using static CSX.Runtime.Hooks;");
            sb.AppendLine();
            sb.AppendLine($"// Generated CSX Component: {component.Name}");
            sb.AppendLine($"public static partial class {component.Name}_Impl");
            sb.AppendLine("{");
            sb.AppendLine($"    public static void Render(RenderContext ctx)");
            sb.AppendLine("    {");
            
            // Inject User Code
            if (!string.IsNullOrEmpty(component.BodyRaw))
            {
                sb.AppendLine("        // User Code");
                sb.AppendLine(component.BodyRaw);
            }

            // Tree Walk
            EmitNode(sb, component.RenderTree, null, component);

            sb.AppendLine("    }");

            // Emit Hoisted Handlers & Closure Classes
            foreach (var handler in _hoistedHandlers)
            {
                // Closure Class
                if (handler.Captures.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"    public class {handler.ClosureName}");
                    sb.AppendLine("    {");
                    foreach (var cap in handler.Captures)
                    {
                        // We assume 'dynamic' or 'object' for now since we don't have type info without semantic model
                        // Or we can try to infer.. no, let's use 'dynamic' for max compatibility in this POC?
                        // Actually, 'var' works in method, 'dynamic' field works.
                        // Or generic Signal, but we don't know the T.
                        // Let's use 'dynamic' for the fields to be safe.
                        sb.AppendLine($"        public dynamic {cap};");
                    }
                    sb.AppendLine("    }");
                }

                sb.AppendLine();
                sb.AppendLine($"    public static void {handler.Id}(object state)");
                sb.AppendLine("    {");
                if (handler.Captures.Count > 0)
                {
                    sb.AppendLine($"        var env = ({handler.ClosureName})state;");
                }
                sb.AppendLine($"        {handler.Body}");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string? EmitNode(StringBuilder sb, Node? node, string? parentVar, ComponentNode component)
        {
            if (node is ElementNode el)
            {
                var elVar = "el_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
                sb.AppendLine($"        var {elVar} = NativeDom.CreateElement(\"{el.TagName}\");");
                
                // Attributes
                foreach (var attr in el.Attributes)
                {
                    if (attr.Name != null && attr.Name.StartsWith("on"))
                    {
                        // Hoisting Logic!
                        // 1. Generate ID
                        var handlerId = $"{component.Name}_Handler_{System.Guid.NewGuid().ToString("N").Substring(0, 6)}";
                        var closureClassName = $"{component.Name}_Closure_{handlerId}";
                        
                        // 2. Extract & Analyze
                        var code = attr.Value;
                        if (code != null && code.StartsWith("{") && code.EndsWith("}"))
                            code = code.Substring(1, code.Length - 2);
                        
                        // Automatic Closure Extraction
                        var analysis = ClosureAnalyzer.AnalyzeAndRewrite(code ?? "", component.BodyRaw ?? "");
                        
                        // 3. Emit Closure Class (Store metadata to emit later)
                        _hoistedHandlers.Add((handlerId, analysis.RewrittenCode, closureClassName, analysis.CapturedVariables));

                        // 4. Instantiate Closure State in Render
                        var closureVar = "env_" + handlerId;
                        sb.AppendLine($"        var {closureVar} = new {closureClassName}();");
                        foreach(var capture in analysis.CapturedVariables)
                        {
                            sb.AppendLine($"        {closureVar}.{capture} = {capture};");
                        }

                        // 5. Emit HTML attribute
                        sb.AppendLine($"        NativeDom.SetAttribute({elVar}, \"{attr.Name}-csx\", \"{handlerId}\");");
                        
                        // 6. Register
                        // Pass the closure instance as state
                        sb.AppendLine($"        HandlerRegistry.Register(\"{handlerId}\", (s) => {handlerId}(s));");
                        // Note: The playground version just calls the static method, passing 's' (which is the closure object)
                    }
                    else
                    {
                        if (attr.Value != null && attr.Value.StartsWith("{"))
                        {
                             // Binder for attributes
                             var code = attr.Value.Substring(1, attr.Value.Length - 2);
                             sb.AppendLine($"        Reactivity.Bind({code}, val => NativeDom.SetAttribute({elVar}, \"{attr.Name}\", val));");
                        }
                        else
                        {
                             sb.AppendLine($"        NativeDom.SetAttribute({elVar}, \"{attr.Name}\", \"{attr.Value}\");");
                        }
                    }
                }

                // Append to parent if valid
                if (!string.IsNullOrEmpty(parentVar))
                {
                    sb.AppendLine($"        NativeDom.AppendChild({parentVar}, {elVar});");
                }

                // Children
                foreach (var child in el.Children)
                {
                    if (child is TextNode text)
                    {
                         if (string.IsNullOrWhiteSpace(text.Text)) continue;

                         // Split by {expression}
                         // Simplified regex: matches { ... } non-greedy
                         var parts = System.Text.RegularExpressions.Regex.Split(text.Text, @"(\{.*?\})");
                         foreach (var part in parts)
                         {
                             if (string.IsNullOrEmpty(part)) continue;
                             
                             if (part.StartsWith("{") && part.EndsWith("}"))
                             {
                                  var code = part.Substring(1, part.Length - 2);
                                  sb.AppendLine($"        Reactivity.Bind({code}, val => NativeDom.SetText({elVar}, val));");
                             }
                             else
                             {
                                  // Verbatim string with quote escaping
                                  var safeText = part.Replace("\"", "\"\"");
                                  // Don't emit empty whitespace if we can avoid it (or do, HTML whitespace rules)
                                  if (!string.IsNullOrWhiteSpace(safeText))
                                      sb.AppendLine($"        NativeDom.SetText({elVar}, @\"{safeText}\");");
                             }
                         }
                    }
                    else
                    {
                        EmitNode(sb, child, elVar, component);
                    }
                }
                
                return elVar;
            }
            return null;
        }
    }
}
