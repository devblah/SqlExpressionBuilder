namespace DevBlah.SqlExpressionBuilder.Expressions
{
    /// <summary>
    /// Creates an expression for a specific table column
    /// </summary>
    public class ColumnExpression : Expression
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">name of the column</param>
        /// <param name="table">depending table of the column</param>
        public ColumnExpression(string name, Table table)
            : base(name)
        {
            Table = table;
        }

        /// <summary>
        /// depending table of the column
        /// </summary>
        public Table Table { get; private set; }

        /// <summary>
        /// creates the actual expression string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", Table.Alias, Content);
        }
    }
}