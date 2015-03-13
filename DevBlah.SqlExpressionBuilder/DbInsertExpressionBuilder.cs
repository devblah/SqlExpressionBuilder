using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevBlah.SqlExpressionBuilder.Meta;

namespace DevBlah.SqlExpressionBuilder
{
    public abstract class DbInsertExpressionBuilder<TFluent, TDbParameter>
        : IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbInsertExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter, new()
    {
        private IList<string> _columnKeys;
        private List<IDictionary<string, object>> _rows;
        private string _tableName;

        protected DbInsertExpressionBuilder(string tableName, ColumnSet columns, bool ignoreMissingColumns = false)
        {
            _tableName = tableName;
            ColumnSet = columns;

            if (!ignoreMissingColumns)
            {
                _columnKeys = columns.Keys.ToList();
            }
        }

        public ColumnSet ColumnSet { get; private set; }

        public IEnumerable<TDbParameter> Parameters { get { return _GetParameters(); } }

        public void AddRow(IDictionary<string, object> row)
        {
            if (_rows == null)
            {
                _rows = new List<IDictionary<string, object>>();
            }

            if (_columnKeys == null)
            {
                _ValidateAllRowColumnsExist(row, ColumnSet.Keys);
                _columnKeys = row.Keys.ToList();
            }
            else
            {
                _ValidateAll(row);
            }

            _rows.Add(row);
        }

        public TFluent FillCommand(IDbCommand cmd)
        {
            cmd.CommandText = ToString();

            foreach (TDbParameter parameter in Parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            return (TFluent)this;
        }

        public override string ToString()
        {
            if (_rows == null)
            {
                throw new InvalidOperationException("No rows added yet.");
            }

            var sb = new StringBuilder("INSERT INTO ");
            sb.Append(_tableName).Append(" (");
            sb.Append(string.Join(", ", _columnKeys)).Append(") VALUES ");

            var parameterStrings = new List<string>();

            for (int i = 0; i < _rows.Count; i++)
            {
                int index = i;
                parameterStrings.Add(string.Join(", ",
                    _columnKeys.Select(c => string.Format("@{0}_{1}", c, index))));
            }

            sb.Append(string.Join(", ", parameterStrings.Select(s => string.Format("({0})", s))));

            return sb.ToString();
        }

        private void _ValidateAll(IDictionary<string, object> row)
        {
            _ValidateAllRowColumnsExist(row, _columnKeys);

            _ValidateRowColumnsMissing(row, _columnKeys);
        }

        private void _ValidateAllRowColumnsExist(IDictionary<string, object> row, IEnumerable<string> compare)
        {
            // check if row has column not in 
            IEnumerable<string> columnsNotExisting =
                row.Keys.Where(rKey => compare.All(cKey => cKey != rKey)).ToList();
            if (columnsNotExisting.Any())
            {
                throw new InvalidOperationException(
                    string.Format("The columns {0} doesn't exist in the columnSet",
                        string.Join(", ", columnsNotExisting.Select(c => "'" + c + "'"))));
            }
        }

        private void _ValidateRowColumnsMissing(IDictionary<string, object> row, IEnumerable<string> compare)
        {
            // check if row has column not in 
            IEnumerable<string> columnsNotExisting =
                compare.Where(cKey => row.Keys.All(rKey => cKey != rKey)).ToList();
            if (columnsNotExisting.Any())
            {
                throw new InvalidOperationException(
                    string.Format("The columns {0} are missing. They are specified by columnSet or by the first row " +
                        "when the option 'ignoreMissingColumns' is active",
                        string.Join(", ", columnsNotExisting.Select(c => "'" + c + "'"))));
            }
        }

        private IEnumerable<TDbParameter> _GetParameters()
        {
            var parameters = new List<TDbParameter>();

            if (_rows == null)
            {
                return parameters;
            }

            for (int i = 0; i < _rows.Count; i++)
            {
                foreach (string columnKey in _columnKeys)
                {
                    Tuple<DbType, int?> columnMeta = ColumnSet[columnKey];

                    var param = new TDbParameter
                    {
                        DbType = columnMeta.Item1,
                        ParameterName = string.Format("@{0}_{1}", columnKey, i),
                        Value = _rows[i][columnKey] ?? DBNull.Value,
                    };

                    if (columnMeta.Item2.HasValue)
                    {
                        param.Size = columnMeta.Item2.Value;
                    }

                    parameters.Add(param);
                }
            }

            return parameters;
        }
    }
}
