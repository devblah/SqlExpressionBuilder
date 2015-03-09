using System.Collections.Generic;
using System.Data;

namespace DevBlah.SqlExpressionBuilder
{
    public interface ISqlExpressionBuilder
    {
        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<IDbDataParameter> Parameters { get; }

        /// <summary>
        /// Specifies how the parts of the where expression are connected to each other
        /// </summary>
        string WhereLogicalConnectionString { get; set; }

        /// <summary>
        /// Limits the results of a query
        ///
        /// TODO: make generic for other db types
        /// </summary>
        int Top { get; set; }

        /// <summary>
        /// Binds a IDbDataParameter object to this query
        /// </summary>
        /// <param name="parameter">IDbDataParameter object</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder BindParameter(IDbDataParameter parameter);

        /// <summary>
        /// Binds a parameter, which is already configured
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder BindParameter(string name, object value);

        /// <summary>
        /// Binds a parameter, which isn't configured yet
        /// </summary>
        /// <param name="name">name of the param</param>
        /// <param name="dbType">Database Type</param>
        /// <param name="value">value to bind</param>
        /// <returns></returns>
        ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value);

        /// <summary>
        /// Binds a parameter, which isn't configured yet
        /// </summary>
        /// <param name="name">name of the param</param>
        /// <param name="dbType">Database Type</param>
        /// <param name="value">value to bind</param>
        /// <param name="suppressWarning">if the exception, when a parameter with the given name couldn't be found
        ///     should be suppressed</param>
        /// <returns></returns>
        ISqlExpressionBuilder BindParameter(string name, DbType dbType, object value, bool suppressWarning);

        /// <summary>
        /// Binds a bunch of IDbDataParameter objects to this query
        /// </summary>
        /// <param name="parameters">Liste von IDbDataParametern</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder BindParameters(IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds the distinct keyword to the query, to avoid getting duplicate rows
        /// </summary>
        /// <returns></returns>
        ISqlExpressionBuilder Distinct();

        /// <summary>
        ///
        /// </summary>
        /// <param name="cmd">SqlCommand to fill</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder FillCommand(IDbCommand cmd);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">Tableobject</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder From(Table table);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder From(string table);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <param name="alias">shorthand for the table</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder From(string table, string alias);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">object of the table</param>
        /// <param name="columns">list of column names, which should be selected (without table alias)</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder From(Table table, IEnumerable<string> columns);

        /// <summary>
        /// gives the query string to count the sets
        /// </summary>
        /// <param name="select"></param>
        /// <returns>this instance</returns>
        string GetSingleSelectString(IExpression select);

        /// <summary>
        /// creates the query string of this builder
        /// </summary>
        /// <returns>query string</returns>
        string GetSqlString();

        /// <summary>
        /// Adds a group by clause with the given columns
        /// </summary>
        /// <param name="columns">all columns which should be grouped</param>
        /// <returns></returns>
        ISqlExpressionBuilder Group(IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ExpressionColumn, ExpressionColumn> on);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Join(SqlJoinTypes type, Compare<ExpressionColumn, ExpressionColumn> on,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Compare<ExpressionColumn, ExpressionColumn> on);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinInner(Compare<ExpressionColumn, ExpressionColumn> on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Compare<ExpressionColumn, ExpressionColumn> on);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinLeft(Compare<ExpressionColumn, ExpressionColumn> on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Compare<ExpressionColumn, ExpressionColumn> on);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinOuter(Compare<ExpressionColumn, ExpressionColumn> on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Table table, string on, IEnumerable<string> columns,
            IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">Columns to Join. </param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Compare<ExpressionColumn, ExpressionColumn> on);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="on">on clause for the join as compare expression</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder JoinRight(Compare<ExpressionColumn, ExpressionColumn> on, IEnumerable<string> columns);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(string column);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(string column, OrderOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(string column, ExpressionOptions expOptions);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(string column, OrderOptions options, ExpressionOptions expOptions);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(ExpressionColumn column);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(ExpressionColumn column, OrderOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">expression option</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(ExpressionColumn column, ExpressionOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Order(ExpressionColumn column, OrderOptions options, ExpressionOptions expOptions);

        /// <summary>
        /// Selects a column from the default table
        /// </summary>
        /// <param name="column">name of the column which shoul be selected</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(string column);

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(IEnumerable<string> columns);

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <param name="option">options for adding the columns</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(IEnumerable<string> columns, ExpressionOptions option);

        /// <summary>
        /// Selects a column from the given table
        /// </summary>
        /// <param name="column">name of the column, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(string column, Table table);

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table"> object where the column should be selected from</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table);

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <param name="option">options for the expression</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(IEnumerable<string> columns, Table table, ExpressionOptions option);

        /// <summary>
        /// Selects an expression with the given alias
        /// </summary>
        /// <param name="expression">expression obj</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Select(IExpression expression);

        /// <summary>
        /// Adds a clause to where statement
        /// </summary>
        /// <param name="clause">where clause to add</param>
        /// <param name="name">the name to identify the clause</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Where(string clause, string name = null);

        /// <summary>
        /// Adds a clause and their parameters to where statement
        /// </summary>
        /// <param name="clause">where clause to add</param>
        /// <param name="parameters">array of parameters of this clause</param>
        /// <param name="name">the name to identify the clause</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Where(string clause, IEnumerable<IDbDataParameter> parameters, string name = null);

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter
        /// </summary>
        /// <param name="compare">compare instance</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Where(Compare<ExpressionColumn, IDbDataParameter> compare);

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter, given as a string
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        ISqlExpressionBuilder Where(Compare<ExpressionColumn, string> compare);

        /// <summary>
        /// Adds a single comparison between a column and an sql expression
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        ISqlExpressionBuilder Where(Compare<ExpressionColumn, Expression> compare);

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter
        /// </summary>
        /// <param name="compare">compare instance</param>
        /// <returns>this instance</returns>
        ISqlExpressionBuilder Where(Compare<string, IDbDataParameter> compare);

        /// <summary>
        /// Adds a single comparison between a column and a IDbDataParameter, given as a string
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        ISqlExpressionBuilder Where(Compare<string, string> compare);
    }
}