
namespace DevBlah.SqlExpressionBuilder.ObjectMap.Converter
{
    public interface INamingConventionConverter
    {
        string ConvertTableNameToEntityName(string tableName);

        string ConvertEntityNameToTableName(string entityName);

        string ConvertColumnNameToPropertyName(string columnName);

        string ConvertPropertyNameToColumnName(string propertyName);

    }
}
