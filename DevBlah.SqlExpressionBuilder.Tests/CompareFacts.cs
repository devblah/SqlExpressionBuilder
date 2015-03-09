using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class CompareFacts
    {
        [Fact]
        public void TestComparePlainDatatype()
        {
            var comp = new Compare<string, int>("bla", 1);
            Assert.Equal("bla = 1", comp.ToString());
        }

        [Fact]
        public void TestCompareIEnumerableDatatypes()
        {
            var comp = new Compare<int, int[]>("{0} IN ({1})", 5, new[] { 1, 2, 3, 4, 5, 6 });
            Assert.Equal("5 IN (1, 2, 3, 4, 5, 6)", comp.ToString());
        }
    }
}