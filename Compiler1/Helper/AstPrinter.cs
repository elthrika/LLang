using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class AstPrinter : IASTVisitor<object>
    {
        string indent = "";

        void Indent()
        {
            indent += "| ";
        }

        void Dedent()
        {
            indent = indent.Substring(2);
        }

        void WriteLine(string msg)
        {
            Console.WriteLine(indent + msg);
        }

        void WriteLine(ASTKind k)
        {
            Console.WriteLine(indent + k);
        }


        public object Visit(ASTNode n)
        {
            return n?.Accept(this);
        }

        public object VisitProgNode(ProgNode n)
        {
            WriteLine(n.kind);

            Indent();
            foreach (ASTNode c in n.children)
            {
                Visit(c);
            }
            Dedent();

            return null;
        }

        public object VisitFunDefNode(FunDefNode n)
        {
            string args = "";
            for (int i = 0; i < n.args.Count; i++)
                args += n.args[i] + ":" + n.argtypes[i].Name + ",";
            WriteLine(n.kind + " - " + n.name + "(" + (args.Length == 0 ? "" : args.Substring(0, args.Length - 1)) + ") -> " + n.rettype.Name);
            Indent();
            Visit(n.body);
            Dedent();
            return null;
        }

        public object VisitGlobalVarDefNode(GlobalVarDefNode n)
        {
            WriteLine(n.kind + " - " + n.name + ":" + n.Type.Name);
            Indent();
            Visit(n.rhs);
            Dedent();

            return null;
        }

        public object VisitStructDefNode(StructDefNode n)
        {
            WriteLine(n.kind + " - " + n.name + "(" + n.Type.Length + ")");
            Indent();
            for (int i = 0; i < n.Type.Fields.Count; i++)
            {
                KeyValuePair<string, TypeSymbol> m = n.Type.Fields.ElementAt(i);
                WriteLine(m.Key + ":" + m.Value + "  +" + n.Type.Offsets[i]);
            }
            Dedent();

            return null;
        }

        public object VisitLibImportNode(LibImportNode n)
        {
            WriteLine(n.kind + " - " + n.libname);
            return null;
        }

        public object VisitVarDeclNode(VarDeclNode n)
        {
            WriteLine(n.kind + " - " + n.name + ":" + n.Type.Name);

            Indent();
            Visit(n.rhs);
            Dedent();

            return null;
        }

        public object VisitAssignNode(AssignNode n)
        {
            WriteLine(n.kind);
            Indent();
            Visit(n.lhs);
            Visit(n.rhs);
            Dedent();

            return null;
        }

        public object VisitAugAssignNode(AugAssignNode n)
        {
            WriteLine(n.kind + " - " + n.op);
            Indent();
            Visit(n.lhs);
            Visit(n.rhs);
            Dedent();

            return null;
        }

        public object VisitFunCallStmtNode(FunCallStmtNode n)
        {
            WriteLine(n.kind);
            Indent();
            Visit(n.name);
            foreach (Expression c in n.args)
                Visit(c);
            Dedent();

            return null;
        }

        public object VisitWhileNode(WhileNode n)
        {
            WriteLine(n.kind);
            Indent();
            Visit(n.test);
            Visit(n.body);
            Dedent();

            return null;
        }

        public object VisitForNode(ForNode n)
        {
            WriteLine(n.kind + " - " + n.var);
            Indent();
            Visit(n.inList);
            Visit(n.body);
            Dedent();

            return null;
        }

        public object VisitIfNode(IfNode n)
        {
            WriteLine(n.kind);
            Indent();
            Visit(n.test);
            Visit(n.body);
            Visit(n.elsebody);
            Dedent();

            return null;
        }

        public object VisitReturnNode(ReturnNode n)
        {
            WriteLine(n.kind);
            Indent();
            Visit(n.ret);
            Dedent();

            return null;
        }

        public object VisitBlockNode(BlockNode n)
        {
            WriteLine(n.kind);
            Indent();
            foreach(Statement s in n.stmts)
            {
                Visit(s);
            }
            Dedent();
            return null;
        }

        public object VisitIdenExprNode(IdenExprNode n)
        {
            WriteLine(n.kind + " - " + n.name + ":" + n.Type);
            return null;
        }

        public object VisitIntExprNode(IntExprNode n)
        {
            WriteLine(n.kind + " - " + n.IntValue + ":" + n.Type);
            return null;
        }

        public object VisitFloatExprNode(FloatExprNode n)
        {
            WriteLine(n.kind + " - " + n.FloatValue + ":" + n.Type);
            return null;
        }

        public object VisitStringExprNode(StringExprNode n)
        {
            WriteLine(n.kind + " - " + n.StringValue + ":" + n.Type);
            return null;
        }

        public object VisitListExprNode(ConstListExprNode n)
        {
            WriteLine(n.kind);
            Indent();
            foreach (Expression e in n.ListValue)
            {
                Visit(e);
            }
            Dedent();

            return null;
        }

        public object VisitUnaryExprNode(UnaryExprNode n)
        {
            WriteLine(n.kind + " - " + n.op + ":" + n.Type);
            Indent();
            Visit(n.expr);
            Dedent();

            return null;
        }

        public object VisitBinaryExprNode(BinaryExprNode n)
        {
            WriteLine(n.kind + " - " + n.op + ":" + n.Type);
            Indent();
            Visit(n.rhs);
            Visit(n.lhs);
            Dedent();

            return null;
        }

        public object VisitFunCallExprNode(FunCallExprNode n)
        {
            WriteLine(n.kind + " - " + n.Type);
            Indent();
            Visit(n.name);
            foreach (Expression c in n.args)
                Visit(c);
            Dedent();

            return null;
        }

        public object VisitConstListExprNode(VarListExprNode n)
        {
            WriteLine(n.kind + " - " + n.Type);
            Indent();
            Visit(n.lower);
            Visit(n.upper);
            Visit(n.stepsize);
            Dedent();

            return null;
        }

        public object VisitArrayIndexNode(ArrayIndexNode n)
        {
            WriteLine(n.kind + " - " + n.Type);
            Indent();
            Visit(n.array);
            Visit(n.index);
            Dedent();

            return null;
        }

        public object VisitFieldAccessNode(FieldAccessNode n)
        {
            WriteLine(n.kind + " - " + n.fieldname + ":" + n.Type);
            Indent();
            Visit(n.basestruct);
            Dedent();

            return null;
        }

        public object VisitNewStructNode(NewStructNode n)
        {
            WriteLine(n.kind + " - " + n.Type);
            return null;
        }

        public object VisitDeferedNode(DeferedNode n)
        {
            WriteLine(n.kind);
            Visit(n.expr);
            return null;
        }

        public object VisitNullNode(NullNode n)
        {
            WriteLine(n.kind);
            return null;
        }

        public object VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            WriteLine(n.kind + " - " + n.Type.ReturnType);
            Indent();
            WriteLine(n.fullname + ":" + n.funsig.Name);
            foreach (Expression c in n.args)
                Visit(c);
            Dedent();

            return null;
        }

        public object VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            WriteLine(n.kind);
            Indent();
            WriteLine(n.fullname + ":" + n.funsig.Name);
            foreach (Expression c in n.args)
                Visit(c);
            Dedent();

            return null;
        }

        public object VisitEnumDefNode(EnumDefNode n)
        {
            WriteLine(n.kind + " - " + n.enumname);
            Indent();
            foreach (var item in n.Type.EnumItems)
            {
                WriteLine(item.Key + " = " + item.Value);
            }
            Dedent();
            return null;
        }
    }
}
