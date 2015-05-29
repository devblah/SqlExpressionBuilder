using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;

namespace DevBlah.SqlExpressionBuilder.Mixins
{
    // ReSharper disable once InconsistentNaming
    public static class IWhereStatementFacadeMixinExtensions
    {
        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, string clause)
        {
            builder.WhereStmt.ConditionSet.Add(new SimpleSubCondition(clause));
            return (TFluent)builder;
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, string clause,
            IEnumerable<ParameterExpression> parameters)
        {
            builder.WhereStmt.ConditionSet.Add(new SimpleSubCondition(clause, parameters));
            return (TFluent)builder;
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, IExpression exp,
            string value)
        {
            return Where(builder, exp, value, CompareOperations.Equals);
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, IExpression exp,
            string value, CompareOperations compare)
        {
            builder.WhereStmt.ConditionSet.Add(new ExpressionSubCondition(exp, compare, new Expression(value)));
            return (TFluent)builder;
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, IExpression exp1,
            IExpression exp2)
        {
            return Where(builder, exp1, exp2, CompareOperations.Equals);
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacade<TFluent> builder, IExpression exp1,
            IExpression exp2, CompareOperations compare)
        {
            var right = exp2 as ParameterExpression;
            if (right != null)
            {
                builder.WhereStmt.ConditionSet.Add(new ParameterizedExpressionSubCondition(exp1, compare, right));
            }
            else
            {
                builder.WhereStmt.ConditionSet.Add(new ExpressionSubCondition(exp1, compare, exp2));
            }
            return (TFluent)builder;
        }
    }
}
