namespace DevBlah.SqlExpressionBuilder.Expressions
{
    /// <summary>
    /// creates a SQL "NULL" expression
    /// </summary>
    public class NullExpression : Expression
    {
        /// <summary>
        /// constructor
        /// </summary>
        public NullExpression()
            : base("NULL")
        { }
    }
}