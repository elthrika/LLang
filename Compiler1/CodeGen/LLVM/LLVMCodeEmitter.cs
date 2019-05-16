using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class LLVMCodeEmitter : CodeEmitter
    {
        public LLVMCodeEmitter(string outfile) : base(outfile)
        {
            
        }

        private int tempregnumber = 0;
        public LLVMRegister GetTempRegister()
        {
            return new LLVMRegister("%tmp" + (tempregnumber++));
        }

        public void Emit(LLVMRegister result, string op, string type, LLVMRegister r1, LLVMRegister r2) //BinaryOp
        {
            Emit(String.Format("{0} = {1} {2} {3},{4}", result, op, type, r1, r2));
        }

        public void Emit(string v, string type, LLVMRegister retval)
        {
            Emit(String.Format("{0} {1} {2}", v, type, retval));
        }

        public void Emit(string s, LLVMRegister reg)
        {
            throw new NotImplementedException();
        }
    }
}
