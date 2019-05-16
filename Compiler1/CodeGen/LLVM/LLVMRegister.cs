using System;
using System.Text.RegularExpressions;

namespace Compiler1
{
    class LLVMRegister : Register
    {
        bool global;

        public LLVMRegister(string name, bool global = false) : base(name)
        {
            bool ok = false;
            if (name != null)
            {
                ok = Regex.IsMatch(name, "[%@][-a-zA-Z$._][-a-zA-Z$._0-9]*");
                ok &= global ? name.StartsWith("@") : name.StartsWith("%");
            }

            if (!ok) throw new ArgumentException("Illegal name for register");
            this.global = global;
        }

        public override object Clone()
        {
            return new LLVMRegister(Name, global);
        }
    }
}
