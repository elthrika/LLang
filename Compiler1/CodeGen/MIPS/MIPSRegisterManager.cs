using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class MIPSRegisterManager
    {

        string[] unmutRegisters =
        {
            "$zero",
            "$at",
            "$k0",
            "$k1",
            "$gp",
            "$sp",
            "$fp",
            "$ra"
        };

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

        List<MIPSRegister> availableRegisters;

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

        public void ReleaseRegister(MIPSRegister reg)
        {
            availableRegisters.Insert(0, reg);
        }
    }
}
