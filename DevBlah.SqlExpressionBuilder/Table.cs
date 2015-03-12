using System.Collections.Generic;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    /// represents a database table
    /// </summary>
    public class Table
    {
        /// <summary>
        /// list of columns used in this table
        /// </summary>
        private List<ColumnExpression> _columns;

        /// <summary>
        /// constructor with name and alias
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public Table(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        /// <summary>
        /// constructor where the table name is used as alias
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
            : this(name, name.Split(new[] { '.' }).Last())
        { }

        /// <summary>
        /// the alias for the current table
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// the name of the current table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// gets a column expression depending to this table
        /// </summary>
        /// <param name="name">name of the column</param>
        /// <returns>column expression object</returns>
        public ColumnExpression GetColumn(string name)
        {
            if (_columns == null)
            {
                _columns = new List<ColumnExpression>();
            }

            ColumnExpression current = _columns.FirstOrDefault(c => c.Content == name);

            if (current == null)
            {
                current = new ColumnExpression(name, this);
                _columns.Add(current);
            }

            return current;
        }

        /// <summary>
        /// creates the table string including the alias
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Alias);
        }
    }
}