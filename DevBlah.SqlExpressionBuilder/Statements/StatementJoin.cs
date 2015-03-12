using System;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementJoin : StatementBase
    {
        public SqlJoinTypes JoinType { get; private set; }

        public Table Table { get; private set; }

        public string On { get; private set; }

        public StatementJoin(SqlJoinTypes joinType, Table table, string on)
            : base(SqlExpressionTypes.Join)
        {
            JoinType = joinType;
            Table = table;
            On = on;
        }

        public StatementJoin(SqlJoinTypes joinType, Compare<ColumnExpression, ColumnExpression> comparer)
            : this(joinType, comparer.Expected.Table, comparer.ToString())
        { }

        public override string ToString()
        {
            return string.Format("{0} JOIN {1} ON {2}", JoinType.ToString().ToUpper(), Table, On);
        }
    }
}