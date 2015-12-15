using System;
using System.Collections.Generic;
using DevBlah.SqlExpressionBuilder.ObjectMap.Mapper;

namespace DevBlah.SqlExpressionBuilder.ObjectMap
{
    public interface IMappingEngine
    {
        IMapper Mapper { get; }

        IDictionary<Type, MappingSet> Mappings { get; }

        IMappingEngine CreateMap(Type entityType, string tableName);

        IMappingEngine CreateMap<TEntity>(string tableName);
    }
}
