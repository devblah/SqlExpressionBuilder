using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DevBlah.DotNetToolkit.Linq.Expressions;
using DevBlah.SqlExpressionBuilder.ObjectMap.Configuration;

namespace DevBlah.SqlExpressionBuilder.ObjectMap.Mapper
{
    public class MappingSet : IMappingSet
    {
        private IList<Item> _items = new List<Item>();

        public Type EntityType { get; private set; }

        internal MappingSet(Type entityType)
        {
            EntityType = entityType;
        }

        public IMappingSet AddField<TPoco, TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName, int size)
        {
            PropertyInfo propertyInfo = expr.GetPropertyInfo();

            _items.Add(new Item(propertyInfo.Name, dbColumnName, TypeMap.ToDbType(propertyInfo.PropertyType), size));

            return this;
        }

        public IMappingSet AddField<TPoco, TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName)
        {
            PropertyInfo propertyInfo = expr.GetPropertyInfo();

            _items.Add(new Item(propertyInfo.Name, dbColumnName, TypeMap.ToDbType(propertyInfo.PropertyType)));

            return this;
        }

        public IMappingSet AddField(string propertyName, string dbColumnName, int size)
        {
            PropertyInfo propertyInfo = EntityType.GetProperty(propertyName,
                BindingFlags.Public & BindingFlags.GetProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("A puplic property named '{0}' does not exist in type '{1}'",
                    propertyName, EntityType.Name), "propertyName");
            }

            _items.Add(new Item(propertyInfo.Name, dbColumnName, TypeMap.ToDbType(propertyInfo.PropertyType), size));

            return this;
        }

        public IMappingSet AddField(string propertyName, string dbColumnName)
        {
            PropertyInfo propertyInfo = EntityType.GetProperty(propertyName,
                BindingFlags.Public & BindingFlags.GetProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("A puplic property named '{0}' does not exist in type '{1}'",
                    propertyName, EntityType.Name), "propertyName");
            }

            _items.Add(new Item(propertyInfo.Name, dbColumnName, TypeMap.ToDbType(propertyInfo.PropertyType)));

            return this;
        }
    }
}
