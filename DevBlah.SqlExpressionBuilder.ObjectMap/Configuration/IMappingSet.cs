using System;
using System.Linq.Expressions;

namespace DevBlah.SqlExpressionBuilder.ObjectMap.Configuration
{
    public interface IMappingSet
    {
        Type EntityType { get; }

        IMappingSet AddField<TPoco, TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName, int size);

        IMappingSet AddField<TPoco, TProperty>(Expression<Func<TPoco, TProperty>> expr, string dbColumnName);

        IMappingSet AddField(string propertyName, string dbColumnName, int size);

        IMappingSet AddField(string propertyName, string dbColumnName);
    }
}
