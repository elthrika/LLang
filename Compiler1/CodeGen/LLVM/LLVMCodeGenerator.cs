using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class LLVMCodeGenerator : CodeGenerator<LLVMRegister>
    {
        LLVMCodeEmitter emit;
        Scope<LLVMRegister> nameToReg;
        Dictionary<TypeSymbol, string> typeToLLVMName;

        public LLVMCodeGenerator(string outfile)
        {
            emit = new LLVMCodeEmitter(outfile);
            nameToReg = new Scope<LLVMRegister>();
            typeToLLVMName = new Dictionary<TypeSymbol, string>()
            {
                { TypeSymbol.INT_SYMBOL, "i64" },
                { TypeSymbol.FLOAT_SYMBOL, "double" },
                { TypeSymbol.VOID_SYMBOL, "void" },
                { TypeSymbol.BOOL_SYMBOL, "i8" },
                { TypeSymbol.STRING_SYMBOL, "{ i64, i8* }" },
            };
        }

        public override LLVMRegister VisitArrayIndexNode(ArrayIndexNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitAssignNode(AssignNode n)
        {
            LLVMRegister lhs = Visit(n.lhs);
            LLVMRegister rhs = Visit(n.rhs);

            return null;
        }

        public override LLVMRegister VisitAugAssignNode(AugAssignNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitBinaryExprNode(BinaryExprNode n)
        {
            LLVMRegister result = emit.GetTempRegister();
            LLVMRegister lhs = Visit(n.lhs);
            LLVMRegister rhs = Visit(n.rhs);

            bool isfloat = n.Type.Equals(TypeSymbol.FLOAT_SYMBOL);
            string type = typeToLLVMName[n.Type];
            string op = "";
            string cmpop = "";

            switch (n.op)
            {
                case "*":
                    {
                        op = isfloat ? "mul" : "mulf";
                    }
                    break;
                case "/":
                    {
                        op = isfloat ? "div" : "divf";
                    }
                    break;
                case "%":
                    {
                        op = isfloat ? "rem" : "remf"; ;
                    }
                    break;
                case "+":
                    {
                        op = isfloat ? "add" : "addf";
                    }
                    break;
                case "-":
                    {
                        op = isfloat ? "sub" : "subf";
                    }
                    break;
                case ">>":
                    {
                        op = "ashr";
                    }
                    break;
                case "<<":
                    {
                        op = "shl";
                    }
                    break;
                case "<":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "olt" : "slt";
                    }
                    break;
                case ">":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "ogt" : "sgt";
                    }
                    break;
                case "<=":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "ole" : "sle";
                    }
                    break;
                case ">=":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "oge" : "sge";
                    }
                    break;
                case "==":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "oeq" : "eq";
                    }
                    break;
                case "!=":
                    {
                        op = isfloat ? "fcmp" : "icmp";
                        cmpop = isfloat ? "one" : "ne";
                    }
                    break;
                case "&":
                    {
                        op = "and";
                    }
                    break;
                case "|":
                    {
                        op = "or";
                    }
                    break;
                case "^":
                    {
                        op = "xor";
                    }
                    break;
                case "&&":
                    {
                        op = "and";
                    }
                    break;
                case "||":
                    {
                        op = "or";
                    }
                    break;
            }

            return result;
        }

        public override LLVMRegister VisitBlockNode(BlockNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitConstListExprNode(VarListExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitDeferedNode(DeferedNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitFieldAccessNode(FieldAccessNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitFloatExprNode(FloatExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitForNode(ForNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitFunCallExprNode(FunCallExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitFunCallStmtNode(FunCallStmtNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitFunDefNode(FunDefNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitGlobalVarDefNode(GlobalVarDefNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitIdenExprNode(IdenExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitIfNode(IfNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitIntExprNode(IntExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitLibImportNode(LibImportNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitListExprNode(ConstListExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitNewStructNode(NewStructNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitNullNode(NullNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitProgNode(ProgNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitReturnNode(ReturnNode n)
        {
            LLVMRegister retval = Visit(n.ret);
            emit.Emit("ret", retval);

            return null;
        }

        public override LLVMRegister VisitStringExprNode(StringExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitStructDefNode(StructDefNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitUnaryExprNode(UnaryExprNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitVarDeclNode(VarDeclNode n)
        {
            throw new NotImplementedException();
        }

        public override LLVMRegister VisitWhileNode(WhileNode n)
        {
            throw new NotImplementedException();
        }
    }
}
