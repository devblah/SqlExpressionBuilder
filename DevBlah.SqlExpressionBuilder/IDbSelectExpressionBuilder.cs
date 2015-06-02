using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    /// Interface for creating a implementation for a specific database driver
    /// </summary>
    public interface IDbSelectExpressionBuilder<out TFluent, TDbParameter>
        : IWhereStatementFacade<TFluent>, IJoinStatementFacade<TFluent>
        where TFluent : IDbSelectExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<TDbParameter> Parameters { get; }

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
        TFluent BindParameter(ParameterExpression parameter);

        /// <summary>
        /// Binds a parameter, which is already configured
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        /// <returns>this instance</returns>
        TFluent BindParameter(string name, object value);

        /// <summary>
        /// Binds a parameter, which isn't configured yet
        /// </summary>
        /// <param name="name">name of the param</param>
        /// <param name="dbType">Database Type</param>
        /// <param name="value">value to bind</param>
        /// <returns></returns>
        TFluent BindParameter(string name, DbType dbType, object value);

        /// <summary>
        /// Binds a bunch of IDbDataParameter objects to this query
        /// </summary>
        /// <param name="parameters">Liste von IDbDataParametern</param>
        /// <returns>this instance</returns>
        TFluent BindParameters(IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds the distinct keyword to the query, to avoid getting duplicate rows
        /// </summary>
        /// <returns></returns>
        TFluent Distinct();

        /// <summary>
        /// fills a command with the query representing the current expression state
        /// </summary>
        /// <param name="cmd">SqlCommand to fill</param>
        /// <returns>this instance</returns>
        TFluent FillCommand(IDbCommand cmd);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">Tableobject</param>
        /// <returns>this instance</returns>
        TFluent From(Table table);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <returns>this instance</returns>
        TFluent From(string table);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <param name="alias">shorthand for the table</param>
        /// <returns>this instance</returns>
        TFluent From(string table, string alias);

        /// <summary>
        /// Adds a table into the FROM clause
        /// </summary>
        /// <param name="table">object of the table</param>
        /// <param name="columns">list of column names, which should be selected (without table alias)</param>
        /// <returns>this instance</returns>
        TFluent From(Table table, IEnumerable<string> columns);

        /// <summary>
        /// gives the query string to count the sets
        /// </summary>
        /// <param name="select"></param>
        /// <returns>this instance</returns>
        string GetSingleSelectString(IExpression select);

        /// <summary>
        /// creates the query string from this builder instance
        /// </summary>
        /// <returns>query string</returns>
        string GetSqlString();

        /// <summary>
        /// Adds a group by clause with the given columns
        /// </summary>
        /// <param name="columns">all columns which should be grouped</param>
        /// <returns></returns>
        TFluent Group(IEnumerable<string> columns);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <returns>this instance</returns>
        TFluent Order(string column);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        TFluent Order(string column, OrderOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        TFluent Order(string column, ExpressionOptions expOptions);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">name of the column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        TFluent Order(string column, OrderOptions options, ExpressionOptions expOptions);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <returns>this instance</returns>
        TFluent Order(ColumnExpression column);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <returns>this instance</returns>
        TFluent Order(ColumnExpression column, OrderOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">expression option</param>
        /// <returns>this instance</returns>
        TFluent Order(ColumnExpression column, ExpressionOptions options);

        /// <summary>
        /// adds a column to the order expression
        /// </summary>
        /// <param name="column">column, which should be ordered</param>
        /// <param name="options">direction of the order</param>
        /// <param name="expOptions">expression option</param>
        /// <returns>this instance</returns>
        TFluent Order(ColumnExpression column, OrderOptions options, ExpressionOptions expOptions);

        /// <summary>
        /// Selects a column from the default table
        /// </summary>
        /// <param name="column">name of the column which shoul be selected</param>
        /// <returns>this instance</returns>
        TFluent Select(string column);

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <returns>this instance</returns>
        TFluent Select(IEnumerable<string> columns);

        /// <summary>
        /// Selects a bunch of columns from the default table
        /// </summary>
        /// <param name="columns">array of columnnames</param>
        /// <param name="option">options for adding the columns</param>
        /// <returns>this instance</returns>
        TFluent Select(IEnumerable<string> columns, ExpressionOptions option);

        /// <summary>
        /// Selects a column from the given table
        /// </summary>
        /// <param name="column">name of the column, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <returns>this instance</returns>
        TFluent Select(string column, Table table);

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table"> object where the column should be selected from</param>
        /// <returns>this instance</returns>
        TFluent Select(IEnumerable<string> columns, Table table);

        /// <summary>
        /// Selects a bunch of columns from the given table
        /// </summary>
        /// <param name="columns">names of the columns, which should be selected</param>
        /// <param name="table">table object where the column should be selected from</param>
        /// <param name="option">options for the expression</param>
        /// <returns>this instance</returns>
        TFluent Select(IEnumerable<string> columns, Table table, ExpressionOptions option);

        /// <summary>
        /// Selects an expression with the given alias
        /// </summary>
        /// <param name="expression">expression obj</param>
        /// <returns>this instance</returns>
        TFluent Select(IExpression expression);


    }
}