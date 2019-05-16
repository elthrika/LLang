using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    class MIPSCodeGenerator : CodeGenerator<MIPSRegister>
    {
        MIPSCodeEmitter emit;
        MIPSRegisterManager rm;

        Scope<int> vars;

        private bool lhsexpr;

        public MIPSCodeGenerator(string outfile)
        {
            emit = new MIPSCodeEmitter(outfile);
            rm = new MIPSRegisterManager();
            rm.InitRegisters();
            vars = new Scope<int>();
        }

        public MIPSRegister Gen(ASTNode n, bool lhsexpr)
        {
            this.lhsexpr = lhsexpr;
            return Visit(n);
        }

        public override MIPSRegister VisitArrayIndexNode(ArrayIndexNode n)
        {
            MIPSRegister array = Gen(n.array, true);
            MIPSRegister index = Gen(n.index, false);
            MIPSRegister reg = rm.GetRegister();

            int size = n.array.Type.ArrayType.Length;
            emit.Emit("li", reg, size);
            emit.Emit("mul", index, index, reg);
            emit.Emit("add", reg, array, index); //adress of element

            if (!lhsexpr) // it's a dereference, not assignment
                emit.EmitLoadStore("lw", reg, 0, reg);

            rm.ReleaseRegister(array);
            rm.ReleaseRegister(index);
            return reg;
        }

        public override MIPSRegister VisitAssignNode(AssignNode n)
        {
            MIPSRegister rhs = Gen(n.rhs, false);
            MIPSRegister lhs = Gen(n.lhs, true);

            emit.EmitLoadStore("sw", rhs, 0, lhs);

            rm.ReleaseRegister(lhs);
            rm.ReleaseRegister(rhs);
            return null;
        }

        public override MIPSRegister VisitAugAssignNode(AugAssignNode n)
        {
            MIPSRegister rhs = Gen(n.rhs, false);
            MIPSRegister lhs = Gen(n.lhs, true);
            MIPSRegister reg = rm.GetRegister();
            emit.EmitLoadStore("lw", reg, 0, lhs);

            switch (n.op)
            {
                case "*=":
                    {
                        emit.Emit("mul", rhs, reg, rhs);
                    } break;
                case "/=":
                    {
                        emit.Emit("div", rhs, reg, rhs);
                    }
                    break;
                case "%=":
                    {
                        emit.Emit("rem", rhs, reg, rhs);
                    } break;
                case "+=":
                    {
                        emit.Emit("add", rhs, reg, rhs);
                    } break;
                case "-=":
                    {
                        emit.Emit("sub", rhs, reg, rhs);
                    } break;
                case "<<=":
                    {
                        emit.Emit("sllv", rhs, reg, rhs);
                    } break;
                case ">>=":
                    {
                        emit.Emit("srav", rhs, reg, rhs);
                    } break;
                case "&=":
                    {
                        emit.Emit("and", rhs, reg, rhs);
                    } break;
                case "^=":
                    {
                        emit.Emit("xor", rhs, reg, rhs);
                    } break;
                case "|=":
                    {
                        emit.Emit("or", rhs, reg, rhs);
                    } break;
            }

            emit.EmitLoadStore("sw", rhs, 0, lhs); //Mem[lhs+0] op= rhs

            return null;
        }

        public override MIPSRegister VisitBinaryExprNode(BinaryExprNode n)
        {
            MIPSRegister lhs = Gen(n.lhs, false);
            MIPSRegister rhs = Gen(n.rhs, false);
            switch (n.op)
            {
                case "+":
                    {
                        emit.Emit("add", lhs, lhs, rhs);
                    }
                    break;
                case "-":
                    {
                        emit.Emit("sub", lhs, lhs, rhs);
                    }
                    break;
                case "*":
                    {
                        emit.Emit("mul", lhs, lhs, rhs);
                    }
                    break;
                case "%":
                    {
                        emit.Emit("rem", lhs, lhs, rhs);
                    }
                    break;
                case "/":
                    {
                        emit.Emit("div", lhs, lhs, rhs);
                    }
                    break;
                case "<<":
                    {
                        emit.Emit("sllv", lhs, lhs, rhs);
                    }
                    break;
                case ">>":
                    {
                        emit.Emit("sarv", lhs, lhs, rhs);
                    }
                    break;
                case "<":
                    {
                        emit.Emit("slt", lhs, lhs, rhs);
                    }
                    break;
                case ">":
                    {
                        emit.Emit("slt", lhs, lhs, rhs);
                    }
                    break;
                case "<=":
                    {

                    }
                    break;
                case ">=":
                    {
                        emit.Emit("slt", lhs, rhs, lhs); //(lhs >= rhs) <=> (rhs < lhs)
                    }
                    break;
                case "==":
                    {

                    }
                    break;
                case "!=":
                    {

                    }
                    break;
                case "&":
                    {

                    }
                    break;
                case "|":
                    {

                    }
                    break;
                case "^":
                    {

                    }
                    break;
                case "&&":
                    {

                    }
                    break;
                case "||":
                    {

                    }
                    break;
            }
            rm.ReleaseRegister(rhs);
            return lhs;
        }

        public override MIPSRegister VisitBlockNode(BlockNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitConstListExprNode(VarListExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitDeferedNode(DeferedNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitFieldAccessNode(FieldAccessNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitFloatExprNode(FloatExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitForNode(ForNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitFunCallExprNode(FunCallExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitFunCallStmtNode(FunCallStmtNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitFunDefNode(FunDefNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitGlobalVarDefNode(GlobalVarDefNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitIdenExprNode(IdenExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitIfNode(IfNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitIntExprNode(IntExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitLibImportNode(LibImportNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitListExprNode(ConstListExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitNewStructNode(NewStructNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitNullNode(NullNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitProgNode(ProgNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitReturnNode(ReturnNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitStringExprNode(StringExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitStructDefNode(StructDefNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitUnaryExprNode(UnaryExprNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitVarDeclNode(VarDeclNode n)
        {
            throw new NotImplementedException();
        }

        public override MIPSRegister VisitWhileNode(WhileNode n)
        {
            throw new NotImplementedException();
        }
    }
}
