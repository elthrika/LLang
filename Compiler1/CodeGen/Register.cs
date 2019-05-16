using System;

namespace Compiler1
{
    internal class Register : ICloneable
    {
        public string Name;

        public Register(string name)
        {
            Name = name;
        }

        public virtual object Clone()
        {
            return new Register(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }

}