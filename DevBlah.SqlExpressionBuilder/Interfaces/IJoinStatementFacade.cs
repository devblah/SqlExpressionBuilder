
using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder.Interfaces
{
    public interface IJoinStatementFacade<out TFluent>
    {
        /// <summary>
        /// List of currently available JOIN clauses
        /// </summary>
        IList<StatementJoin> Joins { get; }

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, string on, IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns,
            IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, ConditionSet onConditionSet);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, ConditionSet onConditionSet, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, IExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, ColumnExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, IExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, ColumnExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, ColumnExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, Table table, IExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="type">type of the join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent Join(SqlJoinTypes type, ColumnExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a INNER JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, string on);

        /// <summary>
        /// Adds a INNER JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(string table, string alias, string on);

        /// <summary>
        /// Adds a INNER JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a INNER JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, string on, IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a INNER JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, string on, IEnumerable<string> columns,
            IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, ConditionSet onConditionSet);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, ConditionSet onConditionSet, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, IExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(ColumnExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, IExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(ColumnExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(ColumnExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(Table table, IExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinInner(ColumnExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, string on, IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, string on, IEnumerable<string> columns,
            IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, ConditionSet onConditionSet);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, ConditionSet onConditionSet, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, IExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(ColumnExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, IExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(ColumnExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(ColumnExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(Table table, IExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinLeft(ColumnExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, string on, IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, string on, IEnumerable<string> columns,
            IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, ConditionSet onConditionSet);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, ConditionSet onConditionSet, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, IExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(ColumnExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, IExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(ColumnExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(ColumnExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(Table table, IExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinOuter(ColumnExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">name of the table to join</param>
        /// <param name="alias">shorthand for the table to join</param>
        /// <param name="on">on clause as string</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(string table, string alias, string on);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, string on, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, string on, IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="on">on clause</param>
        /// <param name="columns">list of columns to select</param>
        /// <param name="parameters">list of parameters in this context</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, string on, IEnumerable<string> columns,
            IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, ConditionSet onConditionSet);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="onConditionSet">condition on which the tables should be joined</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, ConditionSet onConditionSet, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, IExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(ColumnExpression left, CompareOperations compare, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, IExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(ColumnExpression left, IExpression right);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, IExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="compare">compare operation</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(ColumnExpression left, CompareOperations compare, IExpression right,
            IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="table">table to join</param>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(Table table, IExpression left, IExpression right, IEnumerable<string> columns);

        /// <summary>
        /// Adds a JOIN clause for the given table. First column of the expression comparer references to
        /// the join table, second column to the table which will be referenced by the join.
        /// </summary>
        /// <param name="left">left side of the compare</param>
        /// <param name="right">right side of the compare</param>
        /// <param name="columns">list of column names, which should be selected</param>
        /// <returns>this instance</returns>
        TFluent JoinRight(ColumnExpression left, IExpression right, IEnumerable<string> columns);
    }
}
