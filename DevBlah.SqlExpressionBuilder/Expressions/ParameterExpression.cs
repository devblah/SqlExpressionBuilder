using System;
using System.Data;

namespace DevBlah.SqlExpressionBuilder.Expressions
{
    public class ParameterExpression : IExpression
    {
        public string ParameterName { get; set; }

        public DbType DbType { get; set; }

        public int? Size { get; set; }

        public object Value { get; set; }

        public ParameterExpression()
        {

        }

        public ParameterExpression(string name)
            : this()
        {
            ParameterName = name;
        }

        public ParameterExpression(string name, DbType type)
            : this(name)
        {
            DbType = type;
        }

        public ParameterExpression(string name, DbType type, int size)
            : this(name, type)
        {
            Size = size;
        }

        public TDbParameter ToDbDataParameter<TDbParameter>(object suffix = null)
            where TDbParameter : IDbDataParameter, new()
        {
            var param = new TDbParameter
            {
                DbType = DbType,
                ParameterName =
                    suffix == null ? ParameterName : string.Format("{0}_{1}", ParameterName, suffix),
                Value = Value ?? DBNull.Value
            };

            if (Size.HasValue)
            {
                param.Size = Size.Value;
            }

            return param;
        }

        public override string ToString()
        {
            return ParameterName;
        }
    }
}
