using System;
using System.Linq;

namespace DevBlah.SqlExpressionBuilder
{
    public class Table
    {
        public string Name { get; set; }

        public string Alias { get; set; }

        public Table(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        public Table(string name)
            : this(name, name.Split(new[] { '.' }).Last())
        { }

        public override string ToString()
        {
            return String.Format("{0} {1}", Name, Alias);
        }
    }
}