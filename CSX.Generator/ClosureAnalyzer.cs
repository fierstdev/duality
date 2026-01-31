using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text; // Added for TextSpan

namespace CSX.Generator
{
    public class ClosureAnalyzer
    {
        // Finds variables declared in the component body available for capture
        public static List<string> FindLocalVariables(string bodyRaw)
        {
            var tree = CSharpSyntaxTree.ParseText("void Method() { " + bodyRaw + " }");
            var root = tree.GetRoot();
            
            var vars = new List<string>();
            
            var variableDeclarations = root.DescendantNodes().OfType<VariableDeclaratorSyntax>();
            foreach (var decl in variableDeclarations)
            {
                vars.Add(decl.Identifier.Text);
            }
            
            // Note: IsDeconstruction is an extension method in later Roslyn or not available in standard 2.0 context easily.
            // We will simplify: check if Left is TupleExpressionSyntax or DeclarationExpressionSyntax
            // assignment.Left is TupleExpressionSyntax
             var assignments = root.DescendantNodes().OfType<AssignmentExpressionSyntax>();
             foreach(var assign in assignments)
             {
                 if (assign.Left is TupleExpressionSyntax tuple)
                 {
                     // extract variables from (count, setCount)
                     foreach(var arg in tuple.Arguments)
                     {
                         if (arg.Expression is IdentifierNameSyntax id)
                            vars.Add(id.Identifier.Text);
                         else if (arg.Expression is DeclarationExpressionSyntax decl)
                         {
                             // (var x, var y)
                             if (decl.Designation is SingleVariableDesignationSyntax s)
                                 vars.Add(s.Identifier.Text);
                         }
                     }
                 }
             }

            // Also check LocalDeclarationStatement with DeclarationExpression?
            // "var (x, y) = ..." parses as LocalDeclarationStatement -> VariableDeclaration -> VariableDeclarator
            // where Declarator.Identifier is usually empty or weird, but the assignment contains the tuple?
            // Actually: "var (a,b) = ..." -> VariableDeclaration with Type=var, Variables=[(a,b)]
            // The syntax tree for `var (a, b) = ...` is LocalDeclarationStatement
            //   Declaration: VariableDeclaration (Type: var)
            //     Variables: [VariableDeclarator]
            //       Identifier: (missing?) or the tuple is in the Identifier?
            // Wait, in recent C#, `(a, b)` is a specific designation.
            
            // Let's just grab ALL SingleVariableDesignationSyntax, that catches `var (x, y)` pattern usually.
            var designations = root.DescendantNodes().OfType<SingleVariableDesignationSyntax>();
            foreach(var d in designations) vars.Add(d.Identifier.Text);
            
            return vars.Distinct().ToList();
        }

        public class CaptureResult
        {
            public string RewrittenCode { get; set; } = "";
            public List<string> CapturedVariables { get; set; } = new List<string>();
        }

        public static CaptureResult AnalyzeAndRewrite(string handlerCode, string bodyRaw)
        {
             // 1. Identify potential captures (locals in body)
             var candidates = new HashSet<string>(FindLocalVariables(bodyRaw));

             // 2. Parse Handler
             var handlerTree = CSharpSyntaxTree.ParseText("void Handler() { " + handlerCode + " }");
             var handlerRoot = handlerTree.GetRoot();
             
             var captures = new List<string>();
             var rewrites = new Dictionary<TextSpan, string>();

             var identifiers = handlerRoot.DescendantNodes().OfType<IdentifierNameSyntax>();
             
             foreach (var id in identifiers)
             {
                 var name = id.Identifier.Text;
                 // Filter out: types, methods, keywords (parsed as identifiers sometimes)
                 // This requires semantic model to do perfectly.
                 // Heuristic: If it's in candidates, capture it.
                 // Note: we might capture "NativeDom" if defined in body? unlikely.
                 
                 if (candidates.Contains(name))
                 {
                     if (!captures.Contains(name)) captures.Add(name);
                 }
             }

             // Rewrite
             // We can't easily modify the tree and get string back preserving whitespace with just logic above without proper rewriter
             // But for small snippets, string replacement or CSharpSyntaxRewriter is fine.
             
             // Let's just do a naive replacement of words for this PoC? 
             // No, that breaks string literals "count".
             // Let's use information from the syntax tree to replace mostly correctly.
             
             string rewritten = handlerCode;
             // Process in reverse to maintain indices
             var nodesToReplace = identifiers
                .Where(id => captures.Contains(id.Identifier.Text))
                .OrderByDescending(n => n.SpanStart);

             // Offset because we wrapped in "void Handler() { " (length 17)
             int wrapperOffset = 17;
             
             foreach (var node in nodesToReplace)
             {
                 // Check if node is part of MemberAccess (e.g. s.count) -> don't replace 'count' if it's the member name
                 if (node.Parent is MemberAccessExpressionSyntax ma && ma.Name == node) continue;
                 
                 // If it is 'count' in 'count.Value', we DO replace it -> env.count.Value
                 
                 var start = node.SpanStart - wrapperOffset;
                 var length = node.Span.Length;
                 
                 if (start >= 0 && start + length <= rewritten.Length)
                 {
                     rewritten = rewritten.Remove(start, length).Insert(start, "env." + node.Identifier.Text);
                 }
             }

             return new CaptureResult
             {
                 RewrittenCode = rewritten,
                 CapturedVariables = captures
             };
        }
    }
}
