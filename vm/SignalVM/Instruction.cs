using System;
using System.Text;

namespace SignalVM 
{
    public class Instruction
    {
        public OpCode OpCode { get; private set; }
        public UInt16 Operand1 { get; private set; }
        public UInt16 Operand2 { get; private set; }
        public UInt32 Immediate { get; private set; }

        public Instruction(byte[] bytes, ref uint pos)
        {
            OpCode = (OpCode)bytes[pos++];
            Operand1 = BitConverter.ToUInt16(bytes, (int)pos);
            pos += 2;
            Operand2 = BitConverter.ToUInt16(bytes, (int)pos);
            pos += 2;
            Immediate = BitConverter.ToUInt32(bytes, (int)pos);
            pos += 4;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ", OpCode);
            sb.AppendFormat("{0} ", (char)(Operand1 + 65));
            sb.AppendFormat("{0} ", (char)(Operand2 + 65));
            sb.AppendFormat("{0}", Immediate);
            return sb.ToString();
        }
    }
}

