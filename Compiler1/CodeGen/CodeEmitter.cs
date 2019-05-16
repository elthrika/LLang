using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    internal abstract class CodeEmitter
    {
        FileStream fs;

        public CodeEmitter(string outfile)
        {
            fs = File.OpenWrite(outfile);
            fs.Seek(0, SeekOrigin.Begin);
        }

        public void Emit(string asm)
        {
            fs.Write(Encoding.ASCII.GetBytes(asm), 0, asm.Length);
        }
        
        private int lablnumber = 0;
        public string GetUniqueLabel()
        {
            return "labl" + (lablnumber++) + ":";
        }

        public string GetLabel(string l)
        {
            return l + ":";
        }
    }
}
