using System.Data.SqlClient;

namespace DevBlah.SqlExpressionBuilder.MsSql
{
    public class MsSqlDeleteExpressionBuilder : DbDeleteExpressionBuilder<MsSqlDeleteExpressionBuilder, SqlParameter>
    {
        public MsSqlDeleteExpressionBuilder(string table)
            : base(table)
        { }
    }
}
