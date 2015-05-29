using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
using DevBlah.SqlExpressionBuilder.Mixins;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    ///     Template class for using the expression builder with different db types
    /// </summary>
    /// <typeparam name="TFluent"></typeparam>
    /// <typeparam name="TDbParameter">subtype IDbParameter</typeparam>
    public abstract class DbSelectExpressionBuilder<TFluent, TDbParameter>
        : IDbSelectExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbSelectExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter, new()
    {
        private StatementSelect _stmtSelect = new StatementSelect();
        private readonly StatementFrom _stmtFrom = new StatementFrom();
        private readonly List<StatementJoin> _stmtJoin = new List<StatementJoin>();
        private readonly StatementWhere _stmtWhere = new StatementWhere(new ConditionSet());
        private readonly StatementGroup _stmtGroup = new StatementGroup();
        private readonly StatementOrder _stmtOrder = new StatementOrder();

        public IEnumerable<TDbParameter> Parameters
        {
            get
            {
                var list = new List<ParameterExpression>();

                // Add Where Clauses
                list.AddRange(WhereStmt.ConditionSet.GetParameterExpressions());

                // Add Join Clauses
                list.AddRange(_stmtJoin.SelectMany(j => j.On.GetParameterExpressions()));

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

        public IList<StatementJoin> Joins { get { return _stmtJoin; } }

        public StatementWhere WhereStmt { get { return _stmtWhere; } }


        public TFluent Join(SqlJoinTypes type, Table table, string @on)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, @on);
        }

        public TFluent Join(SqlJoinTypes type, string table, string alias, string @on)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, alias, @on);
        }

        public TFluent Join(SqlJoinTypes type, Table table, string @on, IEnumerable<ParameterExpression> parameters)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, @on, parameters);
        }

        public TFluent Join(SqlJoinTypes type, Table table, string @on, IEnumerable<string> columns, IEnumerable<ParameterExpression> parameters)
        {
            Select(columns, table);
            return IJoinStatementFacadeMixin.Join(this, type, table, @on, parameters);
        }

        public TFluent Join(SqlJoinTypes type, Table table, string @on, IEnumerable<string> columns)
        {
            Select(columns, table);
            return IJoinStatementFacadeMixin.Join(this, type, table, @on);
        }

        public TFluent Join(SqlJoinTypes type, Table table, ConditionSet onConditionSet)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, onConditionSet);
        }

        public TFluent Join(SqlJoinTypes type, Table table, ConditionSet onConditionSet, IEnumerable<string> columns)
        {
            Select(columns, table);
            return IJoinStatementFacadeMixin.Join(this, type, table, onConditionSet);
        }

        public TFluent Join(SqlJoinTypes type, Table table, IExpression left, CompareOperations compare, IExpression right)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, left, compare, right);
        }

        public TFluent Join(SqlJoinTypes type, ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return IJoinStatementFacadeMixin.Join(this, type, left, compare, right);
        }

        public TFluent Join(SqlJoinTypes type, Table table, IExpression left, IExpression right)
        {
            return IJoinStatementFacadeMixin.Join(this, type, table, left, right);
        }

        public TFluent Join(SqlJoinTypes type, ColumnExpression left, IExpression right)
        {
            return IJoinStatementFacadeMixin.Join(this, type, left, right);
        }

        public TFluent Join(SqlJoinTypes type, Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns)
        {
            Select(columns, table);
            return IJoinStatementFacadeMixin.Join(this, type, table, left, compare, right);
        }

        public TFluent Join(SqlJoinTypes type, ColumnExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            Select(columns, left.Table);
            return IJoinStatementFacadeMixin.Join(this, type, left, compare, right);
        }

        public TFluent Join(SqlJoinTypes type, Table table, IExpression left, IExpression right, IEnumerable<string> columns)
        {
            Select(columns, table);
            return IJoinStatementFacadeMixin.Join(this, type, table, left, right);
        }

        public TFluent Join(SqlJoinTypes type, ColumnExpression left, IExpression right, IEnumerable<string> columns)
        {
            Select(columns, left.Table);
            return IJoinStatementFacadeMixin.Join(this, type, left, right);
        }

        public TFluent JoinInner(Table table, string @on)
        {
            return Join(SqlJoinTypes.Inner, table, @on);
        }

        public TFluent JoinInner(string table, string alias, string @on)
        {
            return Join(SqlJoinTypes.Inner, table, alias, @on);
        }

        public TFluent JoinInner(Table table, string @on, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, table, @on, columns);
        }

        public TFluent JoinInner(Table table, string @on, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Inner, table, @on, parameters);
        }

        public TFluent JoinInner(Table table, string @on, IEnumerable<string> columns, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Inner, table, @on, columns, parameters);
        }

        public TFluent JoinInner(Table table, ConditionSet onConditionSet)
        {
            return Join(SqlJoinTypes.Inner, table, onConditionSet);
        }

        public TFluent JoinInner(Table table, ConditionSet onConditionSet, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, table, onConditionSet, columns);
        }

        public TFluent JoinInner(Table table, IExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Inner, table, left, compare, right);
        }

        public TFluent JoinInner(ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Inner, left, compare, right);
        }

        public TFluent JoinInner(Table table, IExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Inner, table, left, right);
        }

        public TFluent JoinInner(ColumnExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Inner, left, right);
        }

        public TFluent JoinInner(Table table, IExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, table, left, compare, right, columns);
        }

        public TFluent JoinInner(ColumnExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, left, compare, right, columns);
        }

        public TFluent JoinInner(Table table, IExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, table, left, right, columns);
        }

        public TFluent JoinInner(ColumnExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Inner, left, right, columns);
        }

        public TFluent JoinLeft(Table table, string @on)
        {
            return Join(SqlJoinTypes.Left, table, @on);
        }

        public TFluent JoinLeft(string table, string alias, string @on)
        {
            return Join(SqlJoinTypes.Left, table, alias, @on);
        }

        public TFluent JoinLeft(Table table, string @on, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, table, @on, columns);
        }

        public TFluent JoinLeft(Table table, string @on, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Left, table, @on, parameters);
        }

        public TFluent JoinLeft(Table table, string @on, IEnumerable<string> columns, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Left, table, @on, columns, parameters);
        }

        public TFluent JoinLeft(Table table, ConditionSet onConditionSet)
        {
            return Join(SqlJoinTypes.Left, table, onConditionSet);
        }

        public TFluent JoinLeft(Table table, ConditionSet onConditionSet, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, table, onConditionSet, columns);
        }

        public TFluent JoinLeft(Table table, IExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Left, table, left, compare, right);
        }

        public TFluent JoinLeft(ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Left, left, compare, right);
        }

        public TFluent JoinLeft(Table table, IExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Left, table, left, right);
        }

        public TFluent JoinLeft(ColumnExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Left, left, right);
        }

        public TFluent JoinLeft(Table table, IExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, table, left, compare, right, columns);
        }

        public TFluent JoinLeft(ColumnExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, left, compare, right, columns);
        }

        public TFluent JoinLeft(Table table, IExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, table, left, right, columns);
        }

        public TFluent JoinLeft(ColumnExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Left, left, right, columns);
        }

        public TFluent JoinOuter(Table table, string @on)
        {
            return Join(SqlJoinTypes.Outer, table, @on);
        }

        public TFluent JoinOuter(string table, string alias, string @on)
        {
            return Join(SqlJoinTypes.Outer, table, alias, @on);
        }

        public TFluent JoinOuter(Table table, string @on, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, table, @on, columns);
        }

        public TFluent JoinOuter(Table table, string @on, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Outer, table, @on, parameters);
        }

        public TFluent JoinOuter(Table table, string @on, IEnumerable<string> columns, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Outer, table, @on, columns, parameters);
        }

        public TFluent JoinOuter(Table table, ConditionSet onConditionSet)
        {
            return Join(SqlJoinTypes.Outer, table, onConditionSet);
        }

        public TFluent JoinOuter(Table table, ConditionSet onConditionSet, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, table, onConditionSet, columns);
        }

        public TFluent JoinOuter(Table table, IExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Outer, table, left, compare, right);
        }

        public TFluent JoinOuter(ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Outer, left, compare, right);
        }

        public TFluent JoinOuter(Table table, IExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Outer, table, left, right);
        }

        public TFluent JoinOuter(ColumnExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Outer, left, right);
        }

        public TFluent JoinOuter(Table table, IExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, table, left, compare, right, columns);
        }

        public TFluent JoinOuter(ColumnExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, left, compare, right, columns);
        }

        public TFluent JoinOuter(Table table, IExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, table, left, right, columns);
        }

        public TFluent JoinOuter(ColumnExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Outer, left, right, columns);
        }

        public TFluent JoinRight(Table table, string @on)
        {
            return Join(SqlJoinTypes.Right, table, @on);
        }

        public TFluent JoinRight(string table, string alias, string @on)
        {
            return Join(SqlJoinTypes.Right, table, alias, @on);
        }

        public TFluent JoinRight(Table table, string @on, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, table, @on, columns);
        }

        public TFluent JoinRight(Table table, string @on, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Right, table, @on, parameters);
        }

        public TFluent JoinRight(Table table, string @on, IEnumerable<string> columns, IEnumerable<ParameterExpression> parameters)
        {
            return Join(SqlJoinTypes.Right, table, @on, columns, parameters);
        }

        public TFluent JoinRight(Table table, ConditionSet onConditionSet)
        {
            return Join(SqlJoinTypes.Right, table, onConditionSet);
        }

        public TFluent JoinRight(Table table, ConditionSet onConditionSet, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, table, onConditionSet, columns);
        }

        public TFluent JoinRight(Table table, IExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Right, table, left, compare, right);
        }

        public TFluent JoinRight(ColumnExpression left, CompareOperations compare, IExpression right)
        {
            return Join(SqlJoinTypes.Right, left, compare, right);
        }

        public TFluent JoinRight(Table table, IExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Right, table, left, right);
        }

        public TFluent JoinRight(ColumnExpression left, IExpression right)
        {
            return Join(SqlJoinTypes.Right, left, right);
        }

        public TFluent JoinRight(Table table, IExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, table, left, compare, right, columns);
        }

        public TFluent JoinRight(ColumnExpression left, CompareOperations compare, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, left, compare, right, columns);
        }

        public TFluent JoinRight(Table table, IExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, table, left, right, columns);
        }

        public TFluent JoinRight(ColumnExpression left, IExpression right, IEnumerable<string> columns)
        {
            return Join(SqlJoinTypes.Right, left, right, columns);
        }

        public int Top
        {
            get { return _stmtSelect.Top; }
            set { _stmtSelect.Top = value; }
        }

        public TFluent BindParameter(TDbParameter parameter)
        {
            ParameterExpression existing = _FindDbParameter(parameter.ParameterName);

            existing.DbType = parameter.DbType;
            existing.Size = parameter.Size;
            existing.Value = parameter.Value;

            return (TFluent)this;
        }

        public TFluent BindParameter(string name, object value)
        {
            ParameterExpression existing = _FindDbParameter(name);

            existing.Value = value;

            return (TFluent)this;
        }

        public TFluent BindParameter(string name, DbType dbType, object value)
        {
            ParameterExpression existing = _FindDbParameter(name);

            existing.DbType = dbType;
            existing.Value = value;

            return (TFluent)this;
        }

        public TFluent BindParameters(IEnumerable<TDbParameter> parameters)
        {
            foreach (TDbParameter param in parameters)
            {
                BindParameter(param);
            }
            return (TFluent)this;
        }

        public TFluent Distinct()
        {
            _stmtSelect.Distinct = true;
            return (TFluent)this;
        }

        public TFluent FillCommand(IDbCommand cmd)
        {
            IEnumerable<TDbParameter> parameters = Parameters.ToList();
            List<TDbParameter> incompleteParameters = parameters.Where(x => x.Value == null).ToList();
            if (incompleteParameters.Count > 0)
            {
                throw new Exception(string.Format("The Parameters with the names {0} are not bound correctly",
                    string.Join(",", incompleteParameters.Select(x => string.Format("'{0}'", x.ParameterName)))));
            }
            cmd.CommandText = GetSqlString();
            foreach (TDbParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            return (TFluent)this;
        }

        public TFluent From(Table table)
        {
            _stmtFrom.Tables.Add(table);
            return (TFluent)this;
        }

        public TFluent From(string table)
        {
            From(new Table(table));
            return (TFluent)this;
        }

        public TFluent From(string table, string alias)
        {
            From(new Table(table, alias));
            return (TFluent)this;
        }

        public TFluent From(Table table, IEnumerable<string> columns)
        {
            From(table);
            if (columns != null)
            {
                _AddColumnsToSelect(columns, table);
            }
            return (TFluent)this;
        }

        public string GetSingleSelectString(IExpression select)
        {
            StatementSelect thisStmtSelect = _stmtSelect;
            _stmtSelect = new StatementSelect();
            _stmtSelect.Expressions.Add(select);
            string sql = GetSqlString();
            _stmtSelect = thisStmtSelect;
            return sql;
        }

        public string GetSqlString()
        {
            var expressions = new List<object> { _stmtSelect };

            if (_stmtFrom.Tables.Count == 0)
            {
                throw new InvalidOperationException("You have to specify a table to select from.");
            }

            expressions.Add(_stmtFrom);

            expressions.AddRange(_stmtJoin);

            if (!WhereStmt.ConditionSet.IsEmpty)
            {
                expressions.Add(WhereStmt);
            }

            if (_stmtGroup.Columns.Count > 0)
            {
                expressions.Add(_stmtGroup);
            }

            if (_stmtOrder.Columns.Count > 0)
            {
                expressions.Add(_stmtOrder);
            }

            return string.Join(" ", expressions);
        }

        public TFluent Group(IEnumerable<string> columns)
        {
            _stmtGroup.Columns.AddRange(columns);
            return (TFluent)this;
        }

        public TFluent Order(string column)
        {
            Order(column, OrderOptions.Asc);
            return (TFluent)this;
        }

        public TFluent Order(string column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return (TFluent)this;
        }

        public TFluent Order(string column, ExpressionOptions expOptions)
        {
            Order(column, OrderOptions.Asc, expOptions);
            return (TFluent)this;
        }

        public TFluent Order(string column, OrderOptions options, ExpressionOptions expOptions)
        {
            ColumnExpression col = _CreateColumnByName(column);
            if (expOptions == ExpressionOptions.Overwrite)
            {
                _stmtOrder.Columns.Clear();
            }
            _stmtOrder.Columns.Add(col, options);
            return (TFluent)this;
        }

        public TFluent Order(ColumnExpression column)
        {
            Order(column, OrderOptions.Asc);
            return (TFluent)this;
        }

        public TFluent Order(ColumnExpression column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return (TFluent)this;
        }

        public TFluent Order(ColumnExpression column, ExpressionOptions options)
        {
            Order(column, OrderOptions.Asc, options);
            return (TFluent)this;
        }

        public TFluent Order(ColumnExpression column, OrderOptions options, ExpressionOptions expOptions)
        {
            if (!_IsTablePresent(column.Table))
            {
                throw new Exception("The table of the order column doesn't exist in this context.");
            }
            if (expOptions == ExpressionOptions.Overwrite)
            {
                _stmtOrder.Columns.Clear();
            }
            _stmtOrder.Columns.Add(column, options);
            return (TFluent)this;
        }

        public TFluent Select(string column)
        {
            _stmtSelect.Expressions.Add(_CreateColumnByName(column));
            return (TFluent)this;
        }

        public TFluent Select(IEnumerable<string> columns)
        {
            foreach (string column in columns)
            {
                Select(column);
            }
            return (TFluent)this;
        }

        public TFluent Select(IEnumerable<string> columns, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
            {
                _stmtSelect.Expressions.Clear();
            }
            Select(columns);
            return (TFluent)this;
        }

        public TFluent Select(string column, Table table)
        {
            _stmtSelect.Expressions.Add(new ColumnExpression(column, table));
            return (TFluent)this;
        }

        public TFluent Select(IEnumerable<string> columns, Table table)
        {
            Select(columns, table, ExpressionOptions.Add);
            return (TFluent)this;
        }

        public TFluent Select(IEnumerable<string> columns, Table table, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
            {
                _stmtSelect.Expressions.Clear();
            }
            foreach (string column in columns)
            {
                Select(column, table);
            }
            return (TFluent)this;
        }

        public TFluent Select(IExpression expression)
        {
            _stmtSelect.Expressions.Add(expression);
            return (TFluent)this;
        }

        public override string ToString()
        {
            return GetSqlString();
        }

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

        private void _AddColumnsToSelect(IEnumerable<string> columns, Table table)
        {
            foreach (string col in columns)
            {
                _stmtSelect.Expressions.Add(new ColumnExpression(col, table));
            }
        }

        private void _CheckDefaultTable()
        {
            if (_stmtFrom.Tables.Count == 0)
            {
                throw new InvalidOperationException("You have to specify the default table first (FROM clause)");
            }
        }

        private ColumnExpression _CreateColumnByName(string column)
        {
            // check if default table exists
            _CheckDefaultTable();
            ColumnExpression col;
            // split to get table alias and column name
            string[] split = column.Split('.');
            if (split.Length == 1)
            {
                // no table alias is given -> take default table
                col = new ColumnExpression(column, _stmtFrom.Tables.First());
            }
            else if (split.Length == 2)
            {
                // search table with the given alias
                Table table = null;
                if (!_FindTableByAlias(split[0], ref table))
                {
                    throw new Exception(string.Format(
                        "Table with alias '{0}' does't exist in this context", split[0]));
                }
                col = new ColumnExpression(split[1], table);
            }
            else
            {
                throw new ArgumentException("Syntax error: dots are not allowed in column names.");
            }
            return col;
        }

        // ReSharper disable once RedundantAssignment
        private bool _FindTableByAlias(string alias, ref Table table)
        {
            table = _stmtFrom.Tables.FirstOrDefault(x => alias.Equals(x.Alias));
            if (table == null)
            {
                StatementJoin join = _stmtJoin.FirstOrDefault(x => alias.Equals(x.Table.Alias));
                if (join != null)
                {
                    table = join.Table;
                }
            }

            return table != null;
        }

        private bool _IsTablePresent(Table table)
        {
            return _stmtFrom.Tables.Any(x => x == table) || _stmtJoin.Any(x => table == x.Table);
        }

        private ParameterExpression _FindDbParameter(string name)
        {
            // Check where conditions
            ParameterExpression param =
                WhereStmt.ConditionSet.GetParameterExpressions().FirstOrDefault(x => x.ParameterName == name);
            if (param != null)
            {
                return param;
            }

            // Check Join Statements
            param =
                _stmtJoin.SelectMany(j => j.On.GetParameterExpressions()).FirstOrDefault(x => x.ParameterName == name);
            if (param != null)
            {
                return param;
            }


            throw new InvalidOperationException(string.Format("Parameter with name '{0}' couldn't be found", name));
        }
    }
}