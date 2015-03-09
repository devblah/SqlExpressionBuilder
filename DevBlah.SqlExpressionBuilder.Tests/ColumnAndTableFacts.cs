using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class ColumnAndTableFacts
    {
        [Fact]
        public void TestTableCreationAndToString()
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
        public void TestColumnCreationAndToString()
        {
            var table = new Table("table", "t");
            var column = new ExpressionColumn("col1", table);
            Assert.Equal("t.col1", column.ToString());
        }
    }
}