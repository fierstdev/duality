using System;
using System.Collections.Generic;

namespace CSX.Generator
{
    public enum TokenType
    {
        Unknown,
        Text,           // Standard C# code outside of components or text inside tags
        TagOpenStrict,  // <div (start of tag)
        TagClose,       // </div>
        TagSelfClose,   // />
        GreaterThan,    // > (end of open tag)
        AttributeName,  // class=
        AttributeValue, // "foo"
        ComponentStart, // component Name {
        BlockStart,     // { inside JSX
        BlockEnd,       // } inside JSX
        EndOfFile
    }

    public struct Token
    {
        public TokenType Type;
        public string Value;
        public int Position;

        public Token(TokenType type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }
    }

    public class Lexer
    {
        private readonly string _input;
        private int _pos;

        public Lexer(string input)
        {
            _input = input;
            _pos = 0;
        }

        private char Current => _pos < _input.Length ? _input[_pos] : '\0';
        private char Peek(int n = 1) => _pos + n < _input.Length ? _input[_pos + n] : '\0';

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (_pos < _input.Length)
            {
                // This is a simplified Lexer for the PoC. 
                // In reality, we need to handle the mode switching between "C# Mode" and "JSX Mode".
                // For now, we will regex-style scan for interesting patterns.
                
                // 1. Check for 'component' keyword
                if (IsMatch("component "))
                {
                    tokens.Add(ReadComponentDecl());
                    continue;
                }

                // 2. Check for Tag Open <div
                if (Current == '<' && char.IsLetter(Peek()))
                {
                     tokens.Add(ReadTagOpen());
                     continue;
                }

                // 2.5 Attributes (Rough heuristic: if we are in a tag context)
                // Simplified: Just match attribute-like patterns if we see them. 
                // Currently our main loop is generic.
                if (char.IsLetter(Current) && _input.IndexOf('=', _pos) != -1)
                {
                    // Check if likely an attribute (followed by =)
                    // Verify we are not inside braces or normal C#
                    // This is VERY simplified.
                }

                // 3. Check for Tag Close </div>
                if (Current == '<' && Peek() == '/')
                {
                    tokens.Add(ReadTagClose());
                    continue;
                }
                
                // 4. Check for />
                if (Current == '/' && Peek() == '>')
                {
                    tokens.Add(new Token(TokenType.TagSelfClose, "/>", _pos));
                    _pos += 2;
                    continue;
                }

                // 5. Check for >
                if (Current == '>')
                {
                    tokens.Add(new Token(TokenType.GreaterThan, ">", _pos));
                    _pos++;
                    continue;
                }

                // 6. Default: Read Text
                tokens.Add(ReadText());
            }
            return tokens;
        }

        private Token ReadText()
        {
            int start = _pos;
            // If we fall through to here, any '<' is not a tag start, so consume it as text
            if (Current == '<') _pos++; 
            
            while (_pos < _input.Length && Current != '<' && Current != '>')
            {
                _pos++;
            }
            
            if (_pos == start) return new Token(TokenType.EndOfFile, "", -1); // Safety check
            
            return new Token(TokenType.Text, _input.Substring(start, _pos - start), start);
        }
        
        private bool IsMatch(string str)
        {
            if (_pos + str.Length > _input.Length) return false;
            return _input.Substring(_pos, str.Length) == str;
        }

        private Token ReadComponentDecl()
        {
            // Simple consume until {
            int start = _pos;
            while (Current != '{' && Current != '\0') _pos++;
            if (Current == '{') _pos++; // Consume the brace so it's not read as Text
            return new Token(TokenType.ComponentStart, _input.Substring(start, _pos - start), start);
        }

        private Token ReadTagOpen()
        {
            int start = _pos;
            _pos++; // skip <
            while (char.IsLetterOrDigit(Current)) _pos++;
            return new Token(TokenType.TagOpenStrict, _input.Substring(start, _pos - start), start);
        }
        
        private Token ReadTagClose()
        {
             int start = _pos;
            _pos += 2; // skip </
            while (char.IsLetterOrDigit(Current)) _pos++;
            if (Current == '>') _pos++;
            return new Token(TokenType.TagClose, _input.Substring(start, _pos - start), start);
        }
    }
}
