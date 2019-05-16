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

        public TypeMaker()
        {
            Dictionary<string, TypeSymbol> tmp = new Dictionary<string, TypeSymbol>()
            {
                { "int", TypeSymbol.INT_SYMBOL },
                { "float", TypeSymbol.FLOAT_SYMBOL },
                { "void", TypeSymbol.VOID_SYMBOL },
                { "bool", TypeSymbol.BOOL_SYMBOL },
                { "string", TypeSymbol.STRING_SYMBOL },
            };

            KnownTypes.PutAllInScope(tmp.Keys, tmp.Values);
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
            return n?.Accept(this);
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

        public override object VisitConstListExprNode(VarListExprNode n)
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
                n.Type = MakeTypeSymbolForString(n.basestruct.Type.Name).Fields[n.fieldname];
            else if(n.basestruct.Type.Kind == TypeSymbol.TypeKind.STRUCT)
                n.Type = n.basestruct.Type.Fields[n.fieldname];
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

        public override object VisitFunCallExprNode(FunCallExprNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.Type = n.name.Type.ReturnType;
            return null;
        }

        public override object VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);

            n.fullname = n.name.Type.Name + n.funname;
            n.funsig = varTypes.IsInScope(n.fullname);
            n.Type = n.funsig.ReturnType;

            return null;
        }

        public override object VisitFunCallStmtNode(FunCallStmtNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            return null;
        }

        public override object VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            Visit(n.name);
            foreach (Expression e in n.args)
                Visit(e);
            n.fullname = n.name.Type.Name + n.funname;
            n.funsig = varTypes.IsInScope(n.fullname);
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
            TypeSymbol funType = TypeSymbol.FUNCTION_SYMBOL(functionType, n.rettype, n.argtypes);
            n.funtype = funType;
            varTypes.PutInScope(n.name, funType);

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
            return null;
        }

        public override object VisitIfNode(IfNode n)
        {
            Visit(n.test);

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
            if (n.ListValue.Count > 0)
            {
                foreach (Expression e in n.ListValue)
                    Visit(e);
                n.Type = TypeSymbol.ARRAY_SYMBOL(n.ListValue[0].Type);
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

            Dictionary<string, TypeSymbol> fields = new Dictionary<string, TypeSymbol>();
            List<int> offsets = new List<int>();

            int c_offset = 0;
            for(int i = 0; i < n.Type.Fields.Count; i++)
            {
                KeyValuePair<string, TypeSymbol> m = n.Type.Fields.ElementAt(i);
                TypeSymbol curts = m.Value; string name = m.Key;


                if (curts.Name == n.name || MakeTypeSymbolForString(curts.Name).Kind == TypeSymbol.TypeKind.STRUCT) //pointer to (own) struct
                {
                    fields.Add(name, new TypeSymbol(curts.Name, TypeSymbol.TypeKind.POINTER, 8));
                }
                else
                {
                    fields.Add(name, MakeTypeSymbolForString(curts.Name));
                }

                int len = fields[name].Length;

                if (c_offset > 0 && c_offset % len != 0)
                {
                    c_offset = ((c_offset / len) + 1) * len; // get next biggest multiple of Size -> alignment
                }

                offsets.Add(c_offset);

                c_offset += len;
            }

            TypeSymbol ts = TypeSymbol.STRUCT_SYMBOL(n.name, c_offset, fields, offsets);
            n.Type = ts;

            KnownTypes.PutInScope(n.name, ts);

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
            if (ts == null)
            {
                Visit(n.rhs); ts = n.rhs.Type;
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
