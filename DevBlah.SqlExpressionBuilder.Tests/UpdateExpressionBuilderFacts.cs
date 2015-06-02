using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.MsSql;
using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class UpdateExpressionBuilderFacts
    {
        private ColumnSet _columnSet = new ColumnSet
        {
            {"Id", DbType.Int32},
            {"Content", DbType.String, 200},
            {"CreatedAt", DbType.DateTime}
        };

        [Fact]
        public void ToString_CorrectExecution()
        {
            var rowSet = new RowSet(_columnSet) { { "Content", "foo" } };

            var builder = new MsSqlUpdateExpressionBuilder("dbo.Test", rowSet);

            builder.Where("Id = @Id");

            const string expected = "UPDATE dbo.Test SET dbo.Test.Content = @dboTest_Content WHERE (Id = @Id)";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void GetParameters()
        {
            var rowSet = new RowSet(_columnSet) { { "Content", "foo" } };

            var builder = new MsSqlUpdateExpressionBuilder("dbo.Test", rowSet);

            builder.Where("Id = @Id", new ParameterExpression("@Id", DbType.Int32) { Value = 5 });

            List<SqlParameter> parameters = builder.Parameters.ToList();

            Assert.Equal(2, parameters.Count);

            Assert.Equal("@dboTest_Content", parameters[0].ParameterName);
            Assert.Equal(200, parameters[0].Size);
            Assert.Equal("foo", parameters[0].Value);

            Assert.Equal("@Id", parameters[1].ParameterName);
            Assert.Equal(5, parameters[1].Value);

            
        }
    }
}
