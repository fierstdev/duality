using System.Text;

namespace CSX.Generator
{
    public enum EmitMode { Client, Server }

    public class Emitter
    {
        private List<(string Id, string Body, string ClosureName, List<string> Captures)> _hoistedHandlers = new List<(string, string, string, List<string>)>();
        private readonly EmitMode _mode;
        private int _handlerCounter = 0;

        public Emitter(EmitMode mode = EmitMode.Client)
        {
            _mode = mode;
        }

        public string Emit(ComponentNode component, List<MethodInfo>? serverActions = null, string? membersCode = null, string? renderCode = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using CSX.Runtime;");
            sb.AppendLine("using static CSX.Runtime.Hooks;");
            if (_mode == EmitMode.Server) sb.AppendLine("using System.Text;");
            
            if (!string.IsNullOrEmpty(component.HeaderCode))
            {
                sb.AppendLine();
                sb.AppendLine("// Header Code");
                sb.AppendLine(component.HeaderCode);
            }
            
            sb.AppendLine();
            sb.AppendLine($"// Generated CSX Component: {component.Name}");
            sb.AppendLine($"public static partial class {component.Name}_Impl");
            sb.AppendLine("{");

            // Inject Members (Methods, Properties)
            if (!string.IsNullOrEmpty(membersCode))
            {
                sb.AppendLine("    // Class Members");
                sb.AppendLine(membersCode);
            }
            
            // Emit Proxies (Client Mode Only)
            if (_mode == EmitMode.Client && serverActions != null)
            {
                foreach(var action in serverActions)
                {
                    sb.AppendLine($"    // Proxy for {action.Name}");
                    var paramDecl = string.Join(", ", action.Parameters.Select(p => $"{p.Type} {p.Name}"));
                    var argList = string.Join(", ", action.Parameters.Select(p => p.Name));
                    
                    // Determine return type (void -> async void, Task -> async Task)
                    var retType = action.ReturnType;
                    if (retType == "void") retType = "async void";
                    else if (retType == "Task") retType = "async Task";
                    // else... maintain original but unlikely to work without async wrapping
                    
                    sb.AppendLine($"    public static {retType} {action.Name}({paramDecl})");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        await CSX.Runtime.RpcClient.CallAsync(\"{component.Name}\", \"{action.Name}\", {argList});");
                    sb.AppendLine("    }");
                }
            }

            // Server Signature
            if (_mode == EmitMode.Server)
            {
                sb.AppendLine($"    public static void Render(StringBuilder sb, System.Collections.Generic.Dictionary<string, object>? routeParams = null, System.Action<StringBuilder>? childContent = null)");
            }
            else
            {
                sb.AppendLine($"    public static void Render(RenderContext ctx)");
            }
            sb.AppendLine("    {");
            
            // Unified access to RouteParams
            if (_mode == EmitMode.Server)
            {
                sb.AppendLine("        var RouteParams = routeParams ?? new System.Collections.Generic.Dictionary<string, object>();");
            }
            else
            {
                sb.AppendLine("        var RouteParams = ctx.RouteParams;");
            }
            
            // Inject Render Logic (Statements, Returns)
            if (!string.IsNullOrEmpty(renderCode))
            {
                sb.AppendLine("        // Render Logic");
                sb.AppendLine(renderCode);
            }
            // Fallback to BodyRaw if split failed? (Only if membersCode/renderCode null)
            else if (membersCode == null && renderCode == null && !string.IsNullOrEmpty(component.BodyRaw))
            {
                 // Legacy behavior
                 sb.AppendLine(component.BodyRaw);
            }

            // Tree Walk
            if (_mode == EmitMode.Server)
            {
                // In Server Mode, we don't need 'ctx' usually, or we might need it for state?
                // For this POC, we assume static rendering (no state hooks on server yet, or they return initial).
                EmitServerNode(sb, component.RenderTree, component);
            }
            else
            {
                EmitNode(sb, component.RenderTree, null, component);
            }

            sb.AppendLine("    }");

            // Emit Hoisted Handlers & Closure Classes
            // Ideally, we only need these on the CLIENT. 
            // On the SERVER, we don't execute handlers.
            // BUT, for the server to compile the same file, it needs to be valid code.
            // The handlers refer to variables (closures).
            // We should ideally #if !SERVER them, but for now we emit them so it compiles.
            // (The code inside might reference client-only APIs? We'll see).
            
            foreach (var handler in _hoistedHandlers)
            {
                if (handler.Captures.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"    public class {handler.ClosureName}");
                    sb.AppendLine("    {");
                    foreach (var cap in handler.Captures) sb.AppendLine($"        public dynamic? {cap};");
                    sb.AppendLine("    }");
                }

                sb.AppendLine();
                sb.AppendLine($"    public static void {handler.Id}(object? state)");
                sb.AppendLine("    {");
                // On Server, handler body might fail if it uses Browser APIs. 
                // We should wrap in 'if (false)' or similar? 
                // Or just assume users check for IsClient.
                // For now, emit as is.
                if (handler.Captures.Count > 0) sb.AppendLine($"        var env = ({handler.ClosureName})state!;");
                sb.AppendLine($"        {handler.Body};");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private void EmitServerNode(StringBuilder sb, Node? node, ComponentNode component)
        {
            if (node is ElementNode el)
            {
                if (el.TagName != null && el.TagName.Equals("Slot", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine("        childContent?.Invoke(sb);");
                    return;
                }

                // Component Call (Uppercase)
                if (el.TagName != null && char.IsUpper(el.TagName[0]))
                {
                    sb.AppendLine($"        {el.TagName}_Impl.Render(sb, null, (sb) => {{");
                    foreach (var child in el.Children)
                    {
                        EmitServerNode(sb, child, component);
                    }
                    sb.AppendLine("        });");
                    return;
                }
                
                sb.AppendLine($"        sb.Append(\"<{el.TagName}\");");
                
                foreach (var attr in el.Attributes)
                {
                    if (attr.Name != null && attr.Name.StartsWith("on"))
                    {
                        // Generate deterministic ID matching Client
                        var handlerId = $"{component.Name}_Handler_{_handlerCounter++}";
                        
                        // We do NOT emit the handler body registration on server (no Reactivity/Registry needed).
                        // We DO need to emit the attribute.
                         sb.AppendLine($"        sb.Append(\" {attr.Name}-csx=\\\"{handlerId}\\\"\");");
                         
                         // We still process analysis so we don't crash, but we don't need result?
                         // Actually we don't need to do anything else.
                    }
                    else
                    {
                         if (attr.Value != null && attr.Value.StartsWith("{"))
                         {
                             var code = attr.Value.Substring(1, attr.Value.Length - 2);
                             sb.AppendLine($"        sb.Append(\" {attr.Name}=\\\"\");");
                             sb.AppendLine($"        sb.Append({code});");
                             sb.AppendLine($"        sb.Append(\"\\\"\");");
                         }
                         else
                         {
                             sb.AppendLine($"        sb.Append(\" {attr.Name}=\\\"{attr.Value}\\\"\");");
                         }
                    }
                }

                sb.AppendLine("        sb.Append(\">\");");

                foreach (var child in el.Children)
                {
                    if (child is TextNode text)
                    {
                         // Handle interpolation
                         var parts = System.Text.RegularExpressions.Regex.Split(text.Text, @"(\{.*?\})");
                         foreach (var part in parts)
                         {
                             if (string.IsNullOrEmpty(part)) continue;
                             if (part.StartsWith("{") && part.EndsWith("}"))
                             {
                                  var code = part.Substring(1, part.Length - 2);
                                  sb.AppendLine($"        sb.Append({code});");
                             }
                             else
                             {
                                  sb.AppendLine($"        sb.Append(@\"{part}\");");
                             }
                         }
                    }
                    else
                    {
                        EmitServerNode(sb, child, component);
                    }
                }
                
                sb.AppendLine($"        sb.Append(\"</{el.TagName}>\");");
            }
        }

        private string? EmitNode(StringBuilder sb, Node? node, string? parentVar, ComponentNode component)
        {
            // ... (Existing Client Logic) ...
            if (node is ElementNode el)
            {
                var elVar = "el_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
                sb.AppendLine($"        var {elVar} = NativeDom.CreateElement(\"{el.TagName}\");");
                
                // Attributes
                foreach (var attr in el.Attributes)
                {
                    if (attr.Name != null && attr.Name.StartsWith("on"))
                    {
                        // Hoisting Logic
                        var handlerId = $"{component.Name}_Handler_{_handlerCounter++}";
                        
                        // Client Mode Only for Handler Registration
                        if (_mode == EmitMode.Client)
                        {
                            var closureClassName = $"{component.Name}_Closure_{handlerId}";
                            
                            var code = attr.Value;
                            if (code != null && code.StartsWith("{") && code.EndsWith("}"))
                                code = code.Substring(1, code.Length - 2);
                            
                            // Static Analysis
                            var analysis = ClosureAnalyzer.AnalyzeAndRewrite(code ?? "", component.BodyRaw ?? "");
                            
                            // Add to list (Tuple)
                            _hoistedHandlers.Add((handlerId, analysis.RewrittenCode, closureClassName, analysis.CapturedVariables));

                            var closureVar = "env_" + handlerId;
                            // Fix Nullable: closureVar might be null.
                            sb.AppendLine($"        object? {closureVar} = null;");
                            
                            if (analysis.CapturedVariables.Count > 0)
                            {
                                sb.AppendLine($"        var closure_inst_{handlerId} = new {closureClassName}();");
                                foreach(var capture in analysis.CapturedVariables)
                                {
                                    sb.AppendLine($"        closure_inst_{handlerId}.{capture} = {capture};");
                                }
                                sb.AppendLine($"        {closureVar} = closure_inst_{handlerId};");
                            }

                            sb.AppendLine($"        NativeDom.SetAttribute({elVar}, \"{attr.Name}-csx\", \"{handlerId}\");");
                            sb.AppendLine($"        HandlerRegistry.Register(\"{handlerId}\", () => {handlerId}({closureVar}));");
                        }
                        else
                        {
                             sb.AppendLine($"        sb.Append(\" {attr.Name}-csx=\\\"{handlerId}\\\"\");");
                        }
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
