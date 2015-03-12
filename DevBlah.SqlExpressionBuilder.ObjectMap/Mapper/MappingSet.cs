using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Mapping
{
    public class MappingSet<TPoco>
    {
        private IList<Item> _items = new List<Item>();

        public MappingSet<TPoco> Add<TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName)
        {
            _items.Add();
        }
    }
}
