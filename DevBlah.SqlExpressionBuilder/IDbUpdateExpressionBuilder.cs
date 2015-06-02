using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder
{
    public interface IDbUpdateExpressionBuilder<TFluent, TDbParameter>
        : IWhereStatementFacade<TFluent>
        where TFluent : IDbUpdateExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        RowSet RowSet { get; }

        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<TDbParameter> Parameters { get; }
    }
}
