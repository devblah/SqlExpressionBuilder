using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
using DevBlah.SqlExpressionBuilder.Mixins;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    public class DbUpdateExpressionBuilder<TFluent, TDbParameter>
        : IDbUpdateExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbUpdateExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter, new()
    {

        private readonly StatementWhere _stmtWhere;

        public RowSet Row { get; private set; }

        public ColumnSet ColumnSet { get; set; }

        public StatementWhere WhereStmt { get { return _stmtWhere; } }

        public IEnumerable<TDbParameter> Parameters { get; set; }

        public DbUpdateExpressionBuilder(RowSet row, ConditionSet whereConditionSet)
        {
            Row = row;
            _stmtWhere = new StatementWhere(whereConditionSet);
        }

        public DbUpdateExpressionBuilder(ColumnSet columnSet, IDictionary<string, object> row, ConditionSet whereConditionSet)
            : this(new RowSet(columnSet) { Row = row }, whereConditionSet)
        { }

        public TFluent Where(string clause)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, clause);
        }

        public TFluent Where(string expression, IEnumerable<ParameterExpression> parameters)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, expression, parameters);
        }

        public TFluent Where(IExpression exp, string value)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, exp, value);
        }

        public TFluent Where(IExpression exp, string value, CompareOperations compare)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, exp, value, compare);
        }

        public TFluent Where(IExpression exp1, IExpression exp2)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, exp1, exp2);
        }

        public TFluent Where(IExpression exp1, IExpression exp2, CompareOperations compare)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, exp1, exp2, compare);
        }
    }
}
