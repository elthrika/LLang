using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class TypeMaker : BaseASTVisitor<object>
    {
        public static Scope<TypeSymbol> KnownTypes = new Scope<TypeSymbol>(); //future-proof-kinda ... in case types inside types should be allowed .........

        public Scope<TypeSymbol> varTypes = new Scope<TypeSymbol>();

        private Stack<ASTNode> NodeStack = new Stack<ASTNode>();

        public TypeMaker()
        {
            MakeIntrinsicTypes();
            MakeIntrinsicFunctionsAndVariables();
        }

        private void MakeIntrinsicTypes()
        {
            Dictionary<string, TypeSymbol> tmp = new Dictionary<string, TypeSymbol>()
            {
                { "int", TypeSymbol.INT_SYMBOL },
                { "float", TypeSymbol.FLOAT_SYMBOL },
                { "void", TypeSymbol.VOID_SYMBOL },
                { "bool", TypeSymbol.BOOL_SYMBOL },
                { "string", TypeSymbol.STRING_SYMBOL },
            };

            KnownTypes.PutAllInScope(tmp);
        }

        private void MakeIntrinsicFunctionsAndVariables()
        {
            foreach (var fun in Intrinsics.LIntrinsics)
            {
                varTypes.PutInScope(fun.Key, fun.Value.FunctionSymbol);
            }
        }

        public static TypeSymbol MakeTypeSymbolForString(string s)
        {
            TypeSymbol ts = KnownTypes.IsInScope(s);
            if (ts != null)
            {
                return ts;
            }
            if (s != null && s.StartsWith("[") && s.EndsWith("]"))
            {
                return TypeSymbol.ARRAY_SYMBOL(MakeTypeSymbolForString(s.Substring(1, s.Length - 2)));
            }
            return null;
        }

        public override object Visit(ASTNode n)
        {
            NodeStack.Push(n);
            var ret = n?.Accept(this);
            NodeStack.Pop();
            return ret;
        }

        public override object VisitProgNode(ProgNode n)
        {
            var globdefs   = new List<ASTNode>(n.children.Count);
            var enumdefs   = new List<ASTNode>(n.children.Count);
            var structdefs = new List<ASTNode>(n.children.Count);
            var fundefs    = new List<ASTNode>(n.children.Count);
            var rest       = new List<ASTNode>(n.children.Count);

            foreach (var c in n.children)
            {
                switch (c.kind)
                {
                    case ASTKind.GlobalVarDef:
                        globdefs.Add(c);
                        break;
                    case ASTKind.EnumDef:
                        enumdefs.Add(c);
                        break;
                    case ASTKind.StructDef:
                        structdefs.Add(c);
                        break;
                    case ASTKind.FunDef:
                        fundefs.Add(c);
                        break;
                    default:
                        rest.Add(c);
                        break;
                }
            }

            foreach (ASTNode c in globdefs)
            {
                Visit(c);
            }
            foreach (ASTNode c in enumdefs)
            {
                Visit(c);
            }
            foreach (ASTNode c in structdefs)
            {
                Visit(c);
            }
            foreach (ASTNode c in fundefs)
            {
                Visit(c);
            }
            foreach (ASTNode c in rest)
            {
                Visit(c);
            }

            return null;
        }

        public override object VisitArrayIndexNode(ArrayIndexNode n)
        {
            Visit(n.array);
            Visit(n.index);
            n.Type = n.array.Type.ArrayType;
            return null;
        }

        public override object VisitAssignNode(AssignNode n)
        {
            Visit(n.lhs);
            Visit(n.rhs);

            return null;
        }

        public override object VisitAugAssignNode(AugAssignNode n)
        {
            Visit(n.lhs);
            Visit(n.rhs);
            return null;
        }

        public override object VisitBinaryExprNode(BinaryExprNode n)
        {
            Visit(n.lhs);
            Visit(n.rhs);

            switch (n.op)
            {
                case "*":
                    {
                        if (n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL))
                            n.Type = TypeSymbol.FLOAT_SYMBOL;
                        else if (n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) && n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL))
                            n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case "/":
                    {
                        if (n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL))
                            n.Type = TypeSymbol.FLOAT_SYMBOL;
                        else if (n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) && n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL))
                            n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case "+":
                    {
                        if (n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL))
                            n.Type = TypeSymbol.FLOAT_SYMBOL;
                        else if (n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) && n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL))
                            n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case "-":
                    {
                        if (n.lhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL) || n.rhs.Type.Equals(TypeSymbol.FLOAT_SYMBOL))
                            n.Type = TypeSymbol.FLOAT_SYMBOL;
                        else if (n.lhs.Type.Equals(TypeSymbol.INT_SYMBOL) && n.rhs.Type.Equals(TypeSymbol.INT_SYMBOL))
                            n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case ">>":
                case "<<":
                case "&":
                case "|":
                case "^":
                case "%":
                    {
                        n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "==":
                case "!=":
                case "&&":
                case "||":
                    {
                        n.Type = TypeSymbol.BOOL_SYMBOL;
                    }
                    break;
            }
            return null;
        }

        public override object VisitVarListExprNode(VarListExprNode n)
        {
            Visit(n.lower);
            Visit(n.upper);
            Visit(n.stepsize);
            n.Type = TypeSymbol.ARRAY_SYMBOL(n.lower.Type);
            return null;
        }

        public override object VisitFieldAccessNode(FieldAccessNode n)
        {
            Visit(n.basestruct);
            if (n.basestruct.Type.Kind == TypeSymbol.TypeKind.POINTER)
                n.Type = MakeTypeSymbolForString(n.basestruct.Type.Name).Fields[n.fieldname].Item1;
            else if (n.basestruct.Type.Kind == TypeSymbol.TypeKind.STRUCT)
                n.Type = n.basestruct.Type.Fields[n.fieldname].Item1;
            else if (n.basestruct.Type.Kind == TypeSymbol.TypeKind.ENUM)
                n.Type = TypeSymbol.INT_SYMBOL;
            return null;
        }

        public override object VisitForNode(ForNode n)
        {
            Visit(n.inList);

            varTypes = varTypes.GoDown();
            varTypes.PutInScope(n.var, n.inList.Type.ArrayType);
            Visit(n.body);
            varTypes = varTypes.GoUp();

            return null;
        }

        private TypeSymbol DoInfer(TypeSymbol nametype, List<Expression> args)
        {
            if (nametype.Kind == TypeSymbol.TypeKind.INFER && nametype.inferOptions != null && nametype.inferOptions.Count > 0)
            {
                foreach (var ts in nametype.inferOptions)
                {
                    bool ok = true;
                    for (int i = 0; i < Math.Min(ts.ParameterTypes.Count, args.Count); i++)
                    {
                        ok &= args[i].Type.Match(ts.ParameterTypes[i]);
                    }
                    if (ok)
                    {
                        return ts;
                    }
                }
            }
            return nametype;
        }

        public override object VisitFunCallExprNode(FunCallExprNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.name.Type = DoInfer(n.name.Type, n.args);

            n.Type = n.name.Type.ReturnType;

            return null;
        }

        public override object VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.name.Type = DoInfer(n.name.Type, n.args);

            n.fullname = n.name.Type.Name + n.funname;
            n.funsig = varTypes.IsInScope(TypeSymbol.MakeFunctionName(n.fullname, n.args));
            n.Type = n.funsig.ReturnType;

            return null;
        }

        public override object VisitFunCallStmtNode(FunCallStmtNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.name.Type = DoInfer(n.name.Type, n.args);

            return null;
        }

        public override object VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.name.Type = DoInfer(n.name.Type, n.args);

            n.fullname = n.name.Type.Name + n.funname;
            n.funsig = varTypes.IsInScope(TypeSymbol.MakeFunctionName(n.fullname, n.args));

            return null;
        }

        public override object VisitFunDefNode(FunDefNode n)
        {
            n.rettype = MakeTypeSymbolForString(n.rettype.Name);
            for(int i = 0; i < n.argtypes.Count; i++)
            {
                n.argtypes[i] = MakeTypeSymbolForString(n.argtypes[i].Name);
            }
            string functionType = "";
            if (n.argtypes.Count > 0)
            {
                functionType = $"({n.argtypes.Select(t => t.Name).Aggregate((a, b) => $"{a},{b}")})->{n.rettype.Name}";
            }
            else
            {
                functionType = $"()->{n.rettype.Name}";
            }
            TypeSymbol funType = TypeSymbol.FUNCTION_SYMBOL(n.name, functionType, n.rettype, n.argtypes);
            n.funtype = funType;
            varTypes.PutInScope(funType.FunctionName, funType);

            varTypes = varTypes.GoDown();
            varTypes.PutAllInScope(n.args, n.argtypes);

            Visit(n.body);

            varTypes = varTypes.GoUp();

            return null;
        }

        public override object VisitGlobalVarDefNode(GlobalVarDefNode n)
        {
            TypeSymbol ts = MakeTypeSymbolForString(n.Type.Name);
            if(ts == null)
            {
                Visit(n.rhs); ts = n.rhs.Type;
            }

            n.Type = ts;

            varTypes.PutInScope(n.name, ts);
            return null;
        }

        public override object VisitIdenExprNode(IdenExprNode n)
        {
            n.Type = varTypes.IsInScope(n.name);

            if(n.Type == null)
            {
                var funmatches = varTypes.GetPredicateMatches((s, ts) => ts.FunctionName != null && ts.FunctionName.StartsWith(n.name+"`"));
                if(funmatches.Count == 1)
                {
                    n.Type = funmatches.ElementAt(0);
                }
                else if(funmatches.Count > 1)
                {
                    n.Type = TypeSymbol.INFER_FROM_SYMBOL(n.name, funmatches);
                }
            }
            
            return null;
        }

        public override object VisitIfNode(IfNode n)
        {
            Visit(n.test);

            varTypes = varTypes.GoDown();
            Visit(n.ifbody);
            varTypes = varTypes.GoUp();

            varTypes = varTypes.GoDown();
            Visit(n.elsebody);
            varTypes = varTypes.GoUp();

            return null;
        }

        public override object VisitConstListExprNode(ConstListExprNode n)
        {
            if (n.ListValue.Count > 0)
            {
                TypeSymbol arrType = null;
                foreach (Expression e in n.ListValue)
                {
                    Visit(e);
                    var et = e.Type;
                    if (arrType == null)
                        arrType = et;
                    else
                    {
                        if(et.Kind == TypeSymbol.TypeKind.INFER && et.inferOptions != null && et.inferOptions.Count > 0)
                        {
                            foreach (var ts in et.inferOptions)
                            {
                                if (ts.Match(arrType))
                                {
                                    e.Type = ts;
                                    break;
                                }    
                            }
                        }
                    }
                }
                n.Type = TypeSymbol.ARRAY_SYMBOL(arrType);
            }
            else
            {
                n.Type = TypeSymbol.INFER_SYMOBOL("[]");
            }
            return null;
        }

        public override object VisitNewStructNode(NewStructNode n)
        {
            n.Type = MakeTypeSymbolForString(n.Type.Name);
            return null;
        }

        public override object VisitReturnNode(ReturnNode n)
        {
            Visit(n.ret);
            return null;
        }

        public override object VisitStructDefNode(StructDefNode n)
        {
            foreach (Expression e in n.defaultValues)
                Visit(e);

            var fields = new Dictionary<string, (TypeSymbol, int)>();

            int c_offset = 0;
            for(int i = 0; i < n.Type.Fields.Count; i++)
            {
                KeyValuePair<string, (TypeSymbol, int)> m = n.Type.Fields.ElementAt(i);
                TypeSymbol curts = m.Value.Item1;

                string name = m.Key;

                TypeSymbol toadd = null;
                if (curts.Name == n.name || MakeTypeSymbolForString(curts.Name).Kind == TypeSymbol.TypeKind.STRUCT) //pointer to (own) struct
                {
                    toadd = TypeSymbol.POINTER_SYMBOL(curts); //new TypeSymbol(curts.Name, TypeSymbol.TypeKind.POINTER, 8);
                }
                else
                {
                    toadd = MakeTypeSymbolForString(curts.Name);
                }
                int len = toadd.Length;

                if (c_offset > 0 && c_offset % len != 0)
                {
                    c_offset = ((c_offset / len) + 1) * len; // get next biggest multiple of Size -> alignment
                }

                fields.Add(name, (toadd, c_offset));

                c_offset += len;
            }

            TypeSymbol ts = TypeSymbol.STRUCT_SYMBOL(n.name, c_offset, fields);
            n.Type = ts;

            KnownTypes.PutInScope(n.name, ts);

            return null;
        }

        public override object VisitEnumDefNode(EnumDefNode n)
        {
            varTypes.PutInScope(n.enumname, n.Type);

            return null;
        }

        public override object VisitUnaryExprNode(UnaryExprNode n)
        {
            Visit(n.expr);
            switch (n.op)
            {
                case "-":
                    {
                        n.Type = n.expr.Type.Equals(TypeSymbol.FLOAT_SYMBOL) ? TypeSymbol.FLOAT_SYMBOL : TypeSymbol.INT_SYMBOL;
                    }
                    break;
                case "!":
                    {
                        n.Type = TypeSymbol.BOOL_SYMBOL;
                    }
                    break;
                case "~":
                    {
                        n.Type = TypeSymbol.INT_SYMBOL;
                    }
                    break;
            }
            return null;
        }

        public override object VisitVarDeclNode(VarDeclNode n)
        {
            TypeSymbol ts = MakeTypeSymbolForString(n.Type.Name);
            Visit(n.rhs);
            if (ts == null)
            {
                ts = n.rhs.Type;
            }

            n.Type = ts;

            varTypes.PutInScope(n.name, ts);
            return null;
        }

        public override object VisitWhileNode(WhileNode n)
        {
            Visit(n.test);

            varTypes = varTypes.GoDown();
            Visit(n.body);
            varTypes = varTypes.GoUp();
            
            return null;
        }
    }
}
