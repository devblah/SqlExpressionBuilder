using System;
using System.Collections.Generic;
using System.Linq;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementGroup
    {
        private List<string> _columns = new List<string>();

        public List<string> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public override string ToString()
        {
            return "GROUP BY " + String.Join(", ", Columns.Distinct());
        }
    }
}