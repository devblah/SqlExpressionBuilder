using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Meta.Conditions
{
    public class SimpleSubCondition : ISubCondition, IParameterizedCondition
    {
        public SimpleSubCondition(ConnectOperations operation, string condition,
            IEnumerable<ParameterExpression> parameters)
        {
            Parameters = parameters;
            Condition = condition;
            Operation = operation;
        }

        public SimpleSubCondition(string condition, IEnumerable<ParameterExpression> parameters)
            : this(ConnectOperations.And, condition, parameters)
        { }

        public SimpleSubCondition(ConnectOperations operation, string condition)
            : this(operation, condition, new ParameterExpression[0])
        {
        }

        public SimpleSubCondition(string condition)
            : this(ConnectOperations.And, condition)
        { }

        public string GetStatement()
        {
            return Condition;
        }

        public string Condition { get; set; }

        public ConnectOperations Operation { get; set; }

        public IEnumerable<ParameterExpression> Parameters { get; private set; }

        public override string ToString()
        {
            return Condition;
        }

        public IEnumerable<ParameterExpression> GetParameterExpressions()
        {
            return Parameters;
        }
    }
}
