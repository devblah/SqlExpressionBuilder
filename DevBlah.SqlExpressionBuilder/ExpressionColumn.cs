using System;

namespace DevBlah.SqlExpressionBuilder
{
    public class ExpressionColumn : Expression
    {
        public Table Table { get; private set; }

        public ExpressionColumn(string name, Table table)
            : base(name)
        {
            Table = table;
        }

        public override string ToString()
        {
            string ret = String.Format("{0}.{1}", Table.Alias, Content);
            return ret;
        }
    }
}