namespace DevBlah.SqlExpressionBuilder
{
    public enum SqlExpressionTypes
    {
        Select,
        From,
        Join,
        Order,
        Having,
        Where
    }

    public enum SqlJoinTypes
    {
        Inner,
        Outer,
        Left,
        Right
    }

    public enum ExpressionOptions
    {
        Add,
        Overwrite
    }

    public enum OrderOptions
    {
        Asc,
        Desc
    }

    public enum CompareOperations
    {
        Equals,
        NotEquals,
        In,
        NotIn,
        InNoBraces,
        NotInNoBraces,
        Is,
        IsNot,
        GreaterThan,
        GreaterThanEqual,
        LowerThan,
        LowerThanEqual,
        Like
    }
}