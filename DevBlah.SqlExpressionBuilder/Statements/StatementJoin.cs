using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    public class StatementJoin : StatementBase
    {
        public SqlJoinTypes JoinType { get; private set; }

        public Table Table { get; private set; }

        public ConditionSet On { get; set; }

        public StatementJoin(SqlJoinTypes joinType, Table table, ConditionSet on)
            : base(SqlExpressionTypes.Join)
        {
            JoinType = joinType;
            Table = table;
            On = on;
        }

        public override string ToString()
        {
            return string.Format("{0} JOIN {1} ON {2}", JoinType.ToString().ToUpper(), Table, On);
        }
    }
}