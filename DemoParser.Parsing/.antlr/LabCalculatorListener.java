// Generated from c:/Users/nazar/OneDrive/Рабочий стол/унік/Прога/ООП/test/DemoParser.Parsing/LabCalculator.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link LabCalculatorParser}.
 */
public interface LabCalculatorListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link LabCalculatorParser#compileUnit}.
	 * @param ctx the parse tree
	 */
	void enterCompileUnit(LabCalculatorParser.CompileUnitContext ctx);
	/**
	 * Exit a parse tree produced by {@link LabCalculatorParser#compileUnit}.
	 * @param ctx the parse tree
	 */
	void exitCompileUnit(LabCalculatorParser.CompileUnitContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MultiplicativeExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MultiplicativeExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ExponentialExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterExponentialExpr(LabCalculatorParser.ExponentialExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ExponentialExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitExponentialExpr(LabCalculatorParser.ExponentialExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code AdditiveExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterAdditiveExpr(LabCalculatorParser.AdditiveExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code AdditiveExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitAdditiveExpr(LabCalculatorParser.AdditiveExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code NumberExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterNumberExpr(LabCalculatorParser.NumberExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code NumberExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitNumberExpr(LabCalculatorParser.NumberExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code IdentifierExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterIdentifierExpr(LabCalculatorParser.IdentifierExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code IdentifierExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitIdentifierExpr(LabCalculatorParser.IdentifierExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MminExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMminExpr(LabCalculatorParser.MminExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MminExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMminExpr(LabCalculatorParser.MminExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MmaxExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMmaxExpr(LabCalculatorParser.MmaxExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MmaxExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMmaxExpr(LabCalculatorParser.MmaxExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ParenthesizedExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ParenthesizedExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MaxExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMaxExpr(LabCalculatorParser.MaxExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MaxExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMaxExpr(LabCalculatorParser.MaxExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MinExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMinExpr(LabCalculatorParser.MinExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MinExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMinExpr(LabCalculatorParser.MinExprContext ctx);
	/**
	 * Enter a parse tree produced by the {@code UnaryExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterUnaryExpr(LabCalculatorParser.UnaryExprContext ctx);
	/**
	 * Exit a parse tree produced by the {@code UnaryExpr}
	 * labeled alternative in {@link LabCalculatorParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitUnaryExpr(LabCalculatorParser.UnaryExprContext ctx);
	/**
	 * Enter a parse tree produced by {@link LabCalculatorParser#arglist}.
	 * @param ctx the parse tree
	 */
	void enterArglist(LabCalculatorParser.ArglistContext ctx);
	/**
	 * Exit a parse tree produced by {@link LabCalculatorParser#arglist}.
	 * @param ctx the parse tree
	 */
	void exitArglist(LabCalculatorParser.ArglistContext ctx);
}