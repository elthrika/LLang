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

        public void EmitImmediate(string mnemonic, string t, short C)
        {
            Emit(string.Format("{0} {1},{2}", mnemonic, t, C));
        }

        public void EmitJump(string mnemonic, int C)
        {
            Emit(string.Format("{0} {1}", mnemonic, C));
        }

        public void EmitLoadStore(string mnemonic, string t, short C, string s)
        {
            Emit(string.Format("{0}, {1},{2}({3})", mnemonic, t, C, s));
        }

        public void EmitLoadStore(string mnemonic, MIPSRegister t, short C, MIPSRegister s)
        {
            EmitLoadStore(mnemonic, t.Name, C, s.Name);
        }

        public void Emit(string mnemonic, MIPSRegister d, int C)
        {
            Emit(string.Format("{0} {1},{2}", mnemonic, d.Name, C));
        }

    }
}
