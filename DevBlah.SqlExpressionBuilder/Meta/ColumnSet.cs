using System;
using System.Collections.Generic;
using System.Data;

namespace DevBlah.SqlExpressionBuilder.Meta
{
    public class ColumnSet : Dictionary<string, Tuple<DbType, int?>>
    {
        public void Add(string name, DbType type, int? length = null)
        {
            Add(name, new Tuple<DbType, int?>(type, length));
        }
    }
}
