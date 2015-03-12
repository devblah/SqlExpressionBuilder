using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    ///     Template class for using the expression builder with different db types
    /// </summary>
    /// <typeparam name="TDbParameter">subtype IDbParameter</typeparam>
    public abstract class DbExpressionBuilderSelect<TDbParameter> : ISqlExpressionBuilder
        where TDbParameter : IDbDataParameter, new()
    {
        private readonly List<IDbDataParameter> _parameters = new List<IDbDataParameter>();
        private readonly StatementFrom _stmtFrom = new StatementFrom();
        private readonly List<StatementJoin> _stmtJoin = new List<StatementJoin>();
        private readonly StatementOrder _stmtOrder = new StatementOrder();
        private StatementSelect _stmtSelect = new StatementSelect();
        private readonly StatementWhere _stmtWhere = new StatementWhere();
        private readonly StatementGroup _stmtGroup = new StatementGroup();

        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return _parameters; }
        }

        public string WhereLogicalConnectionString
        {
            get { return _stmtWhere.LogicalConnectionString; }
            set { _stmtWhere.LogicalConnectionString = value; }
        }

        public int Top
        {
            get { return _stmtSelect.Top; }
            set { _stmtSelect.Top = value; }
        }

        public ISqlExpressionBuilder BindParameter(IDbDataParameter parameter)
        {
            IDbDataParameter param = _parameters.FirstOrDefault(x => x.ParameterName == parameter.ParameterName);
            if (param != null)
            {
                _parameters.Remove(param);
            }
            _parameters.Add(parameter);
            return this;
        }

        public ISqlExpressionBuilder BindParameter(string name, object value)
        {
            IDbDataParameter param = _parameters.FirstOrDefault(x => x.ParameterName == name);
            if (param == null)
            {
                throw new InvalidOperationException(
                    string.Format("Parameter with the Name '{0}' couldn't be found.", name));
            }
            param.Value = value;
            return this;
        }

        public ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value)
        {
            BindParameter(name, dbType, value, false);
            return this;
        }

        public ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value, bool suppressWarning)
        {
            IDbDataParameter param = _parameters.FirstOrDefault(x => x.ParameterName == name);
            if (param == null && !suppressWarning)
            {
                throw new InvalidOperationException(
                    string.Format("Parameter with the Name '{0}' couldn't be found.", name));
            }

            param = new TDbParameter
            {
                ParameterName = name,
                DbType = dbType,
                Value = value
            };

            _parameters.Add(param);
            return this;
        }

        public ISqlExpressionBuilder BindParameters(IEnumerable<IDbDataParameter> parameters)
        {
            foreach (IDbDataParameter param in parameters)
            {
                BindParameter(param);
            }
            return this;
        }

        public ISqlExpressionBuilder Distinct()
        {
            _stmtSelect.Distinct = true;
            return this;
        }

        public ISqlExpressionBuilder FillCommand(IDbCommand cmd)
        {
            List<IDbDataParameter> incompleteParameters = Parameters.Where(x => x.Value == null).ToList();
            if (incompleteParameters.Count > 0)
            {
                throw new Exception(string.Format("The Parameters with the names {0} are not bound correctly",
                    string.Join(",", incompleteParameters.Select(x => string.Format("'{0}'", x.ParameterName)))));
            }
            cmd.CommandText = GetSqlString();
            foreach (IDbDataParameter parameter in Parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            return this;
        }

        public ISqlExpressionBuilder From(Table table)
        {
            _stmtFrom.Tables.Add(table);
            return this;
        }

        public ISqlExpressionBuilder From(string table)
        {
            From(new Table(table));
            return this;
        }

        public ISqlExpressionBuilder From(string table, string alias)
        {
            From(new Table(table, alias));
            return this;
        }

        public ISqlExpressionBuilder From(Table table, IEnumerable<string> columns)
        {
            From(table);
            if (columns != null)
            {
                _AddColumnsToSelect(columns, table);
            }
            return this;
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

            if (_stmtWhere.WhereClauses.Count > 0)
            {
                expressions.Add(_stmtWhere);
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

        public ISqlExpressionBuilder Group(IEnumerable<string> columns)
        {
            _stmtGroup.Columns.AddRange(columns);
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on)
        {
            _SetParameters(_ParseParameters(on));
            _stmtJoin.Add(new StatementJoin(type, table, on));
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, string table, string alias, string on)
        {
            Join(type, new Table(table, alias), on);
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns)
        {
            Join(type, table, on);
            if (columns != null)
            {
                _AddColumnsToSelect(columns, table);
            }
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(type, table, on);
            BindParameters(parameters);
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(type, table, on, columns);
            BindParameters(parameters);
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ColumnExpression, ColumnExpression> on)
        {
            if (!_IsTablePresent(on.Actual.Table))
            {
                throw new InvalidOperationException(
                    "The referenced table you want to add does not exist in this context. " +
                    "The actual column has to be the first parameter, to check the context correctly");
            }

            _stmtJoin.Add(new StatementJoin(type, on));
            return this;
        }

        public ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            if (columns != null)
            {
                _AddColumnsToSelect(columns, on.Expected.Table);
            }
            Join(type, on);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Table table, string on)
        {
            Join(SqlJoinTypes.Inner, table, on);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Inner, table, alias, on);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Inner, table, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Inner, table, on, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Inner, table, on, columns, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Inner, on);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Inner, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(ColumnExpression actual, ColumnExpression expected)
        {
            JoinInner(actual, expected, CompareOperations.Equals, null);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(ColumnExpression actual, ColumnExpression expected,
            IEnumerable<string> columns)
        {
            JoinInner(actual, expected, CompareOperations.Equals, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare)
        {
            JoinInner(actual, expected, compare, null);
            return this;
        }

        public ISqlExpressionBuilder JoinInner(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Inner, new Compare<ColumnExpression, ColumnExpression>(compare, actual, expected),
                columns);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Table table, string on)
        {
            Join(SqlJoinTypes.Left, table, on);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Left, table, alias, on);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Left, table, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Left, table, on, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Left, table, on, columns, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Left, on);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Left, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(ColumnExpression actual, ColumnExpression expected)
        {
            JoinLeft(actual, expected, CompareOperations.Equals, null);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(ColumnExpression actual, ColumnExpression expected,
            IEnumerable<string> columns)
        {
            JoinLeft(actual, expected, CompareOperations.Equals, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare)
        {
            JoinLeft(actual, expected, compare, null);
            return this;
        }

        public ISqlExpressionBuilder JoinLeft(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Left, new Compare<ColumnExpression, ColumnExpression>(compare, actual, expected),
                columns);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Table table, string on)
        {
            Join(SqlJoinTypes.Outer, table, on);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Outer, table, alias, on);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Outer, table, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Outer, table, on, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Outer, table, on, columns, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Outer, on);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Outer, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(ColumnExpression actual, ColumnExpression expected)
        {
            JoinOuter(actual, expected, CompareOperations.Equals, null);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(ColumnExpression actual, ColumnExpression expected,
            IEnumerable<string> columns)
        {
            JoinOuter(actual, expected, CompareOperations.Equals, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare)
        {
            JoinOuter(actual, expected, compare, null);
            return this;
        }

        public ISqlExpressionBuilder JoinOuter(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Outer, new Compare<ColumnExpression, ColumnExpression>(compare, actual, expected),
                columns);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Table table, string on)
        {
            Join(SqlJoinTypes.Right, table, on);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Right, table, alias, on);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Right, table, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Right, table, on, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Right, table, on, columns, parameters);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Right, on);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Right, on, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(ColumnExpression actual, ColumnExpression expected)
        {
            JoinRight(actual, expected, CompareOperations.Equals, null);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(ColumnExpression actual, ColumnExpression expected,
            IEnumerable<string> columns)
        {
            JoinRight(actual, expected, CompareOperations.Equals, columns);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare)
        {
            JoinRight(actual, expected, compare, null);
            return this;
        }

        public ISqlExpressionBuilder JoinRight(ColumnExpression actual, ColumnExpression expected,
            CompareOperations compare, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Right, new Compare<ColumnExpression, ColumnExpression>(compare, actual, expected),
                columns);
            return this;
        }

        public ISqlExpressionBuilder Order(string column)
        {
            Order(column, OrderOptions.Asc);
            return this;
        }

        public ISqlExpressionBuilder Order(string column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return this;
        }

        public ISqlExpressionBuilder Order(string column, ExpressionOptions expOptions)
        {
            Order(column, OrderOptions.Asc, expOptions);
            return this;
        }

        public ISqlExpressionBuilder Order(string column, OrderOptions options, ExpressionOptions expOptions)
        {
            ColumnExpression col = _CreateColumnByName(column);
            if (expOptions == ExpressionOptions.Overwrite)
            {
                _stmtOrder.Columns.Clear();
            }
            _stmtOrder.Columns.Add(col, options);
            return this;
        }

        public ISqlExpressionBuilder Order(ColumnExpression column)
        {
            Order(column, OrderOptions.Asc);
            return this;
        }

        public ISqlExpressionBuilder Order(ColumnExpression column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return this;
        }

        public ISqlExpressionBuilder Order(ColumnExpression column, ExpressionOptions options)
        {
            Order(column, OrderOptions.Asc, options);
            return this;
        }

        public ISqlExpressionBuilder Order(ColumnExpression column, OrderOptions options, ExpressionOptions expOptions)
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
            return this;
        }

        public ISqlExpressionBuilder Select(string column)
        {
            _stmtSelect.Expressions.Add(_CreateColumnByName(column));
            return this;
        }

        public ISqlExpressionBuilder Select(IEnumerable<string> columns)
        {
            foreach (string column in columns)
            {
                Select(column);
            }
            return this;
        }

        public ISqlExpressionBuilder Select(IEnumerable<string> columns, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
            {
                _stmtSelect.Expressions.Clear();
            }
            Select(columns);
            return this;
        }

        public ISqlExpressionBuilder Select(string column, Table table)
        {
            _stmtSelect.Expressions.Add(new ColumnExpression(column, table));
            return this;
        }

        public ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table)
        {
            Select(columns, table, ExpressionOptions.Add);
            return this;
        }

        public ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
            {
                _stmtSelect.Expressions.Clear();
            }
            foreach (string column in columns)
            {
                Select(column, table);
            }
            return this;
        }

        public ISqlExpressionBuilder Select(IExpression expression)
        {
            _stmtSelect.Expressions.Add(expression);
            return this;
        }

        public override string ToString()
        {
            return GetSqlString();
        }

        public ISqlExpressionBuilder Where(string clause, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = string.Format("@Undefinded{0}", ++_stmtWhere.UndefinedCount);
            }
            _SetParameters(_ParseParameters(clause));
            _stmtWhere.WhereClauses.Add(name, clause);
            return this;
        }

        public ISqlExpressionBuilder Where(string clause, IEnumerable<IDbDataParameter> parameters, string name = null)
        {
            Where(clause, name);
            BindParameters(parameters);
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, IDbDataParameter param)
        {
            Where(col, param, CompareOperations.Equals);
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, IDbDataParameter param, CompareOperations compare)
        {
            Where(new Compare<ColumnExpression, IDbDataParameter>(compare, col, param));
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, string value)
        {
            Where(col, value, CompareOperations.Equals);
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, string value, CompareOperations compare)
        {
            Where(new Compare<ColumnExpression, string>(compare, col, value));
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, Expression expression)
        {
            Where(col, expression, CompareOperations.Equals);
            return this;
        }

        public ISqlExpressionBuilder Where(ColumnExpression col, Expression expression, CompareOperations compare)
        {
            Where(new Compare<ColumnExpression, Expression>(compare, col, expression));
            return this;
        }

        public ISqlExpressionBuilder Where(Compare<ColumnExpression, IDbDataParameter> compare)
        {
            Where(compare.ToString(), compare.Expected.ParameterName);
            BindParameter(compare.Expected);
            return this;
        }

        public ISqlExpressionBuilder Where(Compare<ColumnExpression, string> compare)
        {
            Where(compare.ToString());
            BindParameter(new TDbParameter { ParameterName = compare.Expected });
            return this;
        }

        public ISqlExpressionBuilder Where(Compare<ColumnExpression, Expression> compare)
        {
            Where(compare.ToString());
            return this;
        }

        public ISqlExpressionBuilder Where(Compare<string, IDbDataParameter> compare)
        {
            Where(compare.ToString(), compare.Expected.ParameterName);
            BindParameter(compare.Expected);
            return this;
        }

        public ISqlExpressionBuilder Where(Compare<string, string> compare)
        {
            Where(compare.ToString());
            BindParameter(new TDbParameter { ParameterName = compare.Expected });
            return this;
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

        private IEnumerable<string> _ParseParameters(string clause)
        {
            var regQuotedStrings = new Regex("'[^']*'", RegexOptions.Compiled);
            clause = regQuotedStrings.Replace(clause, "");
            var regParameters = new Regex(@"@[\w]+", RegexOptions.Compiled);
            return regParameters.Matches(clause).Cast<Match>().Select(x => x.Value).Distinct().ToArray();
        }

        private void _SetParameters(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                _parameters.Add(new TDbParameter { ParameterName = name });
            }
        }
    }
}