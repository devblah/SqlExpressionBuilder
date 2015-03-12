using System.Data;

namespace DevBlah.SqlExpressionBuilder
{
    interface IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TFluent : IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
        TFluent Into(string tableName);
    }
}
