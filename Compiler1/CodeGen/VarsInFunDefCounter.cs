using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class VarsInFunDefCounter : BaseASTVisitor<int>
    {
        public override int VisitFunDefNode(FunDefNode n)
        {
            return Visit(n.body);
        }

        public override int VisitBlockNode(BlockNode n)
        {
            int sum = 0;
            foreach (var stmt in n.stmts)
            {
                sum += Visit(stmt);
            }
            return sum;
        }

        public override int VisitVarDeclNode(VarDeclNode n)
        {
            return 1;
        }
    }
}
