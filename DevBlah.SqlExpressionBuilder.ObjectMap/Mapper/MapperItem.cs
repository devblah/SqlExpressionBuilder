using System.Data;

namespace DevBlah.SqlExpressionBuilder.Mapping
{
    internal class Item
    {
        public Item(string objPropertyName, string dbColumnName, DbType dbType)
        {
            DbType = dbType;
            DbColumnName = dbColumnName;
            ObjPropertyName = objPropertyName;
        }

        public Item(string objPropertyName, string dbColumnName, DbType dbType, int size)
            : this(objPropertyName, dbColumnName, dbType)
        {
            Size = size;
        }

        public string ObjPropertyName { get; private set; }

        public string DbColumnName { get; private set; }

        public DbType DbType { get; private set; }

        public int Size { get; set; }
    }
}
