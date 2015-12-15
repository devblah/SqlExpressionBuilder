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
    public class DeleteExpressionBuilderFacts
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
            var builder = new MsSqlDeleteExpressionBuilder("dbo.Test");

            builder.Where("Id = @Id");

            const string expected = "DELETE FROM dbo.Test WHERE (Id = @Id)";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void GetParameters()
        {
            var builder = new MsSqlDeleteExpressionBuilder("dbo.Test");

            builder.Where("Id = @Id", new ParameterExpression("@Id", DbType.Int32) { Value = 5 });

            List<SqlParameter> parameters = builder.Parameters.ToList();

            Assert.Equal(1, parameters.Count);

            Assert.Equal("@Id", parameters[0].ParameterName);
            Assert.Equal(5, parameters[0].Value);


        }
    }
}
