//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from llang.g4 by ANTLR 4.6

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="llangParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6")]
[System.CLSCompliant(false)]
public interface IllangVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.prog"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProg([NotNull] llangParser.ProgContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.toplevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitToplevel([NotNull] llangParser.ToplevelContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.libimport"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLibimport([NotNull] llangParser.LibimportContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.globalVar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGlobalVar([NotNull] llangParser.GlobalVarContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.fundef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFundef([NotNull] llangParser.FundefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.structdef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStructdef([NotNull] llangParser.StructdefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.enumdef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumdef([NotNull] llangParser.EnumdefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.defargslist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefargslist([NotNull] llangParser.DefargslistContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.defargitem"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefargitem([NotNull] llangParser.DefargitemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.structDeclList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStructDeclList([NotNull] llangParser.StructDeclListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.structDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStructDecl([NotNull] llangParser.StructDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.enumDeclList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumDeclList([NotNull] llangParser.EnumDeclListContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.enumDeclItem"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumDeclItem([NotNull] llangParser.EnumDeclItemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlock([NotNull] llangParser.BlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStmt([NotNull] llangParser.StmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.flowstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFlowstmt([NotNull] llangParser.FlowstmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.whilestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhilestmt([NotNull] llangParser.WhilestmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.forstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForstmt([NotNull] llangParser.ForstmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.ifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfstmt([NotNull] llangParser.IfstmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.retstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRetstmt([NotNull] llangParser.RetstmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.varDeclStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarDeclStmt([NotNull] llangParser.VarDeclStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.assignStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignStmt([NotNull] llangParser.AssignStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.deferstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeferstmt([NotNull] llangParser.DeferstmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.assignop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignop([NotNull] llangParser.AssignopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.funcallStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFuncallStmt([NotNull] llangParser.FuncallStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.argslist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgslist([NotNull] llangParser.ArgslistContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] llangParser.ExprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.varlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarlist([NotNull] llangParser.VarlistContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.elementlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElementlist([NotNull] llangParser.ElementlistContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.unaryop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryop([NotNull] llangParser.UnaryopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.multop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMultop([NotNull] llangParser.MultopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.addop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddop([NotNull] llangParser.AddopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.shiftop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitShiftop([NotNull] llangParser.ShiftopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.compop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompop([NotNull] llangParser.CompopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.eqop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEqop([NotNull] llangParser.EqopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.bitwiseop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBitwiseop([NotNull] llangParser.BitwiseopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.logicop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLogicop([NotNull] llangParser.LogicopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="llangParser.typename"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTypename([NotNull] llangParser.TypenameContext context);
}
