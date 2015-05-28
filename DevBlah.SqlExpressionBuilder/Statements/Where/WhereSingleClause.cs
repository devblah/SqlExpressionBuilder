
namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    public class WhereSingleClause : IWhereSubSet
    {
        public WhereSingleClause(ConnectOperations operation, string expression)
        {
            Expression = expression;
            Operation = operation;
        }

        public WhereSingleClause(string expression)
            : this(ConnectOperations.And, expression)
        { }

        public string GetStatement()
        {
            return Expression;
        }

        public string Expression { get; set; }

        public ConnectOperations Operation { get; set; }

        public override string ToString()
        {
            return Expression;
        }
    }
}
