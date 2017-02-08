using System;

namespace SignalAsm.Scanner
{
    public class Token
    {
        public TokenType TokenType { get; private set; }
        public string Value { get; private set; }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("[Token: TokenType={0}, Value={1}]", TokenType, Value);
        }
    }
}

