using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Meta.Conditions
{
    public class ParameterizedExpressionSubCondition : ISubCondition, IParameterizedCondition
    {
        /// <summary>
        /// corresponding format strings for the compare operations
        /// </summary>
        public static readonly Dictionary<CompareOperations, string> CompareTemplates
            = new Dictionary<CompareOperations, string>
            {
                    { CompareOperations.Equals, "{0} = {1}"},
                    { CompareOperations.NotEquals, "{0} != {1}" },
                    { CompareOperations.In, "{0} IN ({1})" },
                    { CompareOperations.NotIn, "{0} NOT IN ({1})" },
                    { CompareOperations.InNoBraces, "{0} IN {1}" },
                    { CompareOperations.NotInNoBraces, "{0} NOT IN {1}" },
                    { CompareOperations.Is, "{0} IS {1}" },
                    { CompareOperations.IsNot, "{0} IS NOT {1}" },
                    { CompareOperations.GreaterThan, "{0} > {1}" },
                    { CompareOperations.GreaterThanEqual, "{0} >= {1}" },
                    { CompareOperations.LowerThan, "{0} < {1}" },
                    { CompareOperations.LowerThanEqual, "{0} <= {1}" },
                    { CompareOperations.Like, "{0} LIKE {1}" }
                };

        public ParameterizedExpressionSubCondition(ConnectOperations operation, IExpression left,
            CompareOperations compare, ParameterExpression right)
        {
            Operation = operation;
            Left = left;
            CompareOperation = compare;
            Right = right;
        }

        public ParameterizedExpressionSubCondition(ConnectOperations operation, IExpression left, ParameterExpression right)
            : this(operation, left, CompareOperations.Equals, right)
        { }

        public ParameterizedExpressionSubCondition(IExpression left, CompareOperations compare, ParameterExpression right)
            : this(ConnectOperations.And, left, compare, right)
        { }

        public ParameterizedExpressionSubCondition(IExpression left, ParameterExpression right)
            : this(left, CompareOperations.Equals, right)
        { }

        public ConnectOperations Operation { get; set; }

        public IExpression Left { get; set; }

        public ParameterExpression Right { get; set; }

        public CompareOperations CompareOperation { get; set; }

        public override string ToString()
        {
            return string.Format(CompareTemplates[CompareOperation], Left, Right);
        }

        public IEnumerable<ParameterExpression> GetParameterExpressions()
        {
            return new[] { Right };
        }
    }
}
