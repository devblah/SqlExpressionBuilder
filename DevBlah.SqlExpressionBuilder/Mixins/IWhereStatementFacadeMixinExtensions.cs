using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Statements.Where;

namespace DevBlah.SqlExpressionBuilder.Mixins
{
    // ReSharper disable once InconsistentNaming
    public static class IWhereStatementFacadeMixinExtensions
    {
        public static TFluent Where<TFluent>(this IWhereStatementFacadeMixin<TFluent> builder, string clause)
        {
            builder.WhereSet.Add(new WhereSingleClause(clause, ConnectOperations.And, builder.WhereSet));
            return (TFluent)builder;
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacadeMixin<TFluent> builder, IExpression exp,
            object value)
        {
            return Where(builder, exp, value, CompareOperations.Equals);
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacadeMixin<TFluent> builder, IExpression exp,
            object value, CompareOperations compare)
        {
            builder.WhereSet.Add(new WhereValueSet(exp.ToString(), compare, value, builder.WhereSet));
            return (TFluent)builder;
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacadeMixin<TFluent> builder, IExpression exp1,
            IExpression exp2)
        {
            return Where(builder, exp1, exp2, CompareOperations.Equals);
        }

        public static TFluent Where<TFluent>(this IWhereStatementFacadeMixin<TFluent> builder, IExpression exp1,
            IExpression exp2, CompareOperations compare)
        {
            builder.WhereSet.Add(new WhereValueSet(exp1.ToString(), compare, exp2, builder.WhereSet));
            return (TFluent)builder;
        }
    }
}
