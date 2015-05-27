using System;
using System.Collections.Generic;
using System.Data;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    public class RowSet
    {
        public RowSet(ColumnSet columnSet)
        {
            ColumnSet = columnSet;
        }

        public IDictionary<string, object> Row { get; set; }

        public ColumnSet ColumnSet { get; private set; }

        public IEnumerable<DbParameterProxy> GetParameters(IEnumerable<string> columns)
        {
            var parameters = new List<DbParameterProxy>();

            if (Row == null)
            {
                return parameters;
            }

            foreach (string columnKey in columns)
            {
                Tuple<DbType, int?> columnMeta = ColumnSet[columnKey];

                var param = new DbParameterProxy
                {
                    DbType = columnMeta.Item1,
                    ParameterName = string.Format("@{0}", columnKey),
                    Value = Row[columnKey] ?? DBNull.Value,
                    Size = columnMeta.Item2
                };

                parameters.Add(param);
            }

            return parameters;
        }
    }
}
