using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevBlah.SqlExpressionBuilder.Mapping;

namespace DevBlah.SqlExpressionBuilder.ObjectMap.Mapper
{
    public class MappingSet<TPoco>
    {
        private IList<Item> _items = new List<Item>();

        public MappingSet<TPoco> Add<TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName)
        {
            //_items.Add();

            return this;
        }
    }
}
