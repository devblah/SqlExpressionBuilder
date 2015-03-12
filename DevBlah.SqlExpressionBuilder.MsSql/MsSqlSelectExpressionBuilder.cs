using System.Data.SqlClient;

namespace DevBlah.SqlExpressionBuilder.MsSql
{
    /// <summary>
    /// concrete implementation for MS SQL
    /// </summary>
    public class MsSqlSelectExpressionBuilder :
        DbSelectExpressionBuilder<MsSqlSelectExpressionBuilder, SqlParameter>
    { }
}