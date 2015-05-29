
namespace DevBlah.SqlExpressionBuilder.Meta.Conditions
{
    public class SubConditionSet : ConditionSet, ISubCondition
    {
        public SubConditionSet(ConnectOperations operation)
        {
            Operation = operation;
        }

        public SubConditionSet()
            : this(ConnectOperations.And)
        { }

        public ConnectOperations Operation { get; set; }
    }
}
