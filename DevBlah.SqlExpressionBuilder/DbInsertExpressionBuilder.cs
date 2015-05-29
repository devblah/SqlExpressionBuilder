using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder
{
    public abstract class DbInsertExpressionBuilder<TFluent, TDbParameter>
        : IDbInsertExpressionBuilder<TFluent, TDbParameter>
        where TFluent : DbInsertExpressionBuilder<TFluent, TDbParameter>
        where TDbParameter : IDbDataParameter, new()
    {
        protected IList<string> ColumnKeys;

        private List<RowSet> _insertSets;

        private string _tableName;

        protected DbInsertExpressionBuilder(string tableName, ColumnSet columns, bool ignoreMissingColumns = false)
        {
            _tableName = tableName;
            ColumnSet = columns;

            if (!ignoreMissingColumns)
            {
                ColumnKeys = columns.Keys.ToList();
            }
        }

        public ColumnSet ColumnSet { get; private set; }

        public IEnumerable<TDbParameter> Parameters { get { return _GetParameters(); } }

        public void AddRow(IDictionary<string, object> row)
        {
            if (_insertSets == null)
            {
                _insertSets = new List<RowSet>();
            }

            if (ColumnKeys == null)
            {
                _ValidateAllRowColumnsExist(row, ColumnSet.Keys);
                ColumnKeys = row.Keys.ToList();
            }
            else
            {
                _ValidateAll(row);
            }

            _insertSets.Add(new RowSet(ColumnSet) { Row = row });
        }

        public void AddRow(dynamic row)
        {
            var dict = row.ToDictionary();
            if (dict == null)
            {
                throw new ArgumentException("The dynamic object isn't a valid row.");
            }

            AddRow(dict);
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
            if (_insertSets == null || _insertSets.Count == 0)
            {
                throw new InvalidOperationException("No rows added yet.");
            }

            var sb = new StringBuilder("INSERT INTO ");
            sb.Append(_tableName).Append(" (");
            sb.Append(string.Join(", ", ColumnKeys)).Append(") VALUES ");

            var parameterStrings = new List<string>();

            for (int i = 0; i < _insertSets.Count; i++)
            {
                int index = i;
                parameterStrings.Add(string.Join(", ",
                    ColumnKeys.Select(c => string.Format("@{0}_{1}", c, index))));
            }

            sb.Append(string.Join(", ", parameterStrings.Select(s => string.Format("({0})", s))));

            return sb.ToString();
        }

        private IEnumerable<TDbParameter> _GetParameters()
        {
            var parameters = new List<TDbParameter>();

            if (_insertSets == null)
            {
                return parameters;
            }

            for (int i = 0; i < _insertSets.Count; i++)
            {
                int index = i;
                var currentParameters = _insertSets[i].GetParameters(ColumnKeys);
                parameters.AddRange(currentParameters.Select(p => p.ToDbDataParameter<TDbParameter>(index)));
            }

            return parameters;
        }

        private void _ValidateAll(IDictionary<string, object> row)
        {
            _ValidateAllRowColumnsExist(row, ColumnKeys);

            _ValidateRowColumnsMissing(row, ColumnKeys);
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
    }
}
