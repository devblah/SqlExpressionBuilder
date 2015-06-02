using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public RowSet RowSet { get; private set; }

        public StatementWhere WhereStmt { get { return _stmtWhere; } }

        public string Table { get; private set; }

        public IEnumerable<TDbParameter> Parameters
        {
            get
            {
                var list = new List<ParameterExpression>(RowSet.GetParameters(Table));

                list.AddRange(WhereStmt.ConditionSet.GetParameterExpressions());

                return list
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

        public DbUpdateExpressionBuilder(string table, RowSet rowSet, ConditionSet whereConditionSet)
        {
            Table = table;
            RowSet = rowSet;
            _stmtWhere = new StatementWhere(whereConditionSet);
        }

        public DbUpdateExpressionBuilder(string table, ColumnSet columnSet, IDictionary<string, object> row,
            ConditionSet whereConditionSet)
            : this(table, new RowSet(columnSet, row), whereConditionSet)
        { }

        public DbUpdateExpressionBuilder(string table, RowSet rowSet)
        {
            Table = table;
            RowSet = rowSet;
            _stmtWhere = new StatementWhere(new ConditionSet());
        }

        public DbUpdateExpressionBuilder(string table, ColumnSet columnSet, IDictionary<string, object> row)
            : this(table, new RowSet(columnSet, row))
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
            var sb = new StringBuilder("UPDATE ");
            sb.AppendFormat("{0} SET ", Table);

            var columns = new List<string>();

            foreach (KeyValuePair<string, object> row in RowSet)
            {
                KeyValuePair<string, Tuple<DbType, int?>> col;

                try
                {
                    col = RowSet.ColumnSet.First(x => x.Key == row.Key);
                }
                catch (Exception)
                {
                    throw new InvalidOperationException(
                        string.Format("The column '{0}' doesn't exist in the columnSet", row.Key));
                }

                columns.Add(string.Format("{0}.{1} = @{2}_{1}", Table, col.Key, Table.Replace(".", "")));
            }

            sb.Append(string.Join(", ", columns));

            if (_stmtWhere.ConditionSet.Count > 0)
            {
                sb.Append(" ");
                sb.Append(_stmtWhere);
            }

            return sb.ToString();
        }
    }
}
