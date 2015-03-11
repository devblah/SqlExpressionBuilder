using System;
using System.Collections.Generic;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementOrder : StatementBase
    {
        private Dictionary<ColumnExpression, OrderOptions> _columns = new Dictionary<ColumnExpression, OrderOptions>();

        public Dictionary<ColumnExpression, OrderOptions> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public StatementOrder()
            : base(SqlExpressionTypes.Order)
        { }

        public override string ToString()
        {
            return String.Format("ORDER BY {0}",
                String.Join(", ", _columns.Select(x => String.Format("{0} {1}", x.Key, x.Value.ToString().ToUpper()))));
        }
    }
}