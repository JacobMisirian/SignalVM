using System;
using System.IO;

namespace SignalAsm
{
    public class Instruction
    {
        public byte OpCode { get; set; }
        public UInt16 Operand1 { get; set; }
        public UInt16 Operand2 { get; set; }
        public UInt32 Immediate { get; set; }

        public byte[] RawData { get; set; }

        private bool isRaw;

        public Instruction(bool isRaw = false)
        {
            OpCode = 0;
            Operand1 = 0;
            Operand2 = 0;
            Immediate = 0;

            this.isRaw = isRaw;
        }

        public void Write(BinaryWriter writer)
        {
            if (isRaw)
                writer.Write(RawData);
            else
            {
                writer.Write(OpCode);
                writer.Write(Operand1);
                writer.Write(Operand2);
                writer.Write(Immediate);
            }
            writer.Flush();
        }
    }
}

