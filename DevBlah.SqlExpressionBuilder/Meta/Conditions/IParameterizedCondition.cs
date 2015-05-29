using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Meta.Conditions
{
    public interface IParameterizedCondition
    {
        IEnumerable<ParameterExpression> GetParameterExpressions();
    }
}
