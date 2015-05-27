using System.Collections.Generic;

namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    class WhereSingleClause : IWhereSubSet
    {
        public WhereSingleClause(string expression, ConnectOperations operation, IList<IWhereSubSet> parent)
        {
            Expression = expression;
            Operation = operation;
            Parent = parent;
        }

        public string GetStatement()
        {
            return Expression;
        }

        public string Expression { get; set; }

        public ConnectOperations Operation { get; set; }

        public IList<IWhereSubSet> Parent { get; private set; }

        public override string ToString()
        {
            return Expression;
        }
    }
}
