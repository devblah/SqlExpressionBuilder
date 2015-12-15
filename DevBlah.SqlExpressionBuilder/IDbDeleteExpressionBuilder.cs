using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Interfaces;

namespace DevBlah.SqlExpressionBuilder
{
    public interface IDbDeleteExpressionBuilder<TFluent, TDbParameter>
        : IWhereStatementFacade<TFluent>
        where TFluent : IDbDeleteExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        /// <summary>
        /// List of currently assigned db parameters
        /// </summary>
        IEnumerable<TDbParameter> Parameters { get; }
    }
}
