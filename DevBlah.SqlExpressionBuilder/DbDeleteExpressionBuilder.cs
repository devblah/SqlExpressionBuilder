using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
using DevBlah.SqlExpressionBuilder.Mixins;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    public class DbDeleteExpressionBuilder<TFluent, TDbParameter>
        : IDbDeleteExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbDeleteExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter, new()
    {

        private readonly StatementWhere _stmtWhere;

        public StatementWhere WhereStmt { get { return _stmtWhere; } }

        public string Table { get; private set; }

        public IEnumerable<TDbParameter> Parameters
        {
            get
            {
                return WhereStmt.ConditionSet.GetParameterExpressions()
                    .Select(x =>
                    {
                        var param = new TDbParameter
                        {
                            ParameterName = x.ParameterName,
                            DbType = x.DbType,
                            Value = x.Value
                        };

                        if (x.Size.HasValue)
                        {
                            param.Size = x.Size.Value;
                        }

                        return param;
                    });
            }
        }

        public DbDeleteExpressionBuilder(string table, ConditionSet conditionSet)
        {
            Table = table;
            _stmtWhere = new StatementWhere(conditionSet);
        }

        public DbDeleteExpressionBuilder(string table)
            : this(table, new ConditionSet())
        { }

        public TFluent Where(string clause)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, clause);
        }

        public TFluent Where(string expression, IEnumerable<ParameterExpression> parameters)
        {
            return IWhereStatementFacadeMixinExtensions.Where(this, expression, parameters);
        }

        public TFluent Where(string expression, params ParameterExpression[] parameters)
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0}", Table);

            if (_stmtWhere.ConditionSet.Count > 0)
            {
                sb.Append(" ");
                sb.Append(_stmtWhere);
            }

            return sb.ToString();
        }
    }
}
