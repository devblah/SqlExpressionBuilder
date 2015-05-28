using System.Text;

namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    public class WhereSubSet : WhereSet, IWhereSubSet
    {
        public WhereSubSet(ConnectOperations operation)
        {
            Operation = operation;
        }

        public WhereSubSet()
            : this(ConnectOperations.And)
        { }

        public ConnectOperations Operation { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            _AddSubsetsToStringBuilder(sb);

            return sb.ToString();
        }
    }
}
