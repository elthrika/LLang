using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Compiler1
{
    class Interpreter
    {
        ASTNode Root;
        Dictionary<string, FunDefNode> Functions;
        Dictionary<string, TypeSymbol> Types;
        Scope<LData> globals;
        Scope<LData> current;

        public Interpreter(ASTNode root)
        {
            Root = root;
            Functions = new Dictionary<string, FunDefNode>();
            Types = new Dictionary<string, TypeSymbol>();
            globals = new Scope<LData>();
        }

        public void Run()
        {
            // Build types
            new TypeBuilder(this).Visit(Root);
            // Find all functions and get all globals
            new FunctionAndGlobalFinder(this).Visit(Root);

            // Run main - with Command line arguments
            var args = Environment.GetCommandLineArgs().Skip(2);
            CallFunction("main`([string])", new LData[1] { new LArray(args.Select(s => new LString(s)).ToArray()) });

            Console.WriteLine("Finished Execution");
            //Console.WriteLine(globals.ToString());
        }
        
        public LData CallFunction(string name, ICollection<LData> parameters)
        {
            //Console.WriteLine($"Calling: {name}");
            if (Intrinsics.LIntrinsics.ContainsKey(name)) {
                return Intrinsics.LIntrinsics[name].InterpImplementation(parameters);
            }

            current = globals.GoDown();
            current.PutAllInScope(Functions[name].args, parameters);
            var res = new InterpreterVisitor(this, current).Visit(Functions[name]);
            current = current.GoUp();
            return res;
        }

        public LData Eval(Expression expr)
        {
            return new InterpreterVisitor(this, globals).Visit(expr);
        }

        internal class ExecutionTerminatedException : Exception
        {

            public ExecutionTerminatedException(long exitcode)
            {
                ExitCode = exitcode;
            }
            public long ExitCode;
            public override string Message => "Interpreter Execution Terminated";
        }

        private class InterpreterVisitor : BaseASTVisitor<LData>
        {
            Scope<LData> Vars;
            Interpreter interp;
            List<Expression> deferredExpressions;

            LData RetVal;

            public InterpreterVisitor(Interpreter interpreter, Scope<LData> globals)
            {
                Vars = new Scope<LData>(globals);
                interp = interpreter;
                deferredExpressions = new List<Expression>();
            }

            public override LData Visit(ASTNode n)
            {
                //Console.WriteLine(n.sourceLoc.ToString());
                var val = base.Visit(n);
                if (RetVal != null) return RetVal;
                return val;
            }

            public override LData VisitArrayIndexNode(ArrayIndexNode n)
            {
                LData arr = Visit(n.array);
                LData idx = Visit(n.index);

                return arr.GetValue()._arr[idx.GetValue()];
            }

            public override LData VisitAssignNode(AssignNode n)
            {
                LData lhs = Visit(n.lhs);
                LData rhs = Visit(n.rhs);
                lhs.SetValue(rhs);
                return null;
            }

            public override LData VisitAugAssignNode(AugAssignNode n)
            {
                LData lhs = Visit(n.lhs);
                LData rhs = Visit(n.rhs);

                switch (n.op)
                {
                    case "*=":
                        {
                            dynamic newval = lhs.GetValue() * rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "/=":
                        {
                            dynamic newval = lhs.GetValue() / rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "%=":
                        {
                            dynamic newval = lhs.GetValue() % rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "+=":
                        {
                            dynamic newval = lhs.GetValue() + rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "-=":
                        {
                            dynamic newval = lhs.GetValue() - rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "<<=":
                        {
                            dynamic newval = lhs.GetValue() << rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case ">>=":
                        {
                            dynamic newval = lhs.GetValue() >> rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "&=":
                        {
                            dynamic newval = lhs.GetValue() & rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "^=":
                        {
                            dynamic newval = lhs.GetValue() ^ rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;
                    case "|=":
                        {
                            dynamic newval = lhs.GetValue() | rhs.GetValue();
                            lhs.SetValue(newval);
                        }
                        break;

                }

                return lhs;
            }

            public override LData VisitBinaryExprNode(BinaryExprNode n)
            {
                LData lhs = Visit(n.lhs);
                LData rhs = Visit(n.rhs);

                LData newdat = LDataMaker.GetDataFor(n.Type);
                switch (n.op)
                {
                    case "*":
                        {
                            newdat.SetValue(lhs.GetValue() * rhs.GetValue());
                        }
                        break;
                    case "/":
                        {
                            newdat.SetValue(lhs.GetValue() / rhs.GetValue());
                        }
                        break;
                    case "+":
                        {
                            newdat.SetValue(lhs.GetValue() + rhs.GetValue());
                        }
                        break;
                    case "-":
                        {
                            newdat.SetValue(lhs.GetValue() - rhs.GetValue());
                        }
                        break;
                    case ">>":
                        {
                            newdat.SetValue(lhs.GetValue() >> rhs.GetValue());
                        }
                        break;
                    case "<<":
                        {
                            newdat.SetValue(lhs.GetValue() << rhs.GetValue());
                        }
                        break;
                    case "&":
                        {
                            newdat.SetValue(lhs.GetValue() & rhs.GetValue());
                        }
                        break;
                    case "|":
                        {
                            newdat.SetValue(lhs.GetValue() | rhs.GetValue());
                        }
                        break;
                    case "^":
                        {
                            newdat.SetValue(lhs.GetValue() ^ rhs.GetValue());
                        }
                        break;
                    case "%":
                        {
                            newdat.SetValue(lhs.GetValue() % rhs.GetValue());
                        }
                        break;
                    case "<":
                        {
                            newdat.SetValue(lhs.GetValue() < rhs.GetValue());
                        }
                        break;
                    case ">":
                        {
                            newdat.SetValue(lhs.GetValue() > rhs.GetValue());
                        }
                        break;
                    case "<=":
                        {
                            newdat.SetValue(lhs.GetValue() <= rhs.GetValue());
                        }
                        break;
                    case ">=":
                        {
                            newdat.SetValue(lhs.GetValue() >= rhs.GetValue());
                        }
                        break;
                    case "==":
                        {
                            newdat.SetValue(lhs.GetValue() == rhs.GetValue());
                        }
                        break;
                    case "!=":
                        {
                            newdat.SetValue(lhs.GetValue() != rhs.GetValue());
                        }
                        break;
                    case "&&":
                        {
                            newdat.SetValue(lhs.GetValue() && rhs.GetValue());
                        }
                        break;
                    case "||":
                        {
                            newdat.SetValue(lhs.GetValue() || rhs.GetValue());
                        }
                        break;
                    default:
                        throw new Exception($"Did not match the binop: {n.op} at {n.sourceLoc}");
                }
                return newdat;

            }

            public override LData VisitConstListExprNode(VarListExprNode n)
            {
                LData upper = Visit(n.upper);
                LData lower = Visit(n.lower);
                LData stepsize = Visit(n.stepsize);

                LData current = LDataMaker.GetDataFor(n.Type.ArrayType);
                current.SetValue(lower.GetValue());

                List<LData> arr = new List<LData>();

                while(current.GetValue() < upper.GetValue())
                {
                    arr.Add(current);
                    LData prev = current;
                    current = LDataMaker.GetDataFor(n.Type.ArrayType);
                    current.SetValue(prev.GetValue() + stepsize.GetValue());
                }

                return new LArray(arr);
            }

            public override LData VisitDeferedNode(DeferedNode n)
            {
                deferredExpressions.Add(n.expr);
                return null;
            }

            public override LData VisitFieldAccessNode(FieldAccessNode n)
            {
                LData struc = Visit(n.basestruct);
                return struc.GetValue(n.fieldname);
            }

            public override LData VisitFloatExprNode(FloatExprNode n)
            {
                return new LFloat(n.FloatValue);
            }

            public override LData VisitForNode(ForNode n)
            {
                LData list = Visit(n.inList);
                var val = list.GetValue();
                LData it = LDataMaker.GetDataFor(n.inList.Type.ArrayType);
                Vars = Vars.GoDown();
                Vars.PutInScope(n.var, it);
                for (int i = 0; i < val.length; i++)
                {
                    it.SetValue(val._arr[i].GetValue());
                    Visit(n.body);
                }

                Vars = Vars.GoUp();
                return null;
            }

            public override LData VisitFunCallExprNode(FunCallExprNode n)
            {
                if(n.name is IdenExprNode)
                {
                    return interp.CallFunction((n.name as IdenExprNode).Type.FunctionName, n.args.Select(e => Visit(e)).ToArray());
                }
                else
                {
                    // ??? i don't know yet ... this concerns function pointers, and i honestly don't know how to do them elegantly
                    // @TODO: Probably has to be done inside TypeSymbol
                    return null;
                }
            }

            public override LData VisitFunCallStmtNode(FunCallStmtNode n)
            {
                if (n.name is IdenExprNode)
                {
                    var paras = n.args.Select(e => Visit(e)).ToArray();
                    interp.CallFunction((n.name as IdenExprNode).Type.FunctionName, paras);
                }
                else
                {
                    // ??? i don't know yet ... this concerns function pointers, and i honestly don't know how to do them elegantly
                    // @TODO: Probably has to be done inside TypeSymbol
                }
                return null;
            }

            public override LData VisitFunDefNode(FunDefNode n)
            {
                // This is our entry point
                Visit(n.body);
                // at the end, we have to call all the defered statements
                foreach (var defer in deferredExpressions)
                {
                    Visit(defer);
                }
                return null;
            }

            public override LData VisitIdenExprNode(IdenExprNode n)
            {
                return Vars.IsInScope(n.name);
            }

            public override LData VisitIfNode(IfNode n)
            {
                LData cond = Visit(n.test);
                if(cond.GetValue() == true)
                {
                    Visit(n.body);
                }
                else if(n.elsebody != null)
                {
                    Visit(n.elsebody);
                }

                return null;
            }

            public override LData VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
            {
                return interp.CallFunction(TypeSymbol.MakeFunctionName(n.fullname, n.args), n.args.Select(e => Visit(e)).ToArray());
            }

            public override LData VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
            {
                interp.CallFunction(TypeSymbol.MakeFunctionName(n.fullname, n.args), n.args.Select(e => Visit(e)).ToArray());
                return null;
            }

            public override LData VisitIntExprNode(IntExprNode n)
            {
                return new LInt(n.IntValue);
            }

            public override LData VisitLibImportNode(LibImportNode n)
            {
                return base.VisitLibImportNode(n);
            }

            public override LData VisitListExprNode(ConstListExprNode n)
            {
                var arr = new LArray(n.ListValue.Select(e => Visit(e)).ToArray());
                return arr;
            }

            public override LData VisitNewStructNode(NewStructNode n)
            {
                var struc = new LStruct(n.Type.Fields, n.Type);
                return struc;
            }

            public override LData VisitNullNode(NullNode n)
            {
                return LNull.NULL;
            }

            public override LData VisitReturnNode(ReturnNode n)
            {
                RetVal = Visit(n.ret);

                return null;
            }

            public override LData VisitStringExprNode(StringExprNode n)
            {
                return new LString(n.StringValue);
            }

            public override LData VisitUnaryExprNode(UnaryExprNode n)
            {
                LData val = LDataMaker.GetDataFor(n.Type);
                switch (n.op)
                {
                    case "-":
                        {
                            val.SetValue(-Visit(n.expr).GetValue());
                        }
                        break;
                    case "!":
                        {
                            val.SetValue(!Visit(n.expr).GetValue());
                        }
                        break;
                    case "~":
                        {
                            val.SetValue(~Visit(n.expr).GetValue());
                        }
                        break;

                }
                return val;
            }

            public override LData VisitVarDeclNode(VarDeclNode n)
            {
                Vars.PutInScope(n.name, Visit(n.rhs));
                return null;
            }

            public override LData VisitWhileNode(WhileNode n)
            {
                while (Visit(n.test).GetValue())
                {
                    Visit(n.body);
                }
                return null;
            }
        }

        private class FunctionAndGlobalFinder : BaseASTVisitor<object>
        {
            Interpreter interp;

            public FunctionAndGlobalFinder(Interpreter interpreter)
            {
                interp = interpreter;
            }

            public override object VisitFunDefNode(FunDefNode n)
            {
                interp.Functions.Add(n.funtype.FunctionName, n);
                return null;
            }

            public override object VisitGlobalVarDefNode(GlobalVarDefNode n)
            {
                interp.globals.PutInScope(n.name, interp.Eval(n.rhs));
                return null;
            }

            public override object VisitEnumDefNode(EnumDefNode n)
            {
                interp.globals.PutInScope(n.enumname, new LEnum(n.enumname, n.Type.EnumItems));

                return null;
            }
        }

        private class TypeBuilder : BaseASTVisitor<object>
        {
            Interpreter interp;

            public TypeBuilder(Interpreter interpreter)
            {
                interp = interpreter;
                interp.Types.Add("int", TypeSymbol.INT_SYMBOL);
                interp.Types.Add("float", TypeSymbol.FLOAT_SYMBOL);
                interp.Types.Add("bool", TypeSymbol.BOOL_SYMBOL);
                interp.Types.Add("string", TypeSymbol.STRING_SYMBOL);
                interp.Types.Add("void", TypeSymbol.VOID_SYMBOL);
            }

            public override object VisitStructDefNode(StructDefNode n)
            {
                interp.Types.Add(n.name, n.Type);
                return null;
            }
        }

    }
}
