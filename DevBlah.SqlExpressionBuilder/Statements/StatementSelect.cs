using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementSelect : StatementBase
    {
        private List<IExpression> _expressions = new List<IExpression>();

        public List<IExpression> Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        public bool Distinct { get; set; }

        public int Top { get; set; }

        public StatementSelect()
            : base(SqlExpressionTypes.Select)
        { }

        public override string ToString()
        {
            var sb = new StringBuilder("SELECT ");

            if (Distinct)
            {
                sb.Append("DISTINCT ");
            }

            if (Top > 0)
            {
                sb.AppendFormat("TOP {0} ", Top);
            }

            if (_expressions.Count > 0)
            {
                sb.Append(string.Join(", ", _expressions.Select(x => x.ToString())));
            }
            else
            {
                sb.Append("*");
            }

            return sb.ToString();
        }
    }
}