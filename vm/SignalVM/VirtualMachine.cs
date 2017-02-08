using System;
using System.Collections.Generic;

namespace SignalVM
{
    public class VirtualMachine
    {
        private const UInt16 IP = 0x0D;
        private const UInt16 SP = 0x0E;
        private const UInt16 FLAGS = 0x0F;

        private const UInt32 ZERO_FLAG = 0x01;
        private const UInt32 SIGN_FLAG = 0x02;
        private const UInt32 OVERFLOW_FLAG = 0x03;

        private const UInt32 SERIAL_POSITION = 0xFFFE;

        private byte[] ram;
        private int osSize;

        private UInt32[] registers;

        private SerialConsole serial = new SerialConsole();

        public VirtualMachine(byte[] os)
        {
            ram = new byte[0xFFFF];
            os.CopyTo(ram, 0);
            osSize = os.Length;

            registers = new UInt32[16];
            registers[IP] = 0;
            registers[SP] = 9000;
        }

        public void Execute()
        {
            while (registers[IP] < osSize)
            {
                var instruction = new Instruction(ram, ref registers[IP]);
               // Console.WriteLine(instruction + "\ta:{0}", registers[0]);
              //  Console.WriteLine(stack.Count > 0 ? stack.Peek() : 0);
                switch (instruction.OpCode)
                {
                    case OpCode.Add:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] + registers[instruction.Operand2]);
                        break;
                    case OpCode.Addi:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] + instruction.Immediate);
                        break;
                    case OpCode.Sub:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] - registers[instruction.Operand2]);
                        break;
                    case OpCode.Subi:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] - instruction.Immediate);
                        break;
                    case OpCode.Mul:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] * registers[instruction.Operand2]);
                        break;
                    case OpCode.Muli:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] * instruction.Immediate);
                        break;
                    case OpCode.Div:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] / registers[instruction.Operand2]);
                        break;
                    case OpCode.Divi:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] / instruction.Immediate);
                        break;
                    case OpCode.Mod:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] % registers[instruction.Operand2]);
                        break;
                    case OpCode.Modi:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] % instruction.Immediate);
                        break;
                    case OpCode.Shil:
                        registers[instruction.Operand1] = setFlags((uint)((int)registers[instruction.Operand1] << (int)registers[instruction.Operand2]));
                        break;
                    case OpCode.Shili:
                        registers[instruction.Operand1] = setFlags((uint)((int)registers[instruction.Operand1] << (int)instruction.Immediate)); 
                        break;
                    case OpCode.Shir:
                        registers[instruction.Operand1] = setFlags((uint)((int)registers[instruction.Operand1] >> (int)registers[instruction.Operand2]));
                        break;
                    case OpCode.Shiri:
                        registers[instruction.Operand1] = setFlags((uint)((int)registers[instruction.Operand1] >> (int)instruction.Immediate));
                        break;
                    case OpCode.And:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] & registers[instruction.Operand2]);
                        break;
                    case OpCode.Andi:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] & instruction.Immediate);
                        break;
                    case OpCode.Or:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] | registers[instruction.Operand2]);
                        break;
                    case OpCode.Ori:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] | instruction.Immediate);
                        break;
                    case OpCode.Xor:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] ^ registers[instruction.Operand2]);
                        break;
                    case OpCode.Xori:
                        registers[instruction.Operand1] = setFlags(registers[instruction.Operand1] ^ instruction.Immediate);
                        break;
                    case OpCode.Not:
                        registers[instruction.Operand1] = ~registers[instruction.Operand1];
                        break;
                    case OpCode.Cmp:
                        setFlags(registers[instruction.Operand1] - registers[instruction.Operand2]);
                        break;
                    case OpCode.Cmpi:
                        setFlags(registers[instruction.Operand1] - instruction.Immediate);
                        break;
                    case OpCode.Push:
                        push(registers[instruction.Operand1]);
                        break;
                    case OpCode.Pushi:
                        push(instruction.Immediate);
                        break;
                    case OpCode.Pop:
                        registers[instruction.Operand1] = pop();
                        break;
                    case OpCode.Popv:
                        pop();
                        break;
                    case OpCode.Mov:
                        registers[instruction.Operand1] = registers[instruction.Operand2];
                        break;
                    case OpCode.Call:
                        push(registers[IP]);
                        registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Ret:
                        registers[IP] = pop();
                        push(registers[instruction.Operand1]);
                        break;
                    case OpCode.Reti:
                        registers[IP] = pop();
                        push(instruction.Immediate);
                        break;
                    case OpCode.Retv:
                        registers[IP] = pop();
                        break;
                    case OpCode.Jmp:
                        registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Jie:
                        if ((registers[FLAGS] & ZERO_FLAG) == 1)
                            registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Jine:
                        if ((registers[FLAGS] & ZERO_FLAG) != 1)
                            registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Jig:
                        if ((registers[FLAGS] & SIGN_FLAG) == 0)
                            registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Jige:
                        if ((registers[FLAGS] & SIGN_FLAG) == 0 || (registers[FLAGS] & ZERO_FLAG) == 1)
                            registers[IP] = instruction.Immediate;
                        break;
                    case OpCode.Li:
                        registers[instruction.Operand1] = instruction.Immediate;
                        break;
                    case OpCode.Loadb:
                        registers[instruction.Operand1] = ram[registers[instruction.Operand2]];
                        break;
                    case OpCode.Loadbi:
                        registers[instruction.Operand1] = ram[instruction.Immediate];
                        break;
                    case OpCode.Loadw:
                        registers[instruction.Operand1] = (uint)BitConverter.ToUInt16(ram, (int)registers[instruction.Operand2]);
                        break;
                    case OpCode.Loadwi:
                        registers[instruction.Operand1] = (uint)BitConverter.ToUInt16(ram, (int)instruction.Immediate);
                        break;
                    case OpCode.Stob:
                        if (registers[instruction.Operand1] == SERIAL_POSITION)
                            serial.Write((char)(byte)registers[instruction.Operand2]);
                        else
                            ram[registers[instruction.Operand1]] = (byte)registers[instruction.Operand2];
                        break;
                    case OpCode.Stobi:
                        if (instruction.Immediate == SERIAL_POSITION)
                            serial.Write((char)(byte)registers[instruction.Operand1]);
                        else
                            ram[registers[instruction.Operand1]] = (byte)instruction.Immediate;
                        break;
                    case OpCode.Ston:
                        if (registers[instruction.Operand1] == SERIAL_POSITION)
                            serial.Write((char)(byte)registers[instruction.Operand1]);
                        else
                            Buffer.BlockCopy(BitConverter.GetBytes(registers[instruction.Operand2]), 0, ram, (int)registers[instruction.Operand1], (int)instruction.Immediate);
                        break;
                    case OpCode.Stow:
                        if (registers[instruction.Operand1] == SERIAL_POSITION)
                            serial.Write((char)(byte)registers[instruction.Operand1]);
                        else
                            BitConverter.GetBytes((ushort)registers[instruction.Operand2]).CopyTo(ram, registers[instruction.Operand1]);
                        break;
                    case OpCode.Stowi:
                        if (instruction.Immediate == SERIAL_POSITION)
                            serial.Write((char)(byte)registers[instruction.Operand1]);
                        else
                            BitConverter.GetBytes((ushort)registers[instruction.Operand1]).CopyTo(ram, instruction.Immediate);
                        break;
                    case OpCode.Gt:
                        registers[instruction.Operand1] = registers[instruction.Operand1] > registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Gti:
                        registers[instruction.Operand1] = registers[instruction.Operand1] > instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Gte:
                        registers[instruction.Operand1] = registers[instruction.Operand1] >= registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Gtei:
                        registers[instruction.Operand1] = registers[instruction.Operand1] >= instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Lt:
                        registers[instruction.Operand1] = registers[instruction.Operand1] < registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Lti:
                        registers[instruction.Operand1] = registers[instruction.Operand1] < instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Lte:
                        registers[instruction.Operand1] = registers[instruction.Operand1] <= registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Ltei:
                        registers[instruction.Operand1] = registers[instruction.Operand1] <= instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Eq:
                        registers[instruction.Operand1] = registers[instruction.Operand1] == registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Eqi:
                        registers[instruction.Operand1] = registers[instruction.Operand1] == instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Neq:
                        registers[instruction.Operand1] = registers[instruction.Operand1] != registers[instruction.Operand2] ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Neqi:
                        registers[instruction.Operand1] = registers[instruction.Operand1] != instruction.Immediate ? (uint)1 : (uint)0;
                        break;
                    case OpCode.Inc:
                        registers[instruction.Operand1]++;
                        break;
                    case OpCode.Dec:
                        registers[instruction.Operand1]--;
                        break;
                }
            }
        }

        private void push(UInt32 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            registers[SP] -= sizeof(UInt32);
            for (int i = 0; i < bytes.Length; i++)
                ram[registers[SP] + i] = bytes[i];
        }

        private UInt32 pop()
        {
            byte[] bytes = new byte[sizeof(UInt32)];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = ram[registers[SP] + i];
            registers[SP] += sizeof(UInt32);

            return BitConverter.ToUInt32(bytes, 0);
        }

        private UInt32 setFlags(UInt32 value)
        {
            unchecked
            {
                if (value == 0)
                    registers[FLAGS] |= ZERO_FLAG;
                else
                    registers[FLAGS] &= ~ZERO_FLAG;

                if ((UInt16)value < 0)
                    registers[FLAGS] |= SIGN_FLAG;
                else if ((UInt16)value > 0)
                    registers[FLAGS] &= ~SIGN_FLAG;
                return value;
            }
        }
    }
}

