using DevBlah.SqlExpressionBuilder.Meta.Conditions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    public class StatementWhere : StatementBase
    {
        public ConditionSet ConditionSet { get; private set; }

        public StatementWhere(ConditionSet conditionSet)
            : base(SqlExpressionTypes.Where)
        {
            ConditionSet = conditionSet;
        }

        public override string ToString()
        {
            return string.Format("WHERE {0}", ConditionSet);
        }
    }
}