namespace DevBlah.SqlExpressionBuilder.Statements
{
    public abstract class StatementBase
    {
        public SqlExpressionTypes Type { get; private set; }

        public abstract override string ToString();

        protected StatementBase(SqlExpressionTypes type)
        {
            Type = type;
        }
    }
}