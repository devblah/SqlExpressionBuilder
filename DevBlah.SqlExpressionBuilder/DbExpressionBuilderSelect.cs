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
    /// Template class for using the expression builder with different db types
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

        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Specifies how the parts of the where expression are connected to each other
        /// </summary>
        public string WhereLogicalConnectionString
        {
            get { return _stmtWhere.LogicalConnectionString; }
            set { _stmtWhere.LogicalConnectionString = value; }
        }

        /// <summary>
        /// Limits the results of a query
        ///
        /// TODO: make generic for other db types
        /// </summary>
        public int Top
        {
            get { return _stmtSelect.Top; }
            set { _stmtSelect.Top = value; }
        }

        /// <summary>
        /// Binds a IDbDataParameter object to this query
        /// </summary>
        /// <param name="parameter">IDbDataParameter object</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder BindParameter(IDbDataParameter parameter)
        {
            var param = _parameters.FirstOrDefault(x => x.ParameterName == parameter.ParameterName);
            if (param != null)
            {
                _parameters.Remove(param);
            }
            _parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Binds a parameter, which is already configured
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder BindParameter(string name, object value)
        {
            var param = _parameters.FirstOrDefault(x => x.ParameterName == name);
            if (param == null)
                throw new Exception(String.Format("Parameter with the Name '{0}' couldn't be found.", name));
            param.Value = value;
            return this;
        }

        /// <summary>
        /// Binds a parameter, which isn't configured yet
        /// </summary>
        /// <param name="name">name of the param</param>
        /// <param name="dbType">Database Type</param>
        /// <param name="value">value to bind</param>
        /// <returns></returns>
        public ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value)
        {
            BindParameter(name, dbType, value, false);
            return this;
        }

        /// <summary>
        /// Binds a parameter, which isn't configured yet
        /// </summary>
        /// <param name="name">name of the param</param>
        /// <param name="dbType">Database Type</param>
        /// <param name="value">value to bind</param>
        /// <param name="suppressWarning">if the exception, when a parameter with the given name couldn't be found
        ///     should be suppressed</param>
        /// <returns></returns>
        public ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value, bool suppressWarning)
        {
            var param = _parameters.FirstOrDefault(x => x.ParameterName == name);
            if (param == null && !suppressWarning)
            {
                throw new Exception(String.Format("Parameter with the Name '{0}' couldn't be found.", name));
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

        /// <summary>
        /// Binds a bunch of IDbDataParameter objects to this query
        /// </summary>
        /// <param name="parameters">Liste von IDbDataParametern</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder BindParameters(IEnumerable<IDbDataParameter> parameters)
        {
            foreach (IDbDataParameter param in parameters)
            {
                BindParameter(param);
            }
            return this;
        }

        /// <summary>
        /// Adds the distinct keyword to the query, to avoid getting duplicate rows
        /// </summary>
        /// <returns></returns>
        public ISqlExpressionBuilder Distinct()
        {
            _stmtSelect.Distinct = true;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cmd">SqlCommand to fill</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder FillCommand(IDbCommand cmd)
        {
            var incompleteParameters = Parameters.Where(x => x.Value == null).ToList();
            if (incompleteParameters.Count > 0)
            {
                throw new Exception(String.Format("The Parameters with the name {0} are not bound correctly",
                    String.Join(",", incompleteParameters.Select(x => String.Format("'{0}'", x.ParameterName)))));
            }
            cmd.CommandText = GetSqlString();
            foreach (IDbDataParameter parameter in Parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            return this;
        }

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">Tableobject</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder From(Table table)
        {
            _stmtFrom.Tables.Add(table);
            return this;
        }

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder From(string table)
        {
            From(new Table(table));
            return this;
        }

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <param name="alias">shorthand for the table</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder From(string table, string alias)
        {
            From(new Table(table, alias));
            return this;
        }

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">object of the table</param>
        /// <param name="columns">list of column names, which should be selected (without table alias)</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder From(Table table, IEnumerable<string> columns)
        {
            From(table);
            _AddColumnsToSelect(columns, table);
            return this;
        }

        /// <summary>
        /// gives the query string to count the sets
        /// </summary>
        /// <param name="select"></param>
        /// <returns>this instance</returns>
        public string GetSingleSelectString(IExpression select)
        {
            var thisStmtSelect = _stmtSelect;
            _stmtSelect = new StatementSelect();
            _stmtSelect.Expressions.Add(select);
            string sql = GetSqlString();
            _stmtSelect = thisStmtSelect;
            return sql;
        }

        /// <summary>
        /// creates the query string of this builder
        /// </summary>
        /// <returns>query string</returns>
        public string GetSqlString()
        {
            var expressions = new List<object> { _stmtSelect };

            if (_stmtFrom.Tables.Count == 0)
                throw new Exception("You have to specify a table to select from.");

            expressions.Add(_stmtFrom);

            expressions.AddRange(_stmtJoin);

            if (_stmtWhere.WhereClauses.Count > 0)
                expressions.Add(_stmtWhere);

            if (_stmtGroup.Columns.Count > 0)
                expressions.Add(_stmtGroup);

            if (_stmtOrder.Columns.Count > 0)
                expressions.Add(_stmtOrder);

            return String.Join(" ", expressions);
        }

        /// <summary>
        /// Adds a group by clause with the given columns
        /// </summary>
        /// <param name="columns">all columns which should be grouped</param>
        /// <returns></returns>
        public ISqlExpressionBuilder Group(IEnumerable<string> columns)
        {
            _stmtGroup.Columns.AddRange(columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on)
        {
            _SetParameters(_ParseParameters(on));
            _stmtJoin.Add(new StatementJoin(type, table, on));
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, string table, string alias, string on)
        {
            Join(type, new Table(table, alias), on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns)
        {
            Join(type, table, on);
            _AddColumnsToSelect(columns, table);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(type, table, on);
            BindParameters(parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(type, table, on, columns);
            BindParameters(parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ColumnExpression, ColumnExpression> on)
        {
            if (!_IsTablePresent(on.Actual.Table))
                throw new Exception("The referenced table you want to add does not exist in this context.");

            _stmtJoin.Add(new StatementJoin(type, on));
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ColumnExpression, ColumnExpression> on,
            IEnumerable<string> columns)
        {
            _AddColumnsToSelect(columns, on.Expected.Table);
            Join(type, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Table table, string on)
        {
            Join(SqlJoinTypes.Inner, table, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Inner, table, alias, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Inner, table, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Inner, table, on, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Inner, table, on, columns, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Inner, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinInner(Compare<ColumnExpression, ColumnExpression> on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Inner, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Table table, string on)
        {
            Join(SqlJoinTypes.Left, table, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Left, table, alias, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Left, table, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Left, table, on, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Left, table, on, columns, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Left, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinLeft(Compare<ColumnExpression, ColumnExpression> on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Left, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Table table, string on)
        {
            Join(SqlJoinTypes.Outer, table, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Outer, table, alias, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Outer, table, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Outer, table, on, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Outer, table, on, columns, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Outer, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinOuter(Compare<ColumnExpression, ColumnExpression> on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Outer, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Table table, string on)
        {
            Join(SqlJoinTypes.Right, table, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(string table, string alias, string on)
        {
            Join(SqlJoinTypes.Right, table, alias, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Right, table, on, columns);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Right, table, on, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters)
        {
            Join(SqlJoinTypes.Right, table, on, columns, parameters);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Compare<ColumnExpression, ColumnExpression> on)
        {
            Join(SqlJoinTypes.Right, on);
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder JoinRight(Compare<ColumnExpression, ColumnExpression> on, IEnumerable<string> columns)
        {
            Join(SqlJoinTypes.Right, on, columns);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(string column)
        {
            Order(column, OrderOptions.Asc);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(string column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(string column, ExpressionOptions expOptions)
        {
            Order(column, OrderOptions.Asc, expOptions);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(string column, OrderOptions options, ExpressionOptions expOptions)
        {
            var col = _CreateColumnByName(column);
            if (expOptions == ExpressionOptions.Overwrite)
                _stmtOrder.Columns.Clear();
            _stmtOrder.Columns.Add(col, options);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(ColumnExpression column)
        {
            Order(column, OrderOptions.Asc);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(ColumnExpression column, OrderOptions options)
        {
            Order(column, options, ExpressionOptions.Add);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">expression option</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(ColumnExpression column, ExpressionOptions options)
        {
            Order(column, OrderOptions.Asc, options);
            return this;
        }

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Order(ColumnExpression column, OrderOptions options, ExpressionOptions expOptions)
        {
            if (!_IsTablePresent(column.Table))
                throw new Exception("The table of the order column doesn't exist in this context.");
            if (expOptions == ExpressionOptions.Overwrite)
                _stmtOrder.Columns.Clear();
            _stmtOrder.Columns.Add(column, options);
            return this;
        }

        /// <summary>
        /// Selects a column from the default table
        /// </summary>
        /// <param name="column">name of the column which shoul be selected</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(string column)
        {
            _stmtSelect.Expressions.Add(_CreateColumnByName(column));
            return this;
        }

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(IEnumerable<string> columns)
        {
            foreach (string column in columns)
            {
                Select(column);
            }
            return this;
        }

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <param name="option">options for adding the columns</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(IEnumerable<string> columns, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
                _stmtSelect.Expressions.Clear();
            Select(columns);
            return this;
        }

        /// <summary>
        /// Selects a column from the given table
        /// </summary>
        /// <param name="column">name of the column, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(string column, Table table)
        {
            _stmtSelect.Expressions.Add(new ColumnExpression(column, table));
            return this;
        }

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table"> object where the column should be selected from</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table)
        {
            Select(columns, table, ExpressionOptions.Add);
            return this;
        }

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <param name="option">options for the expression</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table, ExpressionOptions option)
        {
            if (option == ExpressionOptions.Overwrite)
                _stmtSelect.Expressions.Clear();
            foreach (string column in columns)
            {
                Select(column, table);
            }
            return this;
        }

        /// <summary>
        /// Selects an expression with the given alias
        /// </summary>
        /// <param name="expression">expression obj</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Select(IExpression expression)
        {
            _stmtSelect.Expressions.Add(expression);
            return this;
        }

        /// <summary>
        /// Invokes GetSqlString()
        /// </summary>
        /// <returns>query string</returns>
        public override string ToString()
        {
            return GetSqlString();
        }

        /// <summary>
        /// Adds a clause to where statement
        /// </summary>
        /// <param name="clause">where clause to add</param>
        /// <param name="name">the name to identify the clause</param>
        /// <returns>this instance</returns>
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

        /// <summary>
        /// Adds a clause and their parameters to where statement
        /// </summary>
        /// <param name="clause">where clause to add</param>
        /// <param name="parameters">array of parameters of this clause</param>
        /// <param name="name">the name to identify the clause</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Where(string clause, IEnumerable<IDbDataParameter> parameters, string name = null)
        {
            Where(clause, name);
            BindParameters(parameters);
            return this;
        }

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter
        /// </summary>
        /// <param name="compare">compare instance</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Where(Compare<ColumnExpression, IDbDataParameter> compare)
        {
            Where(compare.ToString(), compare.Expected.ParameterName);
            BindParameter(compare.Expected);
            return this;
        }

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter, given as a string
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public ISqlExpressionBuilder Where(Compare<ColumnExpression, string> compare)
        {
            Where(compare.ToString());
            BindParameter(new TDbParameter { ParameterName = compare.Expected });
            return this;
        }

        /// <summary>
        /// Adds a single comparison between a column and an sql expression
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public ISqlExpressionBuilder Where(Compare<ColumnExpression, Expression> compare)
        {
            Where(compare.ToString());
            return this;
        }

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter
        /// </summary>
        /// <param name="compare">compare instance</param>
        /// <returns>this instance</returns>
        public ISqlExpressionBuilder Where(Compare<string, IDbDataParameter> compare)
        {
            Where(compare.ToString(), compare.Expected.ParameterName);
            BindParameter(compare.Expected);
            return this;
        }

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter, given as a string
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public ISqlExpressionBuilder Where(Compare<string, string> compare)
        {
            Where(compare.ToString());
            BindParameter(new TDbParameter { ParameterName = compare.Expected });
            return this;
        }

        /// <summary>
        /// Creates column objects from given names and adds them to the SELECT clause
        /// </summary>
        /// <param name="columns">list of column names</param>
        /// <param name="table">by column referenced table</param>
        private void _AddColumnsToSelect(IEnumerable<string> columns, Table table)
        {
            foreach (string col in columns)
            {
                _stmtSelect.Expressions.Add(new ColumnExpression(col, table));
            }
        }

        /// <summary>
        /// Checks if the default from table is set, throws an Exception if not
        /// </summary>
        private void _CheckDefaultTable()
        {
            if (_stmtFrom.Tables.Count == 0)
                throw new Exception("You have to specify the default table first (FROM clause)");
        }

        /// <summary>
        /// Creates a column by the given name
        /// </summary>
        /// <param name="column">column name</param>
        /// <returns>created column</returns>
        private ColumnExpression _CreateColumnByName(string column)
        {
            // check if default table exists
            _CheckDefaultTable();
            ColumnExpression col;
            // split to get table alias and column name
            var split = column.Split('.');
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
                    throw new Exception(String.Format(
                        "Table with alias '{0}' does't exist in this context", split[0]));
                }
                col = new ColumnExpression(split[1], table);
            }
            else
            {
                throw new Exception("Syntax error: dots are not allowed in column names.");
            }
            return col;
        }

        /// <summary>
        /// Searches a Table with the given alias and writes it to the reference given
        /// </summary>
        /// <param name="alias">alias to search for</param>
        /// <param name="table">table reference</param>
        /// <returns>if table was found or not</returns>
        // ReSharper disable once RedundantAssignment
        private bool _FindTableByAlias(string alias, ref Table table)
        {
            table = _stmtFrom.Tables.FirstOrDefault(x => alias.Equals(x.Alias));
            if (table == null)
            {
                var join = _stmtJoin.FirstOrDefault(x => alias.Equals(x.Table.Alias));
                if (join != null)
                    table = join.Table;
            }

            return table != null;
        }

        /// <summary>
        /// Checks if the table is already present
        /// </summary>
        /// <param name="table">the table to check</param>
        /// <returns>whether the table was added or not</returns>
        private bool _IsTablePresent(Table table)
        {
            return _stmtFrom.Tables.Any(x => x == table) || _stmtJoin.Any(x => table == x.Table);
        }

        /// <summary>
        /// Parses the parameters out of a statement and returns them in an array
        /// </summary>
        /// <param name="clause">clause which should be parsed</param>
        /// <returns>array of parameters</returns>
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