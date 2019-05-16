using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    public interface IASTVisitor<out Result>
    {
        Result Visit(ASTNode n);
        Result VisitProgNode(ProgNode n);
        Result VisitFunDefNode(FunDefNode n);
        Result VisitGlobalVarDefNode(GlobalVarDefNode n);
        Result VisitStructDefNode(StructDefNode n);
        Result VisitLibImportNode(LibImportNode n);
        Result VisitVarDeclNode(VarDeclNode n);
        Result VisitAssignNode(AssignNode n);
        Result VisitAugAssignNode(AugAssignNode n);
        Result VisitFunCallStmtNode(FunCallStmtNode n);
        Result VisitWhileNode(WhileNode n);
        Result VisitForNode(ForNode n);
        Result VisitIfNode(IfNode n);
        Result VisitReturnNode(ReturnNode n);
        Result VisitBlockNode(BlockNode n);
        Result VisitDeferedNode(DeferedNode n);
        Result VisitIdenExprNode(IdenExprNode n);
        Result VisitIntExprNode(IntExprNode n);
        Result VisitFloatExprNode(FloatExprNode n);
        Result VisitStringExprNode(StringExprNode n);
        Result VisitListExprNode(ConstListExprNode n);
        Result VisitUnaryExprNode(UnaryExprNode n);
        Result VisitBinaryExprNode(BinaryExprNode n);
        Result VisitFunCallExprNode(FunCallExprNode n);
        Result VisitImplicitFunCallExprNode(ImplicitFunCallExprNode n);
        Result VisitImplicitFunCallStmtNode(ImplicitFunCallStmtNode n);
        Result VisitConstListExprNode(VarListExprNode n);
        Result VisitArrayIndexNode(ArrayIndexNode n);
        Result VisitFieldAccessNode(FieldAccessNode n);
        Result VisitNewStructNode(NewStructNode n);
        Result VisitNullNode(NullNode n);
    }
}
