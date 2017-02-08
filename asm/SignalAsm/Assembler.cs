using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SignalAsm.Scanner;

namespace SignalAsm
{
    public class Assembler
    {
        public const int INSTRUCTION_SIZE = 0x09;

        private List<Token> tokens;
        private int tokenPosition;

        Dictionary<Instruction, string> labelRequests;

        public void Assemble(List<Token> tokens, string file = "a.out")
        {
            this.tokens = tokens;

            List<Instruction> instructions = new List<Instruction>();
            Dictionary<string, UInt32> labelLocations = new Dictionary<string, UInt32>();
            labelRequests = new Dictionary<Instruction, string>();

            tokenPosition = 0;
            UInt32 virtualPosition = 0;

            while (tokenPosition < tokens.Count)
            {
                // If we see a label, log it and skip for now.
                if (acceptToken(TokenType.Dot))
                {
                    string label = expectToken(TokenType.Identifier).Value;
                    labelLocations.Add(label, virtualPosition);
                    continue;
                }

                string strOp = expectToken(TokenType.Identifier).Value;

                if (strOp.ToUpper() == "STRING")
                {
                    Instruction rawInstruction = new Instruction(true);
                    rawInstruction.RawData = ASCIIEncoding.ASCII.GetBytes(expectToken(TokenType.String).Value + "\0");
                    virtualPosition += (uint)rawInstruction.RawData.Length;
                    instructions.Add(rawInstruction);
                    continue;
                }

                Instruction instruction = new Instruction();
                instruction.OpCode = (byte)OpCodeConverter.StringToOpCode(strOp);

                switch ((OpCode)instruction.OpCode)
                {
                    case OpCode.Add:
                    case OpCode.Sub:
                    case OpCode.Mul:
                    case OpCode.Div:
                    case OpCode.Mod:
                    case OpCode.And:
                    case OpCode.Or:
                    case OpCode.Xor:
                    case OpCode.Mov:
                    case OpCode.Cmp:
                    case OpCode.Loadb:
                    case OpCode.Loadw:
                    case OpCode.Stob:
                    case OpCode.Stow:
                    case OpCode.Gt:
                    case OpCode.Gte:
                    case OpCode.Lt:
                    case OpCode.Lte:
                    case OpCode.Eq:
                    case OpCode.Neq:
                        instruction.Operand1 = parseOperand();
                        acceptToken(TokenType.Comma);
                        instruction.Operand2 = parseOperand();
                        break;
                    case OpCode.Not:
                    case OpCode.Push:
                    case OpCode.Pop:
                    case OpCode.Inc:
                    case OpCode.Dec:
                    case OpCode.Ret:
                        instruction.Operand1 = parseOperand();
                        break;
                    case OpCode.Li:
                    case OpCode.Addi:
                    case OpCode.Subi:
                    case OpCode.Muli:
                    case OpCode.Divi:
                    case OpCode.Modi:
                    case OpCode.Shiri:
                    case OpCode.Shili:
                    case OpCode.Andi:
                    case OpCode.Ori:
                    case OpCode.Xori:
                    case OpCode.Cmpi:
                    case OpCode.Loadbi:
                    case OpCode.Loadwi:
                    case OpCode.Gti:
                    case OpCode.Gtei:
                    case OpCode.Lti:
                    case OpCode.Ltei:
                    case OpCode.Eqi:
                    case OpCode.Neqi:
                        instruction.Operand1 = parseOperand();
                        acceptToken(TokenType.Comma);
                        instruction.Immediate = parseImmediate(instruction);
                        break;
                    case OpCode.Stobi:
                    case OpCode.Stowi:
                        instruction.Immediate = parseImmediate(instruction);
                        acceptToken(TokenType.Comma);
                        instruction.Operand1 = parseOperand();
                        break;
                    case OpCode.Jmp:
                    case OpCode.Jie:
                    case OpCode.Jig:
                    case OpCode.Jige:
                    case OpCode.Jil:
                    case OpCode.Jile:
                    case OpCode.Jine:
                    case OpCode.Pushi:
                    case OpCode.Reti:
                    case OpCode.Call:
                        instruction.Immediate = parseImmediate(instruction);
                        break;
                    case OpCode.Ston:
                        instruction.Operand1 = parseOperand();
                        acceptToken(TokenType.Comma);
                        instruction.Operand2 = parseOperand();
                        acceptToken(TokenType.Comma);
                        instruction.Immediate = parseImmediate(instruction);
                        break;
                }
                virtualPosition += INSTRUCTION_SIZE;
                instructions.Add(instruction);
            }

            foreach (var pair in labelRequests)
                pair.Key.Immediate = labelLocations[pair.Value];
            
            BinaryWriter writer = new BinaryWriter(new StreamWriter(file).BaseStream);
            foreach (var instruction in instructions)
                instruction.Write(writer);
            writer.Close();
        }

        private UInt32 parseImmediate(Instruction instruction)
        {
            if (acceptToken(TokenType.Dot))
            {
                if (matchToken(TokenType.Identifier))
                    labelRequests.Add(instruction, expectToken(TokenType.Identifier).Value);
                else if (matchToken(TokenType.Number))
                    labelRequests.Add(instruction, expectToken(TokenType.Number).Value);
                return 0;
            }
            return Convert.ToUInt32(expectToken(TokenType.Number).Value);
        }

        private UInt16 parseOperand()
        {
            char register = expectToken(TokenType.Identifier).Value.ToUpper()[0];
            return Convert.ToUInt16(register - 65);
        }

        private bool matchToken(TokenType tokenType)
        {
            return tokens[tokenPosition].TokenType == tokenType;
        }

        private bool acceptToken(TokenType tokenType)
        {
            if (tokens[tokenPosition].TokenType == tokenType)
            {
                tokenPosition++;
                return true;
            }
            return false;
        }

        private Token expectToken(TokenType tokenType)
        {
            if (tokens[tokenPosition].TokenType == tokenType)
                return tokens[tokenPosition++];
            throw new ExpectedTokenException(tokens[tokenPosition], tokenType);
        }
        private Token expectToken(TokenType tokenType, string val)
        {
            if (tokens[tokenPosition].TokenType == tokenType && tokens[tokenPosition].Value == val)
                return tokens[tokenPosition++];
            throw new ExpectedTokenException(tokens[tokenPosition], tokenType, val);
        }
    }
}

