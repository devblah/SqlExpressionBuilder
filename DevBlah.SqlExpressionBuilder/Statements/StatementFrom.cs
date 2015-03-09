using System;
using System.Collections.Generic;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementFrom : StatementBase
    {
        private List<Table> _tables = new List<Table>();

        public List<Table> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        public StatementFrom()
            : base(SqlExpressionTypes.From)
        { }

        public override string ToString()
        {
            return String.Format("FROM {0}", String.Join(", ", Tables));
        }
    }
}