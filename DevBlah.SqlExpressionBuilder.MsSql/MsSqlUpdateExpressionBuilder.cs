using System.Collections.Generic;
using System.Data.SqlClient;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;

namespace DevBlah.SqlExpressionBuilder.MsSql
{
    public class MsSqlUpdateExpressionBuilder : DbUpdateExpressionBuilder<MsSqlUpdateExpressionBuilder, SqlParameter>
    {
        public MsSqlUpdateExpressionBuilder(string table, RowSet rowSet, ConditionSet whereConditionSet)
            : base(table, rowSet, whereConditionSet)
        { }

        public MsSqlUpdateExpressionBuilder(string table, ColumnSet columnSet, IDictionary<string, object> row, ConditionSet whereConditionSet)
            : base(table, columnSet, row, whereConditionSet)
        { }

        public MsSqlUpdateExpressionBuilder(string table, RowSet rowSet)
            : base(table, rowSet)
        { }

        public MsSqlUpdateExpressionBuilder(string table, ColumnSet columnSet, IDictionary<string, object> row)
            : base(table, columnSet, row)
        { }
    }
}
