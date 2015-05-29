using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class ColumnAndTableFacts
    {
        [Fact]
        public void TableCreationAndToStringFact()
        {
            var table1 = new Table("table");
            var table2 = new Table("table", "t");
            var table3 = new Table("dbo.table1");
            var table4 = new Table("dbo.table2", "t");

            Assert.Equal("table table", table1.ToString());
            Assert.Equal("table t", table2.ToString());
            Assert.Equal("dbo.table1 table1", table3.ToString());
            Assert.Equal("dbo.table2 t", table4.ToString());
        }

        [Fact]
        public void ColumnCreationAndToStringFact()
        {
            var table = new Table("table", "t");
            var column = new ColumnExpression("col1", table);
            Assert.Equal("t.col1", column.ToString());
        }

        [Fact]
        public void TableGetColumnFact()
        {
            var table = new Table("table", "t");
            ColumnExpression col = table.GetColumn("Id");
            Assert.Equal("Id", col.Content);
            Assert.Equal(table, col.Table);

            // check if caching works correctly
            ColumnExpression cached = table.GetColumn("Id");
            Assert.Equal(col, cached);
        }
    }
}