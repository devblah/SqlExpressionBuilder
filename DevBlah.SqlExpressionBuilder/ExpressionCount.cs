using System;

namespace DevBlah.SqlExpressionBuilder
{
    public class ExpressionCount : IExpression
    {
        public ExpressionColumn Column { get; private set; }

        public ExpressionCount(ExpressionColumn column)
        {
            Column = column;
        }

        public override string ToString()
        {
            return String.Format("COUNT({0})", Column);
        }
    }
}