using System.Data.SqlClient;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder.MsSql
{
    public class MsSqlInsertExpressionBuilder : DbInsertExpressionBuilder<MsSqlInsertExpressionBuilder, SqlParameter>
    {
        public MsSqlInsertExpressionBuilder(string table, ColumnSet columns, bool ignoreMissingColumns = false)
            : base(table, columns, ignoreMissingColumns)
        { }
    }
}
