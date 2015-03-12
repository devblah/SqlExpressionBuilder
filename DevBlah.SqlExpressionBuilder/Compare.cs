using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    /// creates an sql comparation string between two elements
    /// </summary>
    /// <typeparam name="TActual">actual comparation value</typeparam>
    /// <typeparam name="TExpected">expected comparation value</typeparam>
    public class Compare<TActual, TExpected>
    {
        /// <summary>
        /// corresponding format strings for the compare operations
        /// </summary>
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

        /// <summary>
        /// contructor for compare operation from enum
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public Compare(CompareOperations operation, TActual actual, TExpected expected)
        {
            CompareTemplate = CompareTemplates[operation];
            Expected = expected;
            Actual = actual;
        }

        /// <summary>
        /// constructor for compare operation with manual template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public Compare(string template, TActual actual, TExpected expected)
        {
            CompareTemplate = template;
            Expected = expected;
            Actual = actual;
        }

        /// <summary>
        /// constructor with equals comparation
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public Compare(TActual actual, TExpected expected)
            : this(CompareOperations.Equals, actual, expected)
        { }

        /// <summary>
        /// actual comparation value
        /// </summary>
        public TActual Actual { get; private set; }

        /// <summary>
        /// current used compare template
        /// </summary>
        public string CompareTemplate { get; private set; }

        /// <summary>
        /// expected comparation value
        /// </summary>
        public TExpected Expected { get; private set; }

        /// <summary>
        /// creates a string of the comparation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!(Expected is string) && Expected is IEnumerable)
            {
                return string.Format(CompareTemplate, Actual, string.Join(", ",
                    ((IEnumerable)Expected).Cast<object>()));
            }
            return string.Format(CompareTemplate, Actual, Expected);
        }
    }
}