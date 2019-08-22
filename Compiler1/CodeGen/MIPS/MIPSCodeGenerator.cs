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

        Scope<Either<MIPSRegister, short>> vars;

        public MIPSCodeGenerator(string outfile)
        {
            emit = new MIPSCodeEmitter(outfile);
            rm = new MIPSRegisterManager();
            rm.InitRegisters();
            vars = new Scope<Either<MIPSRegister, short>>();
        }

        public MIPSRegister Gen(ASTNode n)
        {
            return Visit(n);
        }

        public override MIPSRegister VisitArrayIndexNode(ArrayIndexNode n)
        {
            return base.VisitArrayIndexNode(n);
        }

        public override MIPSRegister VisitAssignNode(AssignNode n)
        {
            return base.VisitAssignNode(n);
        }

        public override MIPSRegister VisitAugAssignNode(AugAssignNode n)
        {
            var lhs = Gen(n.lhs);
            var rhs = Gen(n.rhs);

            switch (n.op)
            {
                case "*=":
                    {
                        emit.Emit("mul", lhs, lhs, rhs);
                    }
                    break;
                case "/=":
                    {
                        emit.Emit("quo", lhs, lhs, rhs);
                    }
                    break;
                case "%=":
                    {
                        emit.Emit("rem", lhs, lhs, rhs);
                    }
                    break;
                case "+=":
                    {
                        emit.Emit("add", lhs, lhs, rhs);
                    }
                    break;
                case "-=":
                    {
                        emit.Emit("sub", lhs, lhs, rhs);
                    }
                    break;
                case "<<=":
                    {
                        emit.Emit("sllv", lhs, lhs, rhs);
                    }
                    break;
                case ">>=":
                    {
                        emit.Emit("srav", lhs, lhs, rhs);
                    }
                    break;
                case "&=":
                    {
                        emit.Emit("and", lhs, lhs, rhs);
                    }
                    break;
                case "^=":
                    {
                        emit.Emit("xor", lhs, lhs, rhs);
                    }
                    break;
                case "|=":
                    {
                        emit.Emit("or", lhs, lhs, rhs);
                    }
                    break;

            }

            return null;
        }

        public override MIPSRegister VisitBinaryExprNode(BinaryExprNode n)
        {
            var lhs = Gen(n.lhs); 
            var rhs = Gen(n.rhs);

            switch (n.op)
            {
                case "*":
                    {
                        emit.Emit("mul", lhs, lhs, rhs);
                    }
                    break;
                case "/":
                    {
                        emit.Emit("rem", lhs, lhs, rhs);
                    }
                    break;
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
                case ">>":
                    {
                        emit.Emit("srav", lhs, lhs, rhs);
                    }
                    break;
                case "<<":
                    {
                        emit.Emit("sllv", lhs, lhs, rhs);
                    }
                    break;
                case "&":
                    {
                        emit.Emit("and", lhs, lhs, rhs);
                    }
                    break;
                case "|":
                    {
                        emit.Emit("or", lhs, lhs, rhs);
                    }
                    break;
                case "^":
                    {
                        emit.Emit("xor", lhs, lhs, rhs);
                    }
                    break;
                case "%":
                    {
                        emit.Emit("rem", lhs, lhs, rhs);
                    }
                    break;
                case "<":
                    {
                        emit.Emit("slt", lhs, lhs, rhs);
                    }
                    break;
                case ">":
                    {
                        emit.Emit("slt", lhs, rhs, lhs);
                    }
                    break;
                case "<=":
                    {
                        emit.Emit("slt", lhs, rhs, lhs);
                        emit.Emit("nor", lhs, lhs, lhs);
                    }
                    break;
                case ">=":
                    {
                        emit.Emit("slt", lhs, lhs, rhs);
                        emit.Emit("nor", lhs, lhs, lhs);
                    }
                    break;
                case "==":
                    {
                        emit.Emit("slt", rm.AT, lhs, rhs);
                        emit.Emit("slt", lhs, rhs, lhs);
                        emit.Emit("nor", lhs, rm.AT, lhs);
                    }
                    break;
                case "!=":
                    {
                        emit.Emit("slt", rm.AT, lhs, rhs);
                        emit.Emit("slt", lhs, rhs, lhs);
                        emit.Emit("or", lhs, rm.AT, lhs);
                    }
                    break;
                case "&&":
                    {
                        emit.Emit("and", lhs, lhs, rhs);
                    }
                    break;
                case "||":
                    {
                        emit.Emit("or", lhs, lhs, rhs);
                    }
                    break;
                default:
                    throw new Exception($"Did not match the binop: {n.op} at {n.sourceLoc}");
            }

            rm.ReleaseRegister(rhs);

            return lhs;
        }

        public override MIPSRegister VisitBlockNode(BlockNode n)
        {
            return base.VisitBlockNode(n);
        }

        public override MIPSRegister VisitVarListExprNode(VarListExprNode n)
        {
            var reg = GetMIPSRegister();
            

            return reg;
        }

        public override MIPSRegister VisitDeferedNode(DeferedNode n)
        {
            return base.VisitDeferedNode(n);
        }

        public override MIPSRegister VisitEnumDefNode(EnumDefNode n)
        {
            return base.VisitEnumDefNode(n);
        }

        public override MIPSRegister VisitFieldAccessNode(FieldAccessNode n)
        {
            var basestruct = Gen(n.basestruct);

            var (ftype, foffset) = n.basestruct.Type.Fields[n.fieldname];
            emit.EmitLoadStore("lw", basestruct, (short)foffset, basestruct);

            return basestruct;
        }

        public override MIPSRegister VisitFloatExprNode(FloatExprNode n)
        {
            return base.VisitFloatExprNode(n);
        }

        public override MIPSRegister VisitForNode(ForNode n)
        {
            var testlbl = emit.GetUniqueLabel();
            var bodylbl = emit.GetUniqueLabel();
            var incrlbl = emit.GetUniqueLabel();
            var endlbl = emit.GetUniqueLabel();

            /*
                Gen(inlist)
                it <- inlist[1]
                idx <- 0
            test:
                len <- inlist[0]
                slt tmp, idx, len
                blez tmp, end
            body:
                Gen(body)
            incr:
                idx <- idx + 1
                it <- inlist[idx]
                j test
            end:
             */
            var list = Gen(n.inList);
            var it = GetMIPSRegister();
            var idx = GetMIPSRegister();

            vars = vars.GoDown();
            vars.PutInScope(n.var, new Either<MIPSRegister, short>(it));

            emit.EmitImmediate("addi", idx.Name, list.Name, 4);
            emit.EmitLoadStore("lw", it, 0, idx);
            emit.EmitLabel(testlbl);
            var tmp = GetMIPSRegister();
            emit.EmitLoadStore("lw", tmp, 0, list);
            emit.Emit("slt", tmp, idx, tmp);
            emit.EmitLabelJump("blez", tmp, endlbl);
            emit.EmitLabel(bodylbl);
            Gen(n.body);
            emit.EmitLabel(incrlbl);
            emit.EmitImmediate("addi", idx.Name, 4);
            emit.EmitLoadStore("lw", it, 0, idx);
            emit.EmitJump("j", testlbl);
            emit.EmitLabel(endlbl);

            vars = vars.GoUp();

            return null;
        }

        public override MIPSRegister VisitFunCallExprNode(FunCallExprNode n)
        {
            return base.VisitFunCallExprNode(n);
        }

        public override MIPSRegister VisitFunCallStmtNode(FunCallStmtNode n)
        {
            return base.VisitFunCallStmtNode(n);
        }

        public override MIPSRegister VisitFunDefNode(FunDefNode n)
        {
            // @TODO
            emit.EmitLabel(n.name);

            int n_vars = new VarsInFunDefCounter().Visit(n);
            /*
             allocate n_vars*4 bytes on the stack for local variables
             store s0-s7
             store sp, fp, ra
             */
            short offset = (short)(n_vars * 4);
            offset -= 4;
            emit.EmitLoadStore("sw", rm.SP, offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", rm.FP, offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", rm.RA, offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s0", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s1", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s2", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s3", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s4", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s5", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s6", offset, rm.SP);
            offset -= 4;
            emit.EmitLoadStore("sw", "$s7", offset, rm.SP);
            emit.EmitImmediate("addi", rm.SP, rm.SP, offset);


            Visit(n.body);

            /*
             restore s0-s7
             restore sp, fp, ra
             */
            offset = 0;
            emit.EmitLoadStore("lw", "$s7", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s6", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s5", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s4", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s3", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s2", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s1", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", "$s0", offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", rm.RA, offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", rm.FP, offset, rm.SP);
            offset += 4;
            emit.EmitLoadStore("lw", rm.SP, offset, rm.SP);
            
            if (n.rettype == TypeSymbol.VOID_SYMBOL)
                emit.Emit("ret");

            return null;
        }

        public override MIPSRegister VisitGlobalVarDefNode(GlobalVarDefNode n)
        {
            return base.VisitGlobalVarDefNode(n);
        }

        public override MIPSRegister VisitIdenExprNode(IdenExprNode n)
        {
            var iden = vars.IsInScope(n.name);
            if (iden.FirstSet)
                return iden.First;
            var reg = GetMIPSRegister();
            emit.EmitLoadStore("lw", reg, iden.Second, rm.SP);
            return reg;
        }

        public override MIPSRegister VisitIfNode(IfNode n)
        {
            string elselbl = emit.GetUniqueLabel();
            string iflbl = emit.GetUniqueLabel();
            string endlbl = emit.GetUniqueLabel();

            /*
                reg <- test
                bgtz reg, iflbl
            elselbl:
                Gen(elsebody)
                j endlbl
            iflbl:
                Gen(ifbody)
            end:
             */

            var reg = Gen(n.test);
            emit.EmitLabelJump("bgtz", reg, iflbl);
            emit.EmitLabel(elselbl);
            vars = vars.GoDown();
            Gen(n.elsebody);
            vars = vars.GoUp();
            emit.EmitJump("j", endlbl);
            emit.EmitLabel(iflbl);
            vars = vars.GoDown();
            Gen(n.ifbody);
            vars = vars.GoUp();
            emit.EmitLabel(endlbl);
            return null;
        }

        public override MIPSRegister VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n)
        {
            return base.VisitImplicitFunCallExprNode(n);
        }

        public override MIPSRegister VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n)
        {
            

            return base.VisitImplicitFunCallStmtNode(n);
        }

        public override MIPSRegister VisitIntExprNode(IntExprNode n)
        {
            return base.VisitIntExprNode(n);
        }

        public override MIPSRegister VisitLibImportNode(LibImportNode n)
        {
            return base.VisitLibImportNode(n);
        }

        public override MIPSRegister VisitConstListExprNode(ConstListExprNode n)
        {
            var reg = GetMIPSRegister();
            emit.EmitMalloc(reg, (short)(n.ListValue.Count + 1));
            for (int i = 0; i < n.ListValue.Count; i++)
            {
                var idx = Gen(n.ListValue[i]);
                emit.EmitLoadStore("sw", idx, (short)((i + 1) * 4), reg);
                rm.ReleaseRegister(idx);
            }
            return reg;
        }

        public override MIPSRegister VisitNewStructNode(NewStructNode n)
        {
            var reg = GetMIPSRegister();
            emit.EmitMalloc(reg, (short)n.Type.Length);
            return reg;
        }

        public override MIPSRegister VisitNullNode(NullNode n)
        {
            var reg = GetMIPSRegister();
            emit.Emit("xor", reg, reg, reg);
            return reg;
        }

        public override MIPSRegister VisitProgNode(ProgNode n)
        {
            return base.VisitProgNode(n);
        }

        public override MIPSRegister VisitReturnNode(ReturnNode n)
        {
            emit.Emit("ret");
            return null;
        }

        public override MIPSRegister VisitStringExprNode(StringExprNode n)
        {
            //TODO String constants have to go in the data segment
            return base.VisitStringExprNode(n);
        }

        public override MIPSRegister VisitStructDefNode(StructDefNode n)
        {
            return base.VisitStructDefNode(n);
        }

        public override MIPSRegister VisitUnaryExprNode(UnaryExprNode n)
        {
            var reg = Gen(n.expr);

            switch (n.op)
            {
                case "-":
                    {
                        emit.Emit("sub", reg, rm.ZERO, reg);
                    }
                    break;
                case "~":
                case "!":
                    {
                        emit.Emit("nor", reg, reg, reg);
                    }
                    break;
            }

            return reg;
        }

        public override MIPSRegister VisitVarDeclNode(VarDeclNode n)
        {
            var varloc = vars.IsInScope(n.name);
            if(n.rhs != null)
            {
                var rhs = Gen(n.rhs);
                if (varloc.SecondSet)
                {
                    // store the value to the stack
                    emit.EmitLoadStore("sw", rhs, varloc.Second, rm.FP);
                }
                else
                {
                    // store the value in the register
                    emit.Emit("add", varloc.First, rhs, rm.ZERO);
                }
            }

            return null;
        }

        public override MIPSRegister VisitWhileNode(WhileNode n)
        {
            return base.VisitWhileNode(n);
        }

        private MIPSRegister GetMIPSRegister()
        {
            try
            {
                return rm.GetRegister();
            }
            catch(Exception e)
            {
                /*
                string rmv = "";
                short address = 0;
                MIPSRegister found = null;

                foreach (var (key, item) in vars)
                {
                    if(item.FirstSet)
                    {
                        address = currentOffsets[key];
                        rmv = key;
                        found = item.First;
                        emit.EmitLoadStore("sw", found, address, rm.SP);
                        break;
                    }
                }
                if(found != null)
                {
                    vars.Remove(rmv);
                    vars.PutInScope(rmv, new Either<MIPSRegister, short>(address));
                    return found;
                }
                */

                /*
                    Here we want to get a register (t0-t9), put it's value on the stack, release the register and remember that we put the value there ...
                    but this is probably better done by whoever needs the register, since they can restore the value after more easily

                    the problem should only ever be a real problem in big trees of binops, since vars have a place on the stack to store them,
                    and only intermediate results need to be saved dynamically
                 */
                throw e;
            }
        }
    }
}
