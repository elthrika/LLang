using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class SemanticChecker
    {
        private int errors = 0;

        public SemanticChecker()
        {
        }

        public int Check(ASTNode root)
        {
            TypeChecker tc = new TypeChecker(this);
            tc.Visit(root);

            MainChecker mc = new MainChecker();
            mc.Visit(root);
            CheckAndReport(mc.FoundMain, new SourceLoc(), "No Main-Function found");

            AllPathsReturnChecker aprc = new AllPathsReturnChecker(this);
            aprc.Visit(root);

            return errors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckAndReport(bool expression, SourceLoc sl, string msg)
        {
            if (!expression)
            {
                Console.WriteLine("TypeError: line {0}, col {1} :: {2}", sl.Row, sl.Col, msg);
                errors++;
                return false;
            }
            return true;
        }

        private class TypeChecker : BaseASTVisitor<object>
        {
            SemanticChecker semanticChecker;

            Scope<TypeSymbol> varTypes = new Scope<TypeSymbol>();
            TypeSymbol currentFunction = null;

            public TypeChecker(SemanticChecker sc)
            {
                semanticChecker = sc;
            }

            public override object Visit(ASTNode n)
            {
                return n?.Accept(this);
            }

            public override object VisitArrayIndexNode(ArrayIndexNode n)
            {
                Visit(n.array);
                Visit(n.index);
                semanticChecker.CheckAndReport(n.array.Type.Kind == TypeSymbol.TypeKind.ARRAY, n.sourceLoc, "Array not typeof Array");
                semanticChecker.CheckAndReport(n.index.Type.Equals(TypeSymbol.INT_SYMBOL), n.sourceLoc, "Index not typeof int");
                return null;
            }

            public override object VisitAssignNode(AssignNode n)
            {
                Visit(n.lhs); Visit(n.rhs);
                semanticChecker.CheckAndReport(n.lhs.Type.Match(n.rhs.Type), n.sourceLoc, "Type mismatch");
                return null;
            }

            public override object VisitAugAssignNode(AugAssignNode n)
            {
                Visit(n.lhs); Visit(n.rhs);
                semanticChecker.CheckAndReport(n.lhs.Type.Match(n.rhs.Type), n.sourceLoc, "Type mismatch - AugAssign");
                return null;
            }

            public override object VisitBinaryExprNode(BinaryExprNode n)
            {
                Visit(n.lhs); Visit(n.rhs);

                switch (n.op)
                {
                    case "*":
                        {
                            semanticChecker.CheckAndReport(n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "type mismatch");
                            semanticChecker.CheckAndReport(n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) || n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "type mismatch");
                            semanticChecker.CheckAndReport(n.Type.Match(n.lhs.Type) && n.Type.Match(n.rhs.Type), n.sourceLoc, "type mismatch");
                        }
                        break;
                    case "/":
                    case "%":
                    case "+":
                    case "-":
                    case "<":
                    case ">":
                    case "<=":
                    case ">=":
                        {
                            semanticChecker.CheckAndReport(n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "type mismatch");
                            semanticChecker.CheckAndReport(n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) || n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "type mismatch");
                        }
                        break;
                    case "==":
                    case "!=":
                        {
                            semanticChecker.CheckAndReport(n.rhs.Type.Match(n.lhs.Type), n.sourceLoc, "type mismatch");
                        }
                        break;
                    case ">>":
                    case "<<":
                    case "&":
                    case "|":
                    case "^":
                        {
                            semanticChecker.CheckAndReport(n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL), n.sourceLoc, "type mismatch");
                            semanticChecker.CheckAndReport(n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL), n.sourceLoc, "type mismatch");
                        }
                        break;
                    case "&&":
                    case "||":
                        {
                            semanticChecker.CheckAndReport(n.rhs.Type.Equals(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "type mismatch");
                            semanticChecker.CheckAndReport(n.lhs.Type.Equals(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "type mismatch");
                        }
                        break;


                }
                return null;
            }

            public override object VisitConstListExprNode(VarListExprNode n)
            {
                Visit(n.lower); Visit(n.upper); Visit(n.stepsize);
                semanticChecker.CheckAndReport(n.lower.Type.Match(n.upper.Type), n.sourceLoc, "Type mismatch");
                semanticChecker.CheckAndReport(n.lower.Type.Match(n.stepsize.Type), n.sourceLoc, "Type mismatch");
                return null;
            }

            public override object VisitFieldAccessNode(FieldAccessNode n)
            {
                Visit(n.basestruct);

                if(n.basestruct.Type.Kind == TypeSymbol.TypeKind.STRUCT)
                {
                    semanticChecker.CheckAndReport(n.basestruct.Type.Fields.ContainsKey(n.fieldname), n.sourceLoc, "Undeclared field");
                }
                else if(n.basestruct.Type.Kind == TypeSymbol.TypeKind.POINTER)
                {
                    semanticChecker.CheckAndReport(TypeMaker.MakeTypeSymbolForString(n.basestruct.Type.Name).Fields.ContainsKey(n.fieldname), n.sourceLoc, "Undeclared Field");
                }
                else
                {
                    semanticChecker.CheckAndReport(n.basestruct.Type.Kind == TypeSymbol.TypeKind.STRUCT, n.sourceLoc, "Struct not typeof struct or pointer");
                }
                return null;
            }

            public override object VisitForNode(ForNode n)
            {
                Visit(n.inList);
                semanticChecker.CheckAndReport(n.inList.Type.Kind == TypeSymbol.TypeKind.ARRAY, n.sourceLoc, "Array not typeof Array");
                varTypes = varTypes.GoDown();
                varTypes.PutInScope(n.var, n.inList.Type.ArrayType);
                Visit(n.body);
                varTypes = varTypes.GoUp();

                return null;
            }

            public override object VisitFunCallExprNode(FunCallExprNode n)
            {
                Visit(n.name);
                TypeSymbol funType = null;
                if(n.name is IdenExprNode)
                {
                    var name = (n.name as IdenExprNode).name;
                    funType = varTypes.IsInScope(name);
                }
                else
                {
                    funType = n.name.Type;
                }
                semanticChecker.CheckAndReport(funType != null, n.sourceLoc, "Undeclared Function");
                if(funType != null)
                {
                    semanticChecker.CheckAndReport(n.args.Count <= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too many arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    semanticChecker.CheckAndReport(n.args.Count >= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too few arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    for (int i = 0; i < Math.Min(funType.ParameterTypes.Count, n.args.Count); i++)
                    {
                        Visit(n.args[i]);
                        semanticChecker.CheckAndReport(funType.ParameterTypes[i].Match(n.args[i].Type), n.sourceLoc, "argument mismatch: " + (i+1));
                    }
                }
                return null;
            }

            public override object VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
            {
                Visit(n.name);
                TypeSymbol funType = null;
                if (n.name is IdenExprNode)
                {
                    var name = (n.name as IdenExprNode).Type.Name + n.funname;
                    funType = varTypes.IsInScope(name);
                }
                else
                {
                    funType = n.name.Type;
                }
                semanticChecker.CheckAndReport(funType != null, n.sourceLoc, "Undeclared Function");
                if (funType != null)
                {
                    semanticChecker.CheckAndReport(n.args.Count <= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too many arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    semanticChecker.CheckAndReport(n.args.Count >= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too few arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    for (int i = 0; i < Math.Min(funType.ParameterTypes.Count, n.args.Count); i++)
                    {
                        Visit(n.args[i]);
                        semanticChecker.CheckAndReport(funType.ParameterTypes[i].Match(n.args[i].Type), n.sourceLoc, "argument mismatch: " + (i + 1));
                    }
                }
                return null;
            }

            public override object VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
            {
                Visit(n.name);
                TypeSymbol funType = null;
                if (n.name is IdenExprNode)
                {
                    var name = (n.name as IdenExprNode).Type.Name + n.funname;
                    funType = varTypes.IsInScope(name);
                }
                else
                {
                    funType = n.name.Type;
                }

                semanticChecker.CheckAndReport(funType != null, n.sourceLoc, "Undeclared Function");

                if (funType != null)
                {
                    semanticChecker.CheckAndReport(n.args.Count <= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too many arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    semanticChecker.CheckAndReport(n.args.Count >= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too few arguments ({0}) given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    for (int i = 0; i < Math.Min(funType.ParameterTypes.Count, n.args.Count); i++)
                    {
                        Visit(n.args[i]);
                        semanticChecker.CheckAndReport(funType.ParameterTypes[i].Match(n.args[i].Type), n.sourceLoc, "argument mismatch: " + (i + 1));
                    }
                }
                return null;
            }

            public override object VisitFunCallStmtNode(FunCallStmtNode n)
            {
                Visit(n.name);
                TypeSymbol funType = null;
                if (n.name is IdenExprNode)
                {
                    var name = (n.name as IdenExprNode).name;
                    funType = varTypes.IsInScope(name);
                }
                else
                {
                    funType = n.name.Type;
                }

                semanticChecker.CheckAndReport(funType != null, n.sourceLoc, "Undeclared Function");

                if (funType != null)
                {
                    semanticChecker.CheckAndReport(n.args.Count <= funType.ParameterTypes.Count, n.sourceLoc, string.Format("Too many arguments {0} given, {1} expected", n.args.Count, funType.ParameterTypes.Count));
                    semanticChecker.CheckAndReport(n.args.Count >= funType.ParameterTypes.Count, n.sourceLoc, "Too few arguments");

                    for (int i = 0; i < Math.Min(funType.ParameterTypes.Count, n.args.Count); i++)
                    {
                        Visit(n.args[i]);
                        semanticChecker.CheckAndReport(funType.ParameterTypes[i].Match(n.args[i].Type), n.sourceLoc, "argument mismatch: " + (i + 1));
                    }
                }
                return null;
            }

            public override object VisitFunDefNode(FunDefNode n)
            {
                semanticChecker.CheckAndReport(varTypes.PutInScope(n.name, n.funtype), n.sourceLoc, $"Function {n.name} already declared");
                currentFunction = n.funtype;

                varTypes = varTypes.GoDown();
                varTypes.PutAllInScope(n.args, n.argtypes);

                Visit(n.body);

                varTypes = varTypes.GoUp();

                return null;
            }

            public override object VisitGlobalVarDefNode(GlobalVarDefNode n)
            {
                Visit(n.rhs);
                semanticChecker.CheckAndReport(varTypes.PutInScope(n.name, n.Type), n.sourceLoc, $"Global Variable {n.name} already declared");
                return null;
            }

            public override object VisitIdenExprNode(IdenExprNode n)
            {
                semanticChecker.CheckAndReport(varTypes.IsInScope(n.name) != null, n.sourceLoc, "Undelared Variable");
                return null;
            }

            public override object VisitIfNode(IfNode n)
            {
                Visit(n.test);
                semanticChecker.CheckAndReport(n.test.Type.Equals(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "Test not typeof bool");

                varTypes = varTypes.GoDown();
                Visit(n.body);
                varTypes = varTypes.GoUp();

                varTypes = varTypes.GoDown();
                Visit(n.elsebody);
                varTypes = varTypes.GoUp();

                return null;
            }

            public override object VisitListExprNode(ConstListExprNode n)
            {
                foreach (Expression e in n.ListValue)
                {
                    Visit(e);
                }
                
                semanticChecker.CheckAndReport(n.ListValue.All((e) => e.Type.Match(n.Type.ArrayType)), n.sourceLoc, "At least one element has a different Type of the others");
                return null;
            }

            public override object VisitProgNode(ProgNode n)
            {
                // Visit in order
                // GlobalVarDefs -> EnumDefs -> StructDefs -> FunDefs -> ?
                foreach(ASTNode c in n.children)
                {
                    if (c.kind == ASTKind.GlobalVarDef)
                        Visit(c);
                }
                foreach (ASTNode c in n.children)
                {
                    if (c.kind == ASTKind.EnumDef)
                        Visit(c);
                }
                foreach (ASTNode c in n.children)
                {
                    if (c.kind == ASTKind.StructDef)
                        Visit(c);
                }
                foreach (ASTNode c in n.children)
                {
                    if (c.kind == ASTKind.FunDef)
                        Visit(c);
                }

                return null;
            }

            public override object VisitReturnNode(ReturnNode n)
            {
                Visit(n.ret);
                if (!currentFunction.ReturnType.Equals(TypeSymbol.VOID_SYMBOL))
                    semanticChecker.CheckAndReport(currentFunction.ReturnType.Match(n.ret.Type), n.sourceLoc, "Returntype does not match return type of function");
                else
                    semanticChecker.CheckAndReport(n.ret == null, n.sourceLoc, "Returning value from void-function");
                return null;
            }

            public override object VisitUnaryExprNode(UnaryExprNode n)
            {
                switch (n.op)
                {
                    case "-":
                        {
                            semanticChecker.CheckAndReport(n.expr.Type.Match(TypeSymbol.INT_SYMBOL) || n.expr.Type.Match(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "Type mismatch");
                            semanticChecker.CheckAndReport(n.Type.Match(TypeSymbol.INT_SYMBOL) || n.Type.Match(TypeSymbol.FLOAT_SYMBOL), n.sourceLoc, "Type mismatch");
                            semanticChecker.CheckAndReport(n.Type.Match(n.expr.Type), n.sourceLoc, "Type mismatch");
                        }
                        break;
                    case "~":
                        {
                            semanticChecker.CheckAndReport(n.expr.Type.Match(TypeSymbol.INT_SYMBOL), n.sourceLoc, "Type mismatch");
                            semanticChecker.CheckAndReport(n.Type.Match(TypeSymbol.INT_SYMBOL), n.sourceLoc, "Type mismatch");
                        }
                        break;
                    case "!":
                        {
                            semanticChecker.CheckAndReport(n.expr.Type.Match(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "Type mismatch");
                            semanticChecker.CheckAndReport(n.Type.Match(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "Type mismatch");
                        }
                        break;
                }
                return null;
            }

            public override object VisitVarDeclNode(VarDeclNode n)
            {
                Visit(n.rhs);
                semanticChecker.CheckAndReport(varTypes.PutInScope(n.name, n.Type), n.sourceLoc, $"Variable {n.name} already declared");
                return null;
            }

            public override object VisitWhileNode(WhileNode n)
            {
                semanticChecker.CheckAndReport(n.test.Type.Equals(TypeSymbol.BOOL_SYMBOL), n.sourceLoc, "Test not typeof bool");
                varTypes = varTypes.GoDown();
                Visit(n.body);
                varTypes = varTypes.GoUp();
                return null;
            }
        }

        private class MainChecker : BaseASTVisitor<object>
        {
            public bool FoundMain;

            public override object VisitFunDefNode(FunDefNode n)
            {
                if (n.name == "main")// && n.rettype.Equals(TypeSymbol.VOID_SYMBOL))
                    FoundMain = true;
                return null;
            }
        }

        private class AllPathsReturnChecker : BaseASTVisitor<bool>
        {
            SemanticChecker semanticChecker;

            public AllPathsReturnChecker(SemanticChecker sc)
            {
                semanticChecker = sc;
            }

            public override bool VisitFunDefNode(FunDefNode n)
            {
                if(!n.rettype.Equals(TypeSymbol.VOID_SYMBOL))
                    semanticChecker.CheckAndReport(Visit(n.body), n.sourceLoc, "Not all paths return a value");
                return false;
            }

            public override bool VisitBlockNode(BlockNode n)
            {
                bool result = false;

                foreach (Statement s in n.stmts)
                    result |= Visit(s);

                return result;
            }

            public override bool VisitIfNode(IfNode n)
            {
                bool result = Visit(n.body);

                if (n.elsebody != null)
                    result &= Visit(n.elsebody);

                return result;
            }

            public override bool VisitReturnNode(ReturnNode n)
            {
                return true;
            }
        }
    }
}
