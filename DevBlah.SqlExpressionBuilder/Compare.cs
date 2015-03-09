using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevBlah.SqlExpressionBuilder
{
    public class Compare<TActual, TExpected>
    {
        public readonly Dictionary<CompareOperations, string> CompareTemplates
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

        public string CompareTemplate { get; private set; }

        public TActual Actual { get; private set; }

        public TExpected Expected { get; private set; }

        public Compare(CompareOperations operation, TActual actual, TExpected expected)
        {
            CompareTemplate = CompareTemplates[operation];
            Expected = expected;
            Actual = actual;
        }

        public Compare(string template, TActual actual, TExpected expected)
        {
            CompareTemplate = template;
            Expected = expected;
            Actual = actual;
        }

        public Compare(TActual actual, TExpected expected)
            : this(CompareOperations.Equals, actual, expected)
        { }

        public override string ToString()
        {
            if (!(Expected is string) && Expected is IEnumerable)
            {
                return String.Format(CompareTemplate, Actual, String.Join(", ",
                    ((IEnumerable)Expected).Cast<object>()));
            }
            return String.Format(CompareTemplate, Actual, Expected);
        }
    }
}