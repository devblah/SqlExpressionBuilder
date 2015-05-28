using System.Collections.Generic;
using System.Text;
using DevBlah.SqlExpressionBuilder.Statements.Where;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    public class WhereSet : List<IWhereSubSet>
    {
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("WHERE ");

            _AddSubsetsToStringBuilder(sb);

            return sb.ToString();
        }

        protected void _AddSubsetsToStringBuilder(StringBuilder sb)
        {
            sb.Append("(");

            for (var i = 0; i < Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(" ").Append(this[i].Operation.ToString().ToUpper()).Append(" ");
                }

                sb.Append(this[i]);
            }

            sb.Append(")");
        }
    }
}
