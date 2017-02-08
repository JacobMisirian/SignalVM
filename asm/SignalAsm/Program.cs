using System;
using System.IO;

using SignalAsm.Scanner;

namespace SignalAsm
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                new Assembler().Assemble(new Lexer().Scan(File.ReadAllText(args[0])));
            }
            catch (ExpectedTokenException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
