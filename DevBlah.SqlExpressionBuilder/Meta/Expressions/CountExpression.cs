namespace DevBlah.SqlExpressionBuilder.Expressions
{
    /// <summary>
    /// Creates a count expression for a given column
    /// </summary>
    public class CountExpression : IExpression
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="column">column, which has to be counted</param>
        public CountExpression(ColumnExpression column)
        {
            Column = column;
        }

        /// <summary>
        /// column, which has to be counted
        /// </summary>
        public ColumnExpression Column { get; private set; }

        /// <summary>
        /// creates the actual expression string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("COUNT({0})", Column);
        }
    }
}