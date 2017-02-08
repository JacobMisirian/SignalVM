using System;

using SignalAsm.Scanner;

namespace SignalAsm
{
    public class ExpectedTokenException : Exception
    {
        public new string Message { get; private set; }

        public ExpectedTokenException(Token got, TokenType tokenType, string val = "")
        {
            Message = string.Format("Expected token with type {0} and value {1}. Got: {2}", tokenType, val, got);
        }
    }
}

