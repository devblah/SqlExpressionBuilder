
using System.Data;

namespace DevBlah.SqlExpressionBuilder
{
    public class DbInsertExpressionBuilder<TFluent, TDbParameter>
        : IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbInsertExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter
    {
    }
}
