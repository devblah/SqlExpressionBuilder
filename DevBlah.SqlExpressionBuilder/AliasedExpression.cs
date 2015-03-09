using System;

namespace DevBlah.SqlExpressionBuilder
{
    public class AliasedExpression<T> : IExpression where T : IExpression
    {
        public T Expression { get; private set; }

        public string Alias { get; private set; }

        public AliasedExpression(T expr, string alias)
        {
            Alias = alias;
            Expression = expr;
        }

        public override string ToString()
        {
            return String.Format("{0} AS {1}", Expression.ToString(), Alias);
        }
    }
}