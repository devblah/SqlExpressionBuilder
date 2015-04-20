using System.Data.SqlClient;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder.MsSql
{
    public class MsSqlInsertExpressionBuilder : DbInsertExpressionBuilder<MsSqlInsertExpressionBuilder, SqlParameter>
    {
        public MsSqlInsertExpressionBuilder(string tableName, ColumnSet columns, bool ignoreMissingColumns = false)
            : base(tableName, columns, ignoreMissingColumns)
        { }
    }
}
