using System.Data;

namespace DevBlah.SqlExpressionBuilder.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static int Insert<TFluent, TDbParameter>(this IDbConnection con,
            IDbInsertExpressionBuilder<TFluent, TDbParameter> builder)
            where TFluent : IDbInsertExpressionBuilder<TFluent, TDbParameter>
            where TDbParameter : IDbDataParameter
        {
            using (IDbCommand cmd = con.CreateCommand())
            {
                builder.FillCommand(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
