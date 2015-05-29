using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder.Mixins
{
    // ReSharper disable once InconsistentNaming
    public static class IJoinStatementFacadeMixin
    {
        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, Table table, string @on)
        {
            var conditionSet = new ConditionSet
            {
                new SimpleSubCondition(@on)
            };
            builder.Joins.Add(new StatementJoin(type, table, conditionSet));
            return (TFluent)builder;
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, string table,
            string alias, string @on)
        {
            return Join(builder, type, new Table(table, alias), @on);
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, Table table,
            string @on, IEnumerable<ParameterExpression> parameters)
        {
            var conditionSet = new ConditionSet
            {
                new SimpleSubCondition(@on, parameters)
            };
            builder.Joins.Add(new StatementJoin(type, table, conditionSet));
            return (TFluent)builder;
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, Table table,
            ConditionSet onConditionSet)
        {
            builder.Joins.Add(new StatementJoin(type, table, onConditionSet));
            return (TFluent)builder;
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, Table table,
            IExpression left, CompareOperations compare, IExpression right)
        {
            var conditionSet = new ConditionSet();

            var parameterExpression = right as ParameterExpression;

            if (parameterExpression != null)
            {
                conditionSet.Add(new ParameterizedExpressionSubCondition(left, compare, parameterExpression));
            }
            else
            {
                conditionSet.Add(new ExpressionSubCondition(left, compare, right));
            }

            builder.Joins.Add(new StatementJoin(type, table, conditionSet));
            return (TFluent)builder;
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type,
            ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return Join(builder, type, left.Table, left, compare, right);
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type, Table table,
            IExpression left, IExpression right)
        {
            return Join(builder, type, table, left, CompareOperations.Equals, right);
        }

        public static TFluent Join<TFluent>(this IJoinStatementFacade<TFluent> builder, SqlJoinTypes type,
            ColumnExpression left, IExpression right)
        {
            return Join(builder, type, left.Table, left, CompareOperations.Equals, right);
        }
    }
}
