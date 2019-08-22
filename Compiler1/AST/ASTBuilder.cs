using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Compiler1
{
    class ASTBuilder : llangBaseVisitor<ASTNode>
    {

        //Scope<TypeSymbol> varTypes;
        //Scope<TypeSymbol> functionTypes;

        public override ASTNode Visit(IParseTree tree)
        {
            //varTypes = new Scope<TypeSymbol>();
            //functionTypes = new Scope<TypeSymbol>();
            return tree.Accept(this);
        }

        public override ASTNode VisitProg([NotNull] llangParser.ProgContext context)
        {
            List<ASTNode> children = new List<ASTNode>();
            for (int i = 0; i < context.toplevel().Length; i++)
            {
                children.Add(Visit(context.toplevel(i)));
            }
            return new ProgNode(children, new SourceLoc(0, 0));
        }

        public override ASTNode VisitAssignStmt([NotNull] llangParser.AssignStmtContext context)
        {
            Expression lhs = (Expression) Visit(context.expr(0)), rhs = (Expression) Visit(context.expr(1));

            //if (!TypeSymbol.Match(lhs.type, rhs.type)) Program.ErrorExit("Type mismatch", context.Start.Line, context.Start.Column);

            if(context.assignop().GetText().Length == 2)
            {
                return new AugAssignNode(lhs, rhs, context.assignop().GetText(), MakeSourceLoc(context));
            }

            return new AssignNode(lhs, rhs, MakeSourceLoc(context));
        }

        public override ASTNode VisitBlock([NotNull] llangParser.BlockContext context)
        {
            List<Statement> stmts = new List<Statement>();

            for(int i = 0; i < context.ChildCount; i++)
            {
                stmts.Add((Statement)Visit(context.GetChild(i)));
            }

            return new BlockNode(stmts, MakeSourceLoc(context));
        }

        public override ASTNode VisitDeferstmt([NotNull] llangParser.DeferstmtContext context)
        {
            Expression e = (Expression)Visit(context.expr());
            return new DeferedNode(e, MakeSourceLoc(context));
        }

        public override ASTNode VisitEnumdef([NotNull] llangParser.EnumdefContext context)
        {
            string name = context.Iden().GetText();

            var items = new Dictionary<string, long>();

            long curval = 0;

            if(context.enumDeclList() != null)
            {
                var ctx = context.enumDeclList().GetChild<llangParser.EnumDeclItemContext>(0);
                for (int i = 1; ctx != null; i++)
                {
                    VarDeclNode v = (VarDeclNode)Visit(ctx);

                    if (v.rhs != null)
                    {
                        curval = v.rhs.IntValue + 1;
                    }
                    else
                    {
                        v.rhs = new IntExprNode(curval++, v.sourceLoc);
                    }
                    items.Add(v.name, v.rhs.IntValue);

                    ctx = context.enumDeclList().GetChild<llangParser.EnumDeclItemContext>(i);
                }
            }

            return new EnumDefNode(name, TypeSymbol.ENUM_SYMBOL(name, items), MakeSourceLoc(context));
        }

        public override ASTNode VisitEnumDeclItem([NotNull] llangParser.EnumDeclItemContext context)
        {
            string name = context.Iden().GetText();
            Expression rhs = null;
            if(context.Number() != null)
            {
                rhs = new IntExprNode(long.Parse(context.Number().GetText()), MakeSourceLoc(context));
            }

            return new VarDeclNode(name, rhs, TypeSymbol.INT_SYMBOL, MakeSourceLoc(context));
        }

        public override ASTNode VisitExpr([NotNull] llangParser.ExprContext context)
        {

            if (context.Iden() != null && context.expr().Length == 0)
            {
                string name = context.Iden().GetText();

                if (name == "null")
                    return new NullNode(MakeSourceLoc(context));

                return new IdenExprNode(name, TypeSymbol.INFER_SYMOBOL("__var__"), MakeSourceLoc(context));
            }
            else if (context.Number() != null)
            {
                if (context.Number().GetText().Contains("."))
                    return new FloatExprNode(float.Parse(context.Number().GetText()), MakeSourceLoc(context));
                return new IntExprNode(int.Parse(context.Number().GetText()), MakeSourceLoc(context));
            }
            else if (context.String() != null)
            {
                return new StringExprNode(context.String().GetText().Trim('\"'), MakeSourceLoc(context));
            }
            else if (context.varlist() != null)
            {
                Expression l = (Expression) Visit(context.varlist().expr(0));
                Expression u = (Expression) Visit(context.varlist().expr(1));
                Expression s = context.varlist().expr().Length == 2 ? new IntExprNode(1, MakeSourceLoc(context)) : (Expression)Visit(context.varlist().expr(2));
                return new VarListExprNode(l, u, s, MakeSourceLoc(context));
            }
            else if (context.elementlist() != null)
            {
                List<Expression> elems = new List<Expression>();
                if (context.elementlist() != null)
                {
                    for (int i = 0; i < context.elementlist().expr().Length; i++)
                    {
                        elems.Add((Expression)Visit(context.elementlist().expr(i)));
                    }
                }
                return new ConstListExprNode(elems, MakeSourceLoc(context));
            }
            else if (context.unaryop() != null)
            {
                Expression operand = (Expression)Visit(context.expr(0));
                return new UnaryExprNode(operand, context.unaryop().GetText(), MakeSourceLoc(context));
            }
            else if(context.multop() != null || context.addop() != null || context.shiftop() != null || context.compop() != null 
                || context.eqop() != null || context.bitwiseop() != null || context.logicop() != null)
            {
                return VisitBinExpr(context);
            }
            else if (context.Iden() != null && context.expr().Length == 1 && context.argslist() == null)
            {
                Expression basestruct = (Expression)Visit(context.expr(0));
                string fieldname = context.Iden().GetText();

                return new FieldAccessNode(basestruct, fieldname, MakeSourceLoc(context));
            }
            else if (context.Iden() != null && context.expr().Length == 1 && context.argslist() != null)
            {
                Expression e = (Expression)Visit(context.expr(0));
                List<Expression> args = new List<Expression>() { (Expression)Visit(context.expr(0)) };
                if (context.argslist() != null)
                {
                    for (int i = 0; i < context.argslist().expr().Length; i++)
                    {
                        args.Add((Expression)Visit(context.argslist().expr(i)));
                    }
                }
                return new ImplicitFunCallExprNode(context.Iden().GetText(), e, args, MakeSourceLoc(context));
            }
            else if (context.argslist() != null)
            {
                Expression e = (Expression)Visit(context.expr(0));
                List<Expression> args = new List<Expression>();
                if (context.argslist() != null)
                {
                    for(int i = 0; i < context.argslist().expr().Length; i++)
                    {
                        args.Add((Expression)Visit(context.argslist().expr(i)));
                    }
                }
                return new FunCallExprNode(e, args, MakeSourceLoc(context));
            }
            else if (context.expr().Length == 2)
            {
                Expression array = (Expression)Visit(context.expr(0));
                Expression idx = (Expression)Visit(context.expr(1));

                return new ArrayIndexNode(array, idx, MakeSourceLoc(context));
            }
            else if (context.expr().Length == 1)
            {
                Expression e = (Expression)Visit(context.expr(0));
                return e;
            }
            else if(context.typename() != null)
            {
                return new NewStructNode(context.typename().GetText(), MakeSourceLoc(context));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public ASTNode VisitBinExpr([NotNull] llangParser.ExprContext context)
        {
            if (context.multop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.multop().GetText(), MakeSourceLoc(context));
            }
            else if (context.addop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.addop().GetText(), MakeSourceLoc(context));
            }
            else if (context.shiftop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.shiftop().GetText(), MakeSourceLoc(context));
            }
            else if (context.compop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.compop().GetText(), MakeSourceLoc(context));
            }
            else if (context.eqop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.eqop().GetText(), MakeSourceLoc(context));
            }
            else if (context.bitwiseop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.bitwiseop().GetText(), MakeSourceLoc(context));
            }
            else if (context.logicop() != null)
            {
                Expression lhs = (Expression)Visit(context.expr(0));
                Expression rhs = (Expression)Visit(context.expr(1));
                return new BinaryExprNode(lhs, rhs, context.logicop().GetText(), MakeSourceLoc(context));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override ASTNode VisitFlowstmt([NotNull] llangParser.FlowstmtContext context)
        {

            Statement n = (Statement)Visit(context.GetChild(0));

            return n;
        }

        public override ASTNode VisitForstmt([NotNull] llangParser.ForstmtContext context)
        {

            string var = context.Iden().GetText();
            Expression inlist = (Expression)Visit(context.expr());

            Statement body = (Statement)Visit(context.stmt());

            return new ForNode(var, inlist, body, MakeSourceLoc(context));
        }

        public override ASTNode VisitFuncallStmt([NotNull] llangParser.FuncallStmtContext context)
        {
            Expression name = (Expression)Visit(context.expr());
            List<Expression> args = new List<Expression>();

            if(context.argslist() != null)
            {
                for(int i = 0; i < context.argslist().expr().Length; i++)
                {
                    Expression e = (Expression)Visit(context.argslist().expr(i));
                    args.Add(e);
                }
            }

            if(context.Iden() != null)
            {
                List<Expression> newargs = new List<Expression>(args.Count + 1) { (Expression)Visit(context.expr()) };
                newargs.AddRange(args);
                return new ImplicitFunCallStmtNode(context.Iden().GetText(), name, newargs, MakeSourceLoc(context));
            }
            else
            {
                return new FunCallStmtNode(name, args, MakeSourceLoc(context));
            }

        }

        public override ASTNode VisitFundef([NotNull] llangParser.FundefContext context)
        {
            string funname = context.Iden().GetText();
            List<string> args = new List<string>();
            List<TypeSymbol> argtypes = new List<TypeSymbol>();
            TypeSymbol rettype = TypeSymbol.INFER_SYMOBOL(context.typename().GetText());

            if (context.defargslist() != null)
            {
                foreach (llangParser.DefargitemContext c in context.defargslist().defargitem())
                {
                    args.Add(c.Iden().GetText());
                    argtypes.Add(TypeSymbol.INFER_SYMOBOL(c.typename().GetText()));
                }
            }

            TypeSymbol functionType = TypeSymbol.FUNCTION_SYMBOL(funname, funname, rettype, argtypes);

            Statement body = (Statement)Visit(context.block());

            return new FunDefNode(funname, args, argtypes, rettype, body, MakeSourceLoc(context));
        }

        public override ASTNode VisitGlobalVar([NotNull] llangParser.GlobalVarContext context)
        {

            VarDeclNode n = (VarDeclNode)Visit(context.varDeclStmt());

            return new GlobalVarDefNode(n.name, n.rhs, n.Type, MakeSourceLoc(context));
        }

        public override ASTNode VisitIfstmt([NotNull] llangParser.IfstmtContext context)
        {

            Expression cond = (Expression)Visit(context.expr());

            Statement ifbody = (Statement)Visit(context.stmt(0)), elsebody = null;

            if(context.stmt().Length > 1)
            {
                elsebody = (Statement)Visit(context.stmt(1));
            }

            return new IfNode(cond, ifbody, elsebody, MakeSourceLoc(context));
        }

        public override ASTNode VisitLibimport([NotNull] llangParser.LibimportContext context)
        {

            return new LibImportNode(context.Iden().GetText(), MakeSourceLoc(context));
        }

        public override ASTNode VisitRetstmt([NotNull] llangParser.RetstmtContext context)
        {

            Expression ret = null;
            if(context.expr() != null)
            {
                ret = (Expression)Visit(context.expr());
            }

            return new ReturnNode(ret, MakeSourceLoc(context));
        }

        public override ASTNode VisitStmt([NotNull] llangParser.StmtContext context)
        {

            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            return null;
        }

        public override ASTNode VisitStructdef([NotNull] llangParser.StructdefContext context)
        {
            string name = context.Iden().GetText();
            List<Expression> defaultValues = new List<Expression>();
            var fields = new Dictionary<string, (TypeSymbol, int)>();

            if (context.structDeclList() != null)
            {
                for (int i = 0; i < context.structDeclList().ChildCount; i++)
                {
                    VarDeclNode s = (VarDeclNode)Visit(context.structDeclList().GetChild(i));
                    fields.Add(s.name, (s.Type, 0));
                    defaultValues.Add(s.rhs);
                }
            }

            TypeSymbol ownType = TypeSymbol.STRUCT_SYMBOL(name, 0, fields);

            return new StructDefNode(name, ownType, defaultValues, MakeSourceLoc(context)); // Technically useless for Codegen
        }

        public override ASTNode VisitStructDecl([NotNull] llangParser.StructDeclContext context)
        {

            string name = context.Iden().GetText();
            Expression rhs = null;
            TypeSymbol ts = TypeSymbol.INFER_SYMOBOL(context.typename().GetText());
            if (context.expr() != null)
                rhs = (Expression)Visit(context.expr());

            SourceLoc sl = MakeSourceLoc(context);
            return new VarDeclNode(name, rhs, ts, sl);

        }

        public override ASTNode VisitToplevel([NotNull] llangParser.ToplevelContext context)
        {
            var n = Visit(context.GetChild(1));
            n.isExport = context.export().GetText() == "export";
            return n;
        }

        public override ASTNode VisitVarDeclStmt([NotNull] llangParser.VarDeclStmtContext context)
        {

            string name = context.Iden().GetText();
            Expression rhs = null;
            TypeSymbol ts = TypeSymbol.INFER_SYMOBOL(context.typename()?.GetText());
            if (context.expr() != null)
                rhs = (Expression)Visit(context.expr());

            return new VarDeclNode(name, rhs, ts, MakeSourceLoc(context));
        }

        public override ASTNode VisitVarlist([NotNull] llangParser.VarlistContext context)
        {
            return null;
        }

        public override ASTNode VisitWhilestmt([NotNull] llangParser.WhilestmtContext context)
        {

            Expression cond = (Expression)Visit(context.expr());
            Statement body = (Statement)Visit(context.stmt());

            return new WhileNode(cond, body, MakeSourceLoc(context));
        }


        public SourceLoc MakeSourceLoc(Antlr4.Runtime.ParserRuleContext context)
        {
            return new SourceLoc(context.Start.Line, context.Start.Column);
        }
    }
}
