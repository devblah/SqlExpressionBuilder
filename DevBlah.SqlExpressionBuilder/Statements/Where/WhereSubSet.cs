using System.Collections.Generic;
using System.Text;

namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    public class WhereSubSet : WhereSet, IWhereSubSet
    {
        public WhereSubSet(ConnectOperations operation, IList<IWhereSubSet> parent)
        {
            Operation = operation;
            Parent = parent;
        }

        public ConnectOperations Operation { get; set; }

        public IList<IWhereSubSet> Parent { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            _AddSubsetsToStringBuilder(sb);

            return sb.ToString();
        }
    }
}
