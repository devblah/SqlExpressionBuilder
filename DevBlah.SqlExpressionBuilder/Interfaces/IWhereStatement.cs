
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Interfaces
{
    public interface IWhereStatementFacade<out TFluent>
    {
        /// <summary>
        /// Adds a clause to where statement
        /// </summary>
        /// <param name="expression">where clause to add</param>
        /// <returns>this instance</returns>
        TFluent Where(string expression);

        /// <summary>
        /// Adds a single comparison between a column and an sql expression
        /// </summary>
        /// <param name="col">column, which should be compared</param>
        /// <param name="expression">expression, which should be compared</param>
        /// <returns></returns>
        //TFluent Where(ColumnExpression col, IExpression expression);

        /// <summary>
        /// Adds a single comparison between a column and an sql expression
        /// </summary>
        /// <param name="col">column, which should be compared</param>
        /// <param name="expression">expression, which should be compared</param>
        /// <param name="compare">how to compare the two expressions</param>
        /// <returns></returns>
        //TFluent Where(ColumnExpression col, IExpression expression, CompareOperations compare);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="col">column, which should be compared</param>
        /// <param name="value">value, which should be compared</param>
        /// <returns>this instance</returns>
        //TFluent Where(ColumnExpression col, object value);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="col">column, which should be compared</param>
        /// <param name="value">value, which should be compared</param>
        /// <param name="compare">how to compare the two expressions</param>
        /// <returns>this instance</returns>
        //TFluent Where(ColumnExpression col, object value, CompareOperations compare);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="exp">expression, which should be compared</param>
        /// <param name="value">value, which should be compared</param>
        /// <returns>this instance</returns>
        TFluent Where(IExpression exp, object value);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="exp">expression, which should be compared</param>
        /// <param name="value">value, which should be compared</param>
        /// <param name="compare">how to compare the two expressions</param>
        /// <returns>this instance</returns>
        TFluent Where(IExpression exp, object value, CompareOperations compare);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="exp1">expression, which should be compared</param>
        /// <param name="exp2">expression, which should be compared to</param>
        /// <returns>this instance</returns>
        TFluent Where(IExpression exp1, IExpression exp2);

        /// <summary>
        /// Adds a single comparison between a column and a given value, given as a string
        /// </summary>
        /// <param name="exp1">expression, which should be compared</param>
        /// <param name="exp2">expression, which should be compared to</param>
        /// <param name="compare">how to compare the two expressions</param>
        /// <returns>this instance</returns>
        TFluent Where(IExpression exp1, IExpression exp2, CompareOperations compare);
    }
}
