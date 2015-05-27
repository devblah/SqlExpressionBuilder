using System.Collections.Generic;

namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    public class WhereValueSet : IWhereSubSet
    {
        /// <summary>
        /// corresponding format strings for the compare operations
        /// </summary>
        public static readonly Dictionary<CompareOperations, string> CompareTemplates
            = new Dictionary<CompareOperations, string>
            {
                    { CompareOperations.Equals, "{0} = {1}"},
                    { CompareOperations.NotEquals, "{0} != {1}" },
                    { CompareOperations.In, "{0} IN ({1})" },
                    { CompareOperations.NotIn, "{0} NOT IN ({1})" },
                    { CompareOperations.InNoBraces, "{0} IN {1}" },
                    { CompareOperations.NotInNoBraces, "{0} NOT IN {1}" },
                    { CompareOperations.Is, "{0} IS {1}" },
                    { CompareOperations.IsNot, "{0} IS NOT {1}" },
                    { CompareOperations.GreaterThan, "{0} > {1}" },
                    { CompareOperations.GreaterThanEqual, "{0} >= {1}" },
                    { CompareOperations.LowerThan, "{0} < {1}" },
                    { CompareOperations.LowerThanEqual, "{0} <= {1}" },
                    { CompareOperations.Like, "{0} LIKE {1}" }
                };

        public WhereValueSet(string column, CompareOperations compare, object value, IList<IWhereSubSet> parent)
        {
            Parent = parent;
            Column = column;
            CompareOperation = compare;
            Value = value;
        }

        public ConnectOperations Operation { get; set; }

        public IList<IWhereSubSet> Parent { get; private set; }

        public string Column { get; set; }

        public object Value { get; set; }

        public CompareOperations CompareOperation { get; set; }

        public override string ToString()
        {
            return string.Format(CompareTemplates[CompareOperation], Column, Value);
        }
    }
}
