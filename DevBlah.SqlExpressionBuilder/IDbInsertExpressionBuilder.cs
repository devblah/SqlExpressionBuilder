using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder
{
    public interface IDbInsertExpressionBuilder<out TFluent, out TDbParameter>
        where TFluent : IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// gets the configured columns for the current table
        /// </summary>
        ColumnSet ColumnSet { get; }

        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<TDbParameter> Parameters { get; }

        /// <summary>
        /// Adds a row to the current insert builder
        /// </summary>
        /// <param name="row"></param>
        void AddRow(IDictionary<string, object> row);

        /// <summary>
        /// Adds a row to the current insert builder
        /// </summary>
        /// <param name="row"></param>
        void AddRow(dynamic row);

        /// <summary>
        /// fills a command with the query representing the current expression state
        /// </summary>
        /// <param name="cmd">SqlCommand to fill</param>
        /// <returns>this instance</returns>
        TFluent FillCommand(IDbCommand cmd);
    }
}
