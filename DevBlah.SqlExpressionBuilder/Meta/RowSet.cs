using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Meta
{
    public class RowSet : IDictionary<string, object>
    {
        private IDictionary<string, object> _baseDictionary = new Dictionary<string, object>();

        public RowSet(ColumnSet columnSet)
        {
            ColumnSet = columnSet;
        }

        public RowSet(ColumnSet columnSet, IEnumerable<KeyValuePair<string, object>> items)
        {
            ColumnSet = columnSet;
            Add(items);
        }

        public ColumnSet ColumnSet { get; private set; }

        public IEnumerable<ParameterExpression> GetParameters(string tableName, IEnumerable<string> columns = null)
        {
            var parameters = new List<ParameterExpression>();

            if (Count == 0)
            {
                return parameters;
            }

            if (columns == null)
            {
                columns = _baseDictionary.Keys;
            }

            foreach (string columnKey in columns)
            {
                Tuple<DbType, int?> columnMeta = ColumnSet[columnKey];

                var param = new ParameterExpression
                {
                    DbType = columnMeta.Item1,
                    ParameterName = string.Format("@{0}_{1}", tableName.Replace(".", ""), columnKey),
                    Value = this[columnKey] ?? DBNull.Value,
                    Size = columnMeta.Item2
                };

                parameters.Add(param);
            }

            return parameters;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _baseDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            KeyValuePair<string, Tuple<DbType, int?>> col;

            try
            {
                col = ColumnSet.First(x => x.Key == item.Key);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(
                    string.Format("The column '{0}' doesn't exist in the columnSet", item.Key));
            }

            _baseDictionary.Add(item);
        }

        public void Clear()
        {
            _baseDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _baseDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _baseDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _baseDictionary.Remove(item);
        }

        public int Count { get { return _baseDictionary.Count; } }

        public bool IsReadOnly { get { return _baseDictionary.IsReadOnly; } }

        public bool ContainsKey(string key)
        {
            return _baseDictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
        }

        public void Add(IEnumerable<KeyValuePair<string, object>> items)
        {
            foreach (KeyValuePair<string, object> keyValuePair in items)
            {
                Add(keyValuePair);
            }
        }

        public bool Remove(string key)
        {
            return _baseDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _baseDictionary.TryGetValue(key, out value);
        }

        public object this[string key]
        {
            get { return _baseDictionary[key]; }
            set { _baseDictionary[key] = value; }
        }

        public ICollection<string> Keys { get { return _baseDictionary.Keys; } }

        public ICollection<object> Values { get { return _baseDictionary.Values; } }
    }
}
