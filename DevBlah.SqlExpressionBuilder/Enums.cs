#pragma warning disable 1570
namespace DevBlah.SqlExpressionBuilder
{
    /// <summary>
    /// types of sql clauses
    /// </summary>
    public enum SqlExpressionTypes
    {
        /// <summary>
        /// SELECT clause
        /// </summary>
        Select,
        /// <summary>
        /// FROM clause
        /// </summary>
        From,
        /// <summary>
        /// JOIN clause
        /// </summary>
        Join,
        /// <summary>
        /// ORDER BY clause
        /// </summary>
        Order,
        /// <summary>
        /// HAVING clause
        /// </summary>
        Having,
        /// <summary>
        /// WHERE clause
        /// </summary>
        Where
    }

    /// <summary>
    /// types of join clauses
    /// </summary>
    public enum SqlJoinTypes
    {
        /// <summary>
        /// INNER JOIN
        /// </summary>
        Inner,
        /// <summary>
        /// OUTER JOIN
        /// </summary>
        Outer,
        /// <summary>
        /// LEFT OUTER JOIN
        /// </summary>
        Left,
        /// <summary>
        /// RIGHT OUTER JOIN
        /// </summary>
        Right
    }

    /// <summary>
    /// option for changing an expression
    /// </summary>
    public enum ExpressionOptions
    {
        /// <summary>
        /// add a new expression
        /// </summary>
        Add,
        /// <summary>
        /// overwrite expressions with equal name
        /// </summary>
        Overwrite
    }

    /// <summary>
    /// order types
    /// </summary>
    public enum OrderOptions
    {
        /// <summary>
        /// order ascending
        /// </summary>
        Asc,
        /// <summary>
        /// order descending
        /// </summary>
        Desc
    }

    /// <summary>
    /// types of sql operations
    /// </summary>
    public enum CompareOperations
    {
        /// <summary>
        /// {0} = {1}
        /// </summary>
        Equals,
        /// <summary>
        /// {0} != {1}
        /// </summary>
        NotEquals,
        /// <summary>
        /// {0} IN ({1})
        /// </summary>
        In,
        /// <summary>
        /// {0} NOT IN ({1})
        /// </summary>
        NotIn,
        /// <summary>
        /// {0} IN {1}
        /// </summary>
        InNoBraces,
        /// <summary>
        /// {0} NOT IN {1}
        /// </summary>
        NotInNoBraces,
        /// <summary>
        /// {0} IS {1}
        /// </summary>
        Is,
        /// <summary>
        /// {0} IS NOT {1}
        /// </summary>
        IsNot,
        /// <summary>
        /// {0} > {1}
        /// </summary>
        GreaterThan,
        /// <summary>
        /// {0} >= {1}
        /// </summary>
        GreaterThanEqual,
        /// <summary>
        /// {0} < {1}
        /// </summary>
        LowerThan,
        /// <summary>
        /// {0} <= {1}
        /// </summary>
        LowerThanEqual,
        /// <summary>
        /// {0} LIKE {1}
        /// </summary>
        Like
    }
}
#pragma warning restore 1570