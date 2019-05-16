using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class BaseASTVisitor<T> : IASTVisitor<T>
    {
        public virtual T Visit(ASTNode n)
        {
            return n == null ? default(T) : n.Accept(this);
        }

        public virtual T VisitArrayIndexNode(ArrayIndexNode n)
        {

            T a = n.array.Accept(this);
            T b = n.index.Accept(this);

            return default(T);
        }

        public virtual T VisitAssignNode(AssignNode n)
        {

            T a = n.lhs.Accept(this);
            T b = n.rhs.Accept(this);

            return default(T);
        }

        public virtual T VisitAugAssignNode(AugAssignNode n)
        {

            T a = n.lhs.Accept(this);
            T b = n.rhs.Accept(this);

            return default(T);
        }

        public virtual T VisitBinaryExprNode(BinaryExprNode n)
        {

            T a = n.lhs.Accept(this);
            T b = n.rhs.Accept(this);

            return default(T);
        }

        public virtual T VisitBlockNode(BlockNode n)
        {

            for(int i = 0; i < n.stmts.Count; i++)
            {
                n.stmts[i]?.Accept(this);
            }

            return default(T);
        }

        public virtual T VisitConstListExprNode(VarListExprNode n)
        {

            T a = n.lower.Accept(this);
            T b = n.upper.Accept(this);
            T c = n.stepsize.Accept(this);

            return default(T);
        }

        public virtual T VisitDeferedNode(DeferedNode n)
        {
            T a = Visit(n.expr);

            return default(T);
        }

        public virtual T VisitFieldAccessNode(FieldAccessNode n)
        {

            T a = n.basestruct.Accept(this);

            return default(T);
        }

        public virtual T VisitFloatExprNode(FloatExprNode n)
        {

            return default(T);
        }

        public virtual T VisitForNode(ForNode n)
        {

            T a = n.inList.Accept(this);
            T b = n.body.Accept(this);

            return default(T);
        }

        public virtual T VisitFunCallExprNode(FunCallExprNode n)
        {

            T a = n.name.Accept(this);
            for (int i = 0; i < n.args.Count; i++)
            {
                n.args[i].Accept(this);
            }

            return default(T);
        }

        public virtual T VisitFunCallStmtNode(FunCallStmtNode n)
        {

            T a = n.name.Accept(this);
            for(int i = 0; i < n.args.Count; i++)
            {
                n.args[i].Accept(this);
            }

            return default(T);
        }

        public virtual T VisitFunDefNode(FunDefNode n)
        {

            T a = n.body.Accept(this);

            return default(T);
        }

        public virtual T VisitGlobalVarDefNode(GlobalVarDefNode n)
        {

            T b = n.rhs.Accept(this);

            return default(T);
        }

        public virtual T VisitIdenExprNode(IdenExprNode n)
        {

            return default(T);
        }

        public virtual T VisitIfNode(IfNode n)
        {

            T a = n.test.Accept(this);
            T b = n.body.Accept(this);
            if (n.elsebody != null)
            {
                T c = n.elsebody.Accept(this);
            }
            return default(T);
        }

        public virtual T VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            VisitFunCallExprNode(n);

            return default(T);
        }

        public virtual T VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            VisitFunCallStmtNode(n);

            return default(T);
        }

        public virtual T VisitIntExprNode(IntExprNode n)
        {

            return default(T);
        }

        public virtual T VisitLibImportNode(LibImportNode n)
        {

            return default(T);
        }

        public virtual T VisitListExprNode(ConstListExprNode n)
        {

            for(int i = 0; i < n.ListValue.Count; i++)
            {
                n.ListValue[i].Accept(this);
            }

            return default(T);
        }

        public virtual T VisitNewStructNode(NewStructNode n)
        {
            return default(T);
        }

        public virtual T VisitNullNode(NullNode n)
        {
            return default(T);
        }

        public virtual T VisitProgNode(ProgNode n)
        {
            foreach (var c in n.children)
            {
                Visit(c);
            }
            
            return default(T);
        }

        public virtual T VisitReturnNode(ReturnNode n)
        {

            T a = n.ret.Accept(this);

            return default(T);
        }

        public virtual T VisitStringExprNode(StringExprNode n)
        {

            return default(T);
        }

        public virtual T VisitStructDefNode(StructDefNode n)
        {

            for(int i = 0; i < n.defaultValues.Count; i++)
            {
                n.defaultValues[i]?.Accept(this);
            }

            return default(T);
        }

        public virtual T VisitUnaryExprNode(UnaryExprNode n)
        {

            T a = n.expr.Accept(this);

            return default(T);
        }

        public virtual T VisitVarDeclNode(VarDeclNode n)
        {

            T a = n.rhs.Accept(this);

            return default(T);
        }

        public virtual T VisitWhileNode(WhileNode n)
        {

            T a = n.test.Accept(this);
            T b = n.body.Accept(this);

            return default(T);
        }
    }
}
