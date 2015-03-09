namespace DevBlah.SqlExpressionBuilder
{
    public class Expression : IExpression
    {
        public string Content { get; private set; }

        public Expression(string content)
        {
            Content = content;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}