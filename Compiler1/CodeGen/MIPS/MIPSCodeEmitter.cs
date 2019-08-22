using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class MIPSCodeEmitter : CodeEmitter
    {
        public MIPSCodeEmitter(string outfile) : base(outfile)
        {
        }

        public void Emit(string mnemonic, string d, string s, string t)
        {
            Emit(string.Format("{0} {1},{2},{3}", mnemonic, d, s, t));
        }

        public void Emit(string mnemonic, MIPSRegister d, MIPSRegister s, MIPSRegister t)
        {
            Emit(mnemonic, d.Name, s.Name, t.Name);
        }

        public void Emit(string mnemonic, MIPSRegister d, string s, MIPSRegister t)
        {
            Emit(mnemonic, d.Name, s, t.Name);
        }

        public void Emit(string mnemonic, MIPSRegister d, string s, string t)
        {
            Emit(mnemonic, d.Name, s, t);
        }

        public void Emit(string mnemonic, MIPSRegister d, MIPSRegister s, string t)
        {
            Emit(mnemonic, d.Name, s.Name, t);
        }

        public void Emit(string mnemonic, MIPSRegister s, MIPSRegister t)
        {
            Emit(mnemonic, s.Name, t.Name);
        }

        public void Emit(string mnemonic, string s, string t)
        {
            Emit(string.Format("{0} {1},{2}", mnemonic, s, t));
        }

        public void EmitImmediate(string mnemonic, string t, string s, short C)
        {
            Emit(string.Format("{0} {1},{2},{3}", mnemonic, t, s, C));
        }

        public void EmitImmediate(string mnemonic, MIPSRegister t, MIPSRegister s, short C)
        {
            EmitImmediate(t.Name, s.Name, C);
        }


        public void EmitImmediate(string mnemonic, string t, short C)
        {
            Emit(string.Format("{0} {1},{2}", mnemonic, t, C));
        }

        public void EmitLabelJump(string mnemonic, string s, string label)
        {
            Emit($"{mnemonic} {s}, {label}");
        }

        public void EmitLabelJump(string mnemonic, MIPSRegister s, string label)
        {
            EmitLabelJump(mnemonic, s.Name, label);
        }

        public void EmitLabelJump(string mnemonic, string s, string t, string label)
        {
            Emit($"{mnemonic} {s}, {t}, {label}");
        }

        public void EmitLabelJump(string mnemonic, MIPSRegister s, string t, string label)
        {
            EmitLabelJump(mnemonic, s.Name, t, label);
        }

        public void EmitLabelJump(string mnemonic, string s, MIPSRegister t, string label)
        {
            EmitLabelJump(mnemonic, s, t.Name, label);
        }

        public void EmitLabelJump(string mnemonic, MIPSRegister s, MIPSRegister t, string label)
        {
            EmitLabelJump(mnemonic, s.Name, t.Name, label);
        }

        public void EmitJump(string mnemonic, int C)
        {
            Emit(string.Format("{0} {1}", mnemonic, C));
        }

        public void EmitJump(string mnemonic, string C)
        {
            Emit($"{mnemonic} {C}");
        }

        public void EmitLoadStore(string mnemonic, string t, short C, string s)
        {
            Emit(string.Format("{0}, {1},{2}({3})", mnemonic, t, C, s));
        }

        public void EmitLoadStore(string mnemonic, MIPSRegister t, short C, string s)
        {
            EmitLoadStore(mnemonic, t.Name, C, s);
        }

        public void EmitLoadStore(string mnemonic, string t, short C, MIPSRegister s)
        {
            EmitLoadStore(mnemonic, t, C, s.Name);
        }

        public void EmitLoadStore(string mnemonic, MIPSRegister t, short C, MIPSRegister s)
        {
            EmitLoadStore(mnemonic, t.Name, C, s.Name);
        }

        public void Emit(string mnemonic, MIPSRegister d, int C)
        {
            Emit(string.Format("{0} {1},{2}", mnemonic, d.Name, C));
        }
        
        public void EmitLabel(string label)
        {
            Emit(label + ":");
        }

        public void EmitMalloc(MIPSRegister reg, short numbytes)
        {
            // put v0 + a0 on the stack
            EmitLoadStore("sw", "$v0", -4, "$sp");
            EmitLoadStore("sw", "$a0", -8, "$sp");
            // set v0 to 9 - sbrk
            EmitImmediate("addi", "$v0", "$zero", 9);
            // set a0 to num bytes
            EmitImmediate("addi", "$a0", "$zero", numbytes);
            // call syscall
            Emit("syscall");
            // put the returned address in v0 to reg
            Emit("add", reg, "$zero", "$v0");
            // restore v0 and a0
            EmitLoadStore("lw", "$v0", -4, "$sp");
            EmitLoadStore("lw", "$a0", -8, "$sp");

        }

    }
}
