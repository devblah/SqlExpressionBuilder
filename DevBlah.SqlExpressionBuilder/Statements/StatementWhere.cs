using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevBlah.SqlExpressionBuilder.Statements
{
    internal class StatementWhere : StatementBase
    {
        private IDictionary<string, string> _whereClauses = new Dictionary<string, string>();

        public IDictionary<string, string> WhereClauses
        {
            get { return _whereClauses; }
        }

        public StatementWhere()
            : base(SqlExpressionTypes.Where)
        { }

        public string LogicalConnectionString { get; set; }

        public int UndefinedCount { get; set; }

        public override string ToString()
        {
            string connectionString;
            if (!string.IsNullOrWhiteSpace(LogicalConnectionString))
            {
                connectionString = LogicalConnectionString;
                var regParams = new Regex(@"(@[\w\d]+)", RegexOptions.Compiled);
                MatchCollection matches = regParams.Matches(LogicalConnectionString);
                int padding = 0;
                foreach (Match match in matches)
                {
                    if (!_whereClauses.ContainsKey(match.Value))
                    {
                        throw new ArgumentException(
                            string.Format("The Placeholder with the name '{0}' couldn't be found.", match.Value));
                    }

                    string stringBefore = connectionString.Substring(0, padding + match.Index);

                    connectionString = stringBefore + _whereClauses[match.Value] +
                        connectionString.Substring(padding + match.Index + match.Length);

                    padding += _whereClauses[match.Value].Length - match.Length;
                }
            }
            else
            {
                connectionString = String.Join(" AND ", WhereClauses.Select(x => x.Value));
            }
            return String.Format("WHERE {0}", connectionString);
        }
    }
}