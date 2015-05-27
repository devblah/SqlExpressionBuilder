using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    public interface IDbUpdateExpressionBuilder<out TFluent, TDbParameter>
        : IWhereStatementFacade<TFluent>
        where TFluent : IDbUpdateExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        RowSet Row { get; }

        ColumnSet ColumnSet { get; }

        WhereSet WhereSet { get; }

        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<TDbParameter> Parameters { get; }
    }
}
