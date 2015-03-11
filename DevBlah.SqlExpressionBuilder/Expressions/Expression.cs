namespace DevBlah.SqlExpressionBuilder.Expressions
{
    /// <summary>
    /// Creates a manual expression for the query
    /// </summary>
    public class Expression : IExpression
    {
        /// <summary>
        /// expression content
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="content">expression content</param>
        public Expression(string content)
        {
            Content = content;
        }

        /// <summary>
        /// creates the actual expression string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Content;
        }
    }
}