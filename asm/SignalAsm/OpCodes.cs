using System;
using System.Text;

namespace SignalAsm
{
    public enum OpCode
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Shil,
        Shir,
        And,
        Or,
        Xor,
        Not,
        Cmp,
        Jie,
        Jine,
        Jig,
        Jige,
        Jil,
        Jile,
        Gt,
        Gte,
        Lt,
        Lte,
        Eq,
        Neq,
        Stob,
        Ston,
        Stow,
        Loadb,
        Loadw,
        Inc,
        Dec,
        Push,
        Pop,
        Mov,
        Jmp,
        Call,
        Ret,
        Li,
        Addi,
        Subi,
        Muli,
        Divi,
        Modi,
        Shili,
        Shiri,
        Andi,
        Ori,
        Xori,
        Cmpi,
        Stobi,
        Stowi,
        Gti,
        Gtei,
        Lti,
        Ltei,
        Eqi,
        Neqi,
        Loadbi,
        Loadwi,
        Pushi,
        Popv,
        Reti,
        Retv
    }

    public class OpCodeConverter
    {
        public static OpCode StringToOpCode(string str)
        {
            // Ensure that the format of the string matches the format of the OpCode enum
            StringBuilder sb = new StringBuilder(str.ToLower());
            sb[0] = char.ToUpper(sb[0]);
            str = sb.ToString();

            return (OpCode)Enum.Parse(typeof(OpCode), str, true);
        }
    }
}