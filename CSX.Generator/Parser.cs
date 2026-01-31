using System;
using System.Collections.Generic;

namespace CSX.Generator
{
    public abstract class Node { }

    public class ComponentNode : Node
    {
        public string? Name { get; set; }
        public string? BodyRaw { get; set; }
        public Node? RenderTree { get; set; }
    }

    public class ElementNode : Node
    {
        public string? TagName { get; set; }
        public List<AttributeNode> Attributes { get; set; } = new List<AttributeNode>();
        public List<Node> Children { get; set; } = new List<Node>();
    }

    public class AttributeNode : Node
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public class TextNode : Node
    {
        public string? Text { get; set; }
    }
    
    public class ExpressionNode : Node
    {
        public string? Code { get; set; }
    }

    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _pos = 0;
        }

        private Token Current => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EndOfFile, "", -1);
        private Token Peek => _pos + 1 < _tokens.Count ? _tokens[_pos + 1] : new Token(TokenType.EndOfFile, "", -1);

        public ComponentNode? ParseComponent()
        {
            // Expected: ComponentStart ("component Name ")
            if (Current.Type != TokenType.ComponentStart) return null;
            
            var decl = Current.Value; // "component Name " or "component Name {"
            var name = decl.Substring(9).Replace("{", "").Trim();
            _pos++;

            // Accumulate body text until we find the XML return
            string bodyRaw = "";
            while (Current.Type != TokenType.EndOfFile)
            {
                 if (Current.Type == TokenType.TagOpenStrict) break;
                 if (Current.Type == TokenType.Text)
                 {
                     bodyRaw += Current.Value;
                 }
                 _pos++;
            }
            
            // Hacky clean up of 'return'
            var lastReturnIndex = bodyRaw.LastIndexOf("return");
            if (lastReturnIndex != -1)
            {
                bodyRaw = bodyRaw.Substring(0, lastReturnIndex);
            }

            return new ComponentNode
            {
                Name = name,
                BodyRaw = bodyRaw,
                RenderTree = ParseElement()
            };
        }

        private ElementNode ParseElement()
        {
            var tagToken = Current; // <div
             _pos++; // consume <div
            
            var node = new ElementNode { TagName = tagToken.Value.Substring(1) }; // div

            // Attributes
             while (Current.Type != TokenType.GreaterThan && Current.Type != TokenType.TagSelfClose)
             {
                 if (Current.Type == TokenType.Text && !string.IsNullOrWhiteSpace(Current.Value))
                 {
                     // Simplified attribute parser
                     // Expect: name="value" or name={val}
                     // Since our lexer is weak, we'll parse the raw string roughly for this POC
                     var raw = Current.Value.Trim();
                     if (string.IsNullOrEmpty(raw)) { _pos++; continue; }

                     var parts = raw.Split(new[] { '=' }, 2);
                     if (parts.Length == 2)
                     {
                         var name = parts[0].Trim();
                         var val = parts[1].Trim();
                         
                         // Strip quotes if present
                         if (val.StartsWith("\"") && val.EndsWith("\""))
                            val = val.Substring(1, val.Length - 2);

                         node.Attributes.Add(new AttributeNode { Name = name, Value = val });
                     }
                 }
                 _pos++;
             }

             if (Current.Type == TokenType.TagSelfClose)
             {
                 _pos++;
                 return node;
             }
             
             _pos++; // Consume >

             // Children
             while (Current.Type != TokenType.TagClose)
             {
                 if (Current.Type == TokenType.TagOpenStrict)
                 {
                     node.Children.Add(ParseElement());
                 }
                 else if (Current.Type == TokenType.Text)
                 {
                      node.Children.Add(new TextNode { Text = Current.Value });
                      _pos++;
                 }
                 else
                 {
                     _pos++;
                 }
             }
             
             _pos++; // Consume </div>
             return node;
        }
    }
}
