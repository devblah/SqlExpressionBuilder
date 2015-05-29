namespace DevBlah.SqlExpressionBuilder.Expressions
{
    /// <summary>
    /// creates an alias for the given expression (e.g. {expression} AS {alias})
    /// </summary>
    /// <typeparam name="T">type of the expression</typeparam>
    public class AliasedExpression<T> : IExpression where T : IExpression
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="expr">expression which has to be aliased</param>
        /// <param name="alias">alias for the expression</param>
        public AliasedExpression(T expr, string alias)
        {
            Alias = alias;
            Expression = expr;
        }

        /// <summary>
        /// alias for the expression
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// expression which has to be aliased
        /// </summary>
        public T Expression { get; private set; }

        /// <summary>
        /// creates the actual expression string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} AS {1}", Expression.ToString(), Alias);
        }
    }
}