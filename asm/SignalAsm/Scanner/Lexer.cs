using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalAsm.Scanner
{
    public class Lexer
    {
        private int pos;
        private string code;
        private List<Token> tokens;
        private Dictionary<string, string> defines;

        public List<Token> Scan(string source)
        {
            pos = 0;
            code = source;
            tokens = new List<Token>();
            defines = new Dictionary<string, string>();

            while (peekChar() != -1)
            {
                whiteSpace();
                if (char.IsLetterOrDigit((char)peekChar()))
                    scanIdentifier();
                else
                {
                    switch ((char)peekChar())
                    {
                        case '#':
                            scanSingleLineComment();
                            break;
                        case '.':
                            tokens.Add(new Token(TokenType.Dot, ((char)readChar()).ToString()));
                            break;
                        case ',':
                            tokens.Add(new Token(TokenType.Comma, ((char)readChar()).ToString()));
                            break;
                        case '"':
                            scanString();
                            break;
                        default:
                            Console.WriteLine("Error unknown char: {0}", readChar());
                            break;
                    }
                }
                whiteSpace();
            }
            return tokens;
        }

        private void whiteSpace()
        {
            if (char.IsWhiteSpace((char)peekChar()) && peekChar() != -1)
                readChar();
        }

        private void scanSingleLineComment()
        {
            readChar(); // #
            StringBuilder sb = new StringBuilder();
            while (peekChar() != -1 && (char)peekChar() != '\n')
                sb.Append((char)readChar());
            readChar(); // \n
            string[] parts = sb.ToString().Split(' ');
            if (parts[0].ToLower().StartsWith("define"))
                defines.Add(parts[1], string.Join(" ", parts, 2, parts.Length - 2));
        }

        private void scanIdentifier()
        {
            StringBuilder id = new StringBuilder();
            while (char.IsLetterOrDigit((char)peekChar()) && peekChar() != -1)
                id.Append((char)readChar());
            if (defines.ContainsKey(id.ToString()))
                id = new StringBuilder(defines[id.ToString()]);
            try
            {
                tokens.Add(new Token(TokenType.Number, Convert.ToInt32(id.ToString()).ToString()));
            }
            catch
            {
                tokens.Add(new Token(TokenType.Identifier, id.ToString()));
            }
        }

        private void scanString()
        {
            StringBuilder str = new StringBuilder();
            readChar(); // "
            while ((char)peekChar() != '"' && peekChar() != -1)
                str.Append((char)readChar());
            readChar(); // "
            tokens.Add(new Token(TokenType.String, str.ToString()));
        }

        private int peekChar(int n = 0)
        {
            return pos + n < code.Length ? code[pos + n] : -1;
        }
        private int readChar()
        {
            return pos < code.Length ? code[pos++] : -1;
        }
    }
}

