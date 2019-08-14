using System;
using System.Collections.Generic;

namespace Compiler1
{
    public enum ASTKind
    {
        Prog,
        FunDef,
        GlobalVarDef,
        StructDef,
        LibImport,
        EnumDef,
        VarDecl,
        Assign,
        AugAssign,
        FunCallStmt,
        FunCallStmtImpl,
        While,
        For,
        If,
        Return,
        Block,
        IdenExpr,
        IntExpr,
        FloatExpr,
        StringExpr,
        ListExpr,
        UnaryExpr,
        BinaryExpr,
        FunCallExpr,
        FunCallExprImpl,
        ConstListExpr,
        ArrayIndex,
        FieldAccess,
        NewStruct,
        Defer,
        Null,
    }

    public struct SourceLoc
    {
        public readonly int Col;
        public readonly int Row;
        //string file;
        public SourceLoc(int r, int c)
        {
            Col = c; Row = r;
        }

        public override string ToString()
        {
            return $"[{Row}, {Col}]";
        }

        public override bool Equals(object obj)
        {
            if(obj is SourceLoc || obj is string)
            {
                return obj.ToString() == ToString();
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 973646036;
            hashCode = hashCode * -1521134295 + Col.GetHashCode();
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            return hashCode;
        }
    }

    public abstract class ASTNode
    {

        public ASTKind kind;
        public SourceLoc sourceLoc;
        public bool isExport = false;

        public ASTNode(ASTKind kind, SourceLoc sl)
        {
            this.kind = kind;
            this.sourceLoc = sl;
        }

        public abstract T Accept<T>(IASTVisitor<T> visitor);
    }

    public abstract class Statement : ASTNode
    {
        public Statement(ASTKind kind, SourceLoc sl) : base(kind, sl)
        {
        }
    }

    public abstract class Expression : ASTNode
    {
        public TypeSymbol Type;
        public bool IsConstant = false;

        public long IntValue;
        public double FloatValue;
        public string StringValue;
        public List<Expression> ListValue;

        public Expression(ASTKind kind, TypeSymbol type, SourceLoc sl) : base(kind, sl)
        {
            this.Type = type;
        }
    }

    public class ProgNode : Statement
    {

        public List<ASTNode> children;

        public ProgNode(List<ASTNode> children, SourceLoc sl) : base(ASTKind.Prog, sl)
        {
            this.children = children;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitProgNode(this);
        }
    }

    public class FunDefNode : Statement
    {

        public string name;
        public List<string> args;
        public List<TypeSymbol> argtypes;
        public TypeSymbol rettype;
        public TypeSymbol funtype;
        public Statement body;

        public FunDefNode(string name, List<string> args, List<TypeSymbol> argtypes, TypeSymbol rettype, Statement body, SourceLoc sl) : base(ASTKind.FunDef, sl)
        {
            this.name = name;
            this.args = args;
            this.body = body;
            this.argtypes = argtypes;
            this.rettype = rettype;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFunDefNode(this);
        }
    }

    public class StructDefNode : Statement
    {
        public string name;
        public TypeSymbol Type;
        public List<Expression> defaultValues;

        public StructDefNode(string name, TypeSymbol ownType, List<Expression> defaultValues, SourceLoc sl) : base(ASTKind.StructDef, sl)
        {
            this.name = name;
            this.Type = ownType;
            this.defaultValues = defaultValues;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStructDefNode(this);
        }
    }

    public class LibImportNode : Statement
    {
        public string libname;

        public LibImportNode(string name, SourceLoc sl) : base(ASTKind.LibImport, sl)
        {
            this.libname = name;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitLibImportNode(this);
        }
    }

    public class GlobalVarDefNode : Statement
    {

        public string name;
        public Expression rhs;
        public TypeSymbol Type;

        public GlobalVarDefNode(string name, Expression rhs, TypeSymbol type, SourceLoc sl) : base(ASTKind.GlobalVarDef, sl)
        {
            this.name = name;
            this.rhs = rhs;
            this.Type = type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitGlobalVarDefNode(this);
        }
    }

    public class VarDeclNode : Statement
    {
        public string name;
        public Expression rhs;
        public TypeSymbol Type;

        public VarDeclNode(string name, Expression rhs, TypeSymbol type, SourceLoc sl) : base(ASTKind.VarDecl, sl)
        {
            this.name = name;
            this.rhs = rhs;
            this.Type = type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitVarDeclNode(this);
        }
    }

    public class AssignNode : Statement
    {
        public Expression lhs;
        public Expression rhs;

        public AssignNode(Expression lhs, Expression rhs, SourceLoc sl) : base(ASTKind.Assign, sl)
        {
            this.rhs = rhs;
            this.lhs = lhs;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitAssignNode(this);
        }
    }

    public class AugAssignNode : Statement
    {
        public Expression lhs;
        public Expression rhs;
        public string op;

        public AugAssignNode(Expression lhs, Expression rhs, string op, SourceLoc sl) : base(ASTKind.AugAssign, sl)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitAugAssignNode(this);
        }
    }

    public class FunCallStmtNode : Statement
    {
        public Expression name;
        public List<Expression> args;
        public TypeSymbol funsig;

        public FunCallStmtNode(Expression name, List<Expression> args, SourceLoc sl) : base(ASTKind.FunCallStmt, sl)
        {
            this.name = name;
            this.funsig = this.name.Type;
            this.args = args;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFunCallStmtNode(this);
        }
    }

    public class ImplicitFunCallStmtNode : FunCallStmtNode
    {
        public string funname;
        public string fullname;

        public ImplicitFunCallStmtNode(string funname, Expression name, List<Expression> args, SourceLoc sl) : base(name, args, sl)
        {
            this.kind = ASTKind.FunCallStmtImpl;
            this.funname = funname;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitImplicitFunCallStmtNode(this);
        }
    }

    public class WhileNode : Statement
    {
        public Expression test;
        public Statement body;

        public WhileNode(Expression test, Statement body, SourceLoc sl) : base(ASTKind.While, sl)
        {
            this.test = test;
            this.body = body;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitWhileNode(this);
        }
    }

    public class ForNode : Statement
    {
        public string var;
        public Expression inList;
        public Statement body;

        public ForNode(string var, Expression inlist, Statement body, SourceLoc sl) : base(ASTKind.For, sl)
        {
            this.inList = inlist;
            this.var = var;
            this.body = body;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitForNode(this);
        }
    }

    public class IfNode : Statement
    {
        public Expression test;
        public Statement body;
        public Statement elsebody;

        public IfNode(Expression test, Statement body, Statement elsebody, SourceLoc sl) : base(ASTKind.If, sl)
        {
            this.test = test;
            this.body = body;
            this.elsebody = elsebody;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitIfNode(this);
        }
    }

    public class ReturnNode : Statement
    {
        public Expression ret;

        public ReturnNode(Expression ret, SourceLoc sl) : base(ASTKind.Return, sl)
        {
            this.ret = ret;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitReturnNode(this);
        }
    }

    public class BlockNode : Statement
    {
        public List<Statement> stmts;

        public BlockNode(List<Statement> stmts, SourceLoc sl) : base(ASTKind.Block, sl)
        {
            this.stmts = stmts;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBlockNode(this);
        }
    }

    public class IdenExprNode : Expression
    {
        public string name;

        public IdenExprNode(string name, TypeSymbol type, SourceLoc sl) : base(ASTKind.IdenExpr, type, sl)
        {
            this.name = name;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitIdenExprNode(this);
        }
    }

    public class IntExprNode : Expression
    {
        public IntExprNode(long value, SourceLoc sl) : base(ASTKind.IntExpr, TypeSymbol.INT_SYMBOL, sl)
        {
            IntValue = value;
            IsConstant = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitIntExprNode(this);
        }
    }

    public class FloatExprNode : Expression
    {
        public FloatExprNode(double value, SourceLoc sl) : base(ASTKind.FloatExpr, TypeSymbol.FLOAT_SYMBOL, sl)
        {
            FloatValue = value;
            IsConstant = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFloatExprNode(this);
        }
    }

    public class StringExprNode : Expression
    {
        public StringExprNode(string value, SourceLoc sl) : base(ASTKind.StringExpr, TypeSymbol.STRING_SYMBOL, sl)
        {
            StringValue = value;
            IsConstant = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStringExprNode(this);
        }
    }

    public class UnaryExprNode : Expression
    {
        public Expression expr;
        public string op;

        public UnaryExprNode(Expression expr, string op, SourceLoc sl) : base(ASTKind.UnaryExpr, expr.Type, sl)
        {
            this.expr = expr;
            this.op = op;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitUnaryExprNode(this);
        }
    }

    public class BinaryExprNode : Expression
    {
        public Expression lhs;
        public Expression rhs;
        public string op;

        public BinaryExprNode(Expression lhs, Expression rhs, string op, SourceLoc sl) : base(ASTKind.BinaryExpr, TypeSymbol.INFER_SYMOBOL(""), sl)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBinaryExprNode(this);
        }
    }

    public class FunCallExprNode : Expression
    {
        public Expression name;
        public List<Expression> args;
        public TypeSymbol funsig;

        public FunCallExprNode(Expression name, List<Expression> args, SourceLoc sl) : base(ASTKind.FunCallExpr, TypeSymbol.INFER_SYMOBOL(""), sl)
        {
            this.name = name;
            this.args = args;
            this.funsig = this.name.Type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFunCallExprNode(this);
        }
    }

    public class ImplicitFunCallExprNode : FunCallExprNode
    {
        public string funname;
        public string fullname;

        public ImplicitFunCallExprNode(string funname, Expression name, List<Expression> args, SourceLoc sl) : base(name, args, sl)
        {
            this.kind = ASTKind.FunCallExprImpl;
            this.funname = funname;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitImplicitFunCallExprNode(this);
        }
    }

    public class VarListExprNode : Expression
    {
        public Expression lower;
        public Expression upper;
        public Expression stepsize;

        public VarListExprNode(Expression lower, Expression upper, Expression stepsize, SourceLoc sl) 
            : base(ASTKind.ConstListExpr, TypeSymbol.INFER_SYMOBOL(""), sl)
        {
            this.lower = lower;
            this.upper = upper;
            this.stepsize = stepsize;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitConstListExprNode(this);
        }
    }

    public class ConstListExprNode : Expression
    {
        public ConstListExprNode(List<Expression> elems, SourceLoc sl) 
            : base(ASTKind.ListExpr, TypeSymbol.INFER_SYMOBOL(""), sl)
        {
            ListValue = elems;
            IsConstant = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitListExprNode(this);
        }
    }

    public class ArrayIndexNode : Expression
    {
        public Expression array;
        public Expression index;
        
        public ArrayIndexNode(Expression array, Expression index, SourceLoc sl) : base(ASTKind.ArrayIndex, TypeSymbol.INFER_SYMOBOL("[]"), sl)
        {
            this.array = array;
            this.index = index;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitArrayIndexNode(this);
        }
    }

    public class FieldAccessNode : Expression
    {

        public Expression basestruct;
        public string fieldname;

        public FieldAccessNode(Expression basestruct, string fieldname, SourceLoc sl) : base(ASTKind.FieldAccess, TypeSymbol.INFER_SYMOBOL(fieldname), sl)
        {
            this.basestruct = basestruct;
            this.fieldname = fieldname;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFieldAccessNode(this);
        }
    }

    public class NewStructNode : Expression
    {

        public NewStructNode(string name, SourceLoc sl) : base(ASTKind.NewStruct, TypeSymbol.INFER_SYMOBOL(name), sl)
        {  }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNewStructNode(this);
        }
    }

    public class DeferedNode : Statement
    {
        public Expression expr;

        public DeferedNode(Expression expr, SourceLoc sl) : base(ASTKind.Defer, sl)
        {
            this.expr = expr;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitDeferedNode(this);
        }
    }

    public class NullNode : Expression
    {
        public NullNode(SourceLoc sl) : base(ASTKind.Null, TypeSymbol.VOID_SYMBOL, sl)
        {
            IsConstant = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNullNode(this);
        }
    }
}

