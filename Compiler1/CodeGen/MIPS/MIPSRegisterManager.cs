using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class MIPSRegisterManager
    {

        string[] gpRegisters =
        {
            "$v0",
            "$v1",
            "$a0",
            "$a1",
            "$a2",
            "$a3",
            "$t0",
            "$t1",
            "$t2",
            "$t3",
            "$t4",
            "$t5",
            "$t6",
            "$t7",
            "$s0",
            "$s1",
            "$s2",
            "$s3",
            "$s4",
            "$s5",
            "$s6",
            "$s7",
            "$t8",
            "$t9"
        };

        string[] fpRegisters =
        {
            ""
        };

        List<MIPSRegister> availableRegisters;

        internal readonly MIPSRegister ZERO = new MIPSRegister("$zero");
        internal readonly MIPSRegister SP   = new MIPSRegister("$sp");
        internal readonly MIPSRegister FP   = new MIPSRegister("$fp");
        internal readonly MIPSRegister GP   = new MIPSRegister("$gp");
        internal readonly MIPSRegister RA   = new MIPSRegister("$ra");
        internal readonly MIPSRegister AT   = new MIPSRegister("$at");
        //internal readonly MIPSRegister K0   = new MIPSRegister("$k0");
        //internal readonly MIPSRegister K1   = new MIPSRegister("$k1");


        public MIPSRegisterManager()
        {
        }

        public void InitRegisters()
        {
            availableRegisters = new List<MIPSRegister>();
            for (int i = 0; i < gpRegisters.Length; i++)
            {
                availableRegisters[i] = new MIPSRegister(gpRegisters[i]);
            }
        }

        public MIPSRegister GetRegister()
        {
            if (availableRegisters.Count == 0)
                throw new Exception("No more registers!");

            MIPSRegister reg = availableRegisters[0];
            availableRegisters.RemoveAt(0);
            return reg;
        }

        public MIPSRegister GetRegister(string specific)
        {
            foreach (var reg in availableRegisters)
            {
                if (reg.Name == specific) return reg;
            }
            throw new Exception($"Register '{specific}' Not Available");
        }

        public void ReleaseRegister(MIPSRegister reg)
        {
            availableRegisters.Insert(0, reg);
        }
    }
}
