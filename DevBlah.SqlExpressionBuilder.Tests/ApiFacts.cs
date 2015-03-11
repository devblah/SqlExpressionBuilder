using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;
using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class ApiFacts
    {
        [Fact]
        public void TestSimpleSelectFrom()
        {
            var fromTable = new Table("from", "f");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);

            Assert.Equal("SELECT * FROM from f", builder.ToString());

            var expr = new AliasedExpression<CountExpression>(
                new CountExpression(new ColumnExpression("Id", fromTable)), "Anzahl");
            Assert.Equal("SELECT COUNT(f.Id) AS Anzahl FROM from f", builder.GetSingleSelectString(expr));
            Assert.Equal("SELECT * FROM from f", builder.ToString());
        }

        [Fact]
        public void TestMultipleFromQuery()
        {
            var fromTable1 = new Table("from1", "f1");
            var fromTable2 = new Table("from2", "f2");

            var builder = new SqlExpressionBuilderSelect();
            builder
                .From(fromTable1)
                .From(fromTable2, new[] { "col1", "col2", "col3" })
                .From("from3", "f3")
                .From("from4");
            Assert.Equal(
                "SELECT f2.col1, f2.col2, f2.col3 FROM from1 f1, from2 f2, from3 f3, from4 from4",
                builder.ToString());
        }

        [Fact]
        public void TestfJoinReferencedTableExistsCheck()
        {
            bool thrown = false;

            var fromTable = new Table("from", "f");
            var joinTable1 = new Table("join1", "j1");
            var joinTable2 = new Table("join2", "j2");

            // Works
            var builder = new SqlExpressionBuilderSelect();
            try
            {
                builder
                    .From(fromTable)
                    .JoinInner(
                        new Compare<ColumnExpression, ColumnExpression>(
                            new ColumnExpression("bla", fromTable),
                            new ColumnExpression("bla", joinTable1)));
            }
            catch
            {
                thrown = true;
            }
            Assert.False(thrown);

            // Fails
            thrown = false;
            builder = new SqlExpressionBuilderSelect();
            try
            {
                builder
                    .From(fromTable)
                    .JoinInner(
                        new Compare<ColumnExpression, ColumnExpression>(
                            new ColumnExpression("bla", joinTable1),     // first table has to be the referenced table
                            new ColumnExpression("bla", fromTable)));
            }
            catch
            {
                thrown = true;
            }
            Assert.True(thrown);

            // Works
            thrown = false;
            builder = new SqlExpressionBuilderSelect();
            try
            {
                builder
                    .From(fromTable)
                    .JoinInner(
                        new Compare<ColumnExpression, ColumnExpression>(
                            new ColumnExpression("bla", fromTable),
                            new ColumnExpression("bla", joinTable1)))
                    .JoinInner(
                        new Compare<ColumnExpression, ColumnExpression>(
                            new ColumnExpression("bla", joinTable1),
                            new ColumnExpression("bla", joinTable2)));
            }
            catch
            {
                thrown = true;
            }
            Assert.False(thrown);
        }

        [Fact]
        public void TestJoinInnerQuery()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.JoinInner(joinTable1, "f.Id = j1.f_Id");
            builder.JoinInner("dbo.join2", "j2", "j1.Id = j2.j1_Id");
            builder.JoinInner(joinTable3, "j2.Id = j3.j2_Id", new[] { "col1", "col2", "col3" });
            builder.JoinInner(
                joinTable4,
                "j3.Id = j4.j3_Id AND j4.col1 = @param1",
                new List<SqlParameter> { new SqlParameter { ParameterName = "@param1" } });
            builder.JoinInner(
                joinTable5,
                "j4.Id = j5.j4_Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new SqlParameter { ParameterName = "@param2" } });
            builder.JoinInner(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable5),
                    new ColumnExpression("j5_Id", joinTable6)));
            builder.JoinInner(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable6),
                    new ColumnExpression("j6_Id", joinTable7)),
                new[] { "col1", "col2" });

            const string expected = "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2 " +
                                    "FROM dbo.from f " +
                                    "INNER JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                                    "INNER JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                                    "INNER JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                                    "INNER JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                                    "INNER JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                                    "INNER JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                                    "INNER JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestJoinLeftQuery()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.JoinLeft(joinTable1, "f.Id = j1.f_Id");
            builder.JoinLeft("dbo.join2", "j2", "j1.Id = j2.j1_Id");
            builder.JoinLeft(joinTable3, "j2.Id = j3.j2_Id", new[] { "col1", "col2", "col3" });
            builder.JoinLeft(
                joinTable4,
                "j3.Id = j4.j3_Id AND j4.col1 = @param1",
                new List<SqlParameter> { new SqlParameter { ParameterName = "@param1" } });
            builder.JoinLeft(
                joinTable5,
                "j4.Id = j5.j4_Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new SqlParameter { ParameterName = "@param2" } });
            builder.JoinLeft(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable5),
                    new ColumnExpression("j5_Id", joinTable6)));
            builder.JoinLeft(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable6),
                    new ColumnExpression("j6_Id", joinTable7)),
                new[] { "col1", "col2" });

            const string expected = "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2 " +
                                    "FROM dbo.from f " +
                                    "LEFT JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                                    "LEFT JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                                    "LEFT JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                                    "LEFT JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                                    "LEFT JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                                    "LEFT JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                                    "LEFT JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestJoinOuterQuery()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.JoinOuter(joinTable1, "f.Id = j1.f_Id");
            builder.JoinOuter("dbo.join2", "j2", "j1.Id = j2.j1_Id");
            builder.JoinOuter(joinTable3, "j2.Id = j3.j2_Id", new[] { "col1", "col2", "col3" });
            builder.JoinOuter(
                joinTable4,
                "j3.Id = j4.j3_Id AND j4.col1 = @param1",
                new List<SqlParameter> { new SqlParameter { ParameterName = "@param1" } });
            builder.JoinOuter(
                joinTable5,
                "j4.Id = j5.j4_Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new SqlParameter { ParameterName = "@param2" } });
            builder.JoinOuter(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable5),
                    new ColumnExpression("j5_Id", joinTable6)));
            builder.JoinOuter(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable6),
                    new ColumnExpression("j6_Id", joinTable7)),
                new[] { "col1", "col2" });

            const string expected = "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2 " +
                                    "FROM dbo.from f " +
                                    "OUTER JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                                    "OUTER JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                                    "OUTER JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                                    "OUTER JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                                    "OUTER JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                                    "OUTER JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                                    "OUTER JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestJoinRightQuery()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.JoinRight(joinTable1, "f.Id = j1.f_Id");
            builder.JoinRight("dbo.join2", "j2", "j1.Id = j2.j1_Id");
            builder.JoinRight(joinTable3, "j2.Id = j3.j2_Id", new[] { "col1", "col2", "col3" });
            builder.JoinRight(
                joinTable4,
                "j3.Id = j4.j3_Id AND j4.col1 = @param1",
                new List<SqlParameter> { new SqlParameter { ParameterName = "@param1" } });
            builder.JoinRight(
                joinTable5,
                "j4.Id = j5.j4_Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new SqlParameter { ParameterName = "@param2" } });
            builder.JoinRight(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable5),
                    new ColumnExpression("j5_Id", joinTable6)));
            builder.JoinRight(
                new Compare<ColumnExpression, ColumnExpression>(
                    new ColumnExpression("Id", joinTable6),
                    new ColumnExpression("j6_Id", joinTable7)),
                new[] { "col1", "col2" });

            const string expected = "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2 " +
                                    "FROM dbo.from f " +
                                    "RIGHT JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                                    "RIGHT JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                                    "RIGHT JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                                    "RIGHT JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                                    "RIGHT JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                                    "RIGHT JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                                    "RIGHT JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestOrderQuery()
        {
            string expectedTemplate = "SELECT * FROM dbo.from f ";
            var fromTable = new Table("dbo.from", "f");
            var joinTable = new Table("dbo.join", "j");

            var builder = new SqlExpressionBuilderSelect();
            builder
                .From(fromTable)
                .Order("bla")
                .Order("blubb", OrderOptions.Desc);

            string expected = expectedTemplate + "ORDER BY f.bla ASC, f.blubb DESC";
            Assert.Equal(expected, builder.ToString());

            // Fails
            bool throws = false;
            builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable).Order("bla");
            try
            {
                builder.Order("b.blubb", OrderOptions.Desc); // table with alias b not found
            }
            catch
            {
                throws = true;
            }
            Assert.True(throws);

            builder
                .JoinInner(joinTable, "wayne = wayne")
                .Order("j.blubb", OrderOptions.Desc)
                .Order("j.foo", ExpressionOptions.Overwrite)
                .Order(new ColumnExpression("bla", fromTable))
                .Order(new ColumnExpression("muh", fromTable), OrderOptions.Desc);
            expectedTemplate += "INNER JOIN dbo.join j ON wayne = wayne ";
            expected = expectedTemplate + "ORDER BY j.foo ASC, f.bla ASC, f.muh DESC";
            Assert.Equal(expected, builder.ToString());

            builder.Order(new ColumnExpression("bla", fromTable), ExpressionOptions.Overwrite);
            expected = expectedTemplate + "ORDER BY f.bla ASC";
            Assert.Equal(expected, builder.ToString());

            builder.Order(new ColumnExpression("foo", fromTable), OrderOptions.Desc, ExpressionOptions.Overwrite);
            expected = expectedTemplate + "ORDER BY f.foo DESC";
            Assert.Equal(expected, builder.ToString());

            // Fails
            throws = false;
            builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable).Order("bla");
            try
            {
                builder.Order(new ColumnExpression("foo", joinTable)); // table does not exist in sql builder
            }
            catch
            {
                throws = true;
            }
            Assert.True(throws);
        }

        [Fact]
        public void TestSelectStatements()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable = new Table("dbo.join", "j");

            var builder = new SqlExpressionBuilderSelect();
            builder
                .From(fromTable)
                .Select("bla")
                .Select(new[] { "f.blubb", "f.foo" });
            Assert.Equal("SELECT f.bla, f.blubb, f.foo FROM dbo.from f", builder.ToString());

            builder.Select(new[] { "f.muh" }, ExpressionOptions.Overwrite);
            Assert.Equal("SELECT f.muh FROM dbo.from f", builder.ToString());

            builder.Select("maeh", fromTable);
            Assert.Equal("SELECT f.muh, f.maeh FROM dbo.from f", builder.ToString());

            builder.JoinLeft(joinTable, "was = wer");
            builder.Select(new[] { "muh", "maeh", "foo" }, joinTable);
            Assert.Equal(
                "SELECT f.muh, f.maeh, j.muh, j.maeh, j.foo FROM dbo.from f LEFT JOIN dbo.join j ON was = wer",
                builder.ToString());

            builder.Select(new[] { "foo", "bar" }, joinTable, ExpressionOptions.Overwrite);
            Assert.Equal(
                "SELECT j.foo, j.bar FROM dbo.from f LEFT JOIN dbo.join j ON was = wer", builder.ToString());

            builder.Select(new AliasedExpression<Expression>(new Expression("CONCAT(Street, Number)"), "Address"));
            Assert.Equal(
                "SELECT j.foo, j.bar, CONCAT(Street, Number) AS Address " +
                    "FROM dbo.from f LEFT JOIN dbo.join j ON was = wer",
                builder.ToString());
        }

        [Fact]
        public void TestWhereQuery()
        {
            var fromTable = new Table("dbo.from", "f");

            var builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.Where("bla = @bla OR blubb = @bla");
            Assert.Equal("SELECT * FROM dbo.from f WHERE bla = @bla OR blubb = @bla", builder.ToString());
            Assert.Equal(1, builder.Parameters.Count());

            builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.Where("bla = @bla OR blubb = @bla", new[] { new SqlParameter { ParameterName = "@bla" } });
            Assert.Equal("SELECT * FROM dbo.from f WHERE bla = @bla OR blubb = @bla", builder.ToString());
            Assert.Equal(1, builder.Parameters.Count());

            builder = new SqlExpressionBuilderSelect();
            builder.From(fromTable);
            builder.Where(
                new Compare<ColumnExpression, string>(
                    new ColumnExpression("foo", fromTable),
                    "@foo"));
            builder.Where(
                new Compare<ColumnExpression, IDbDataParameter>(
                    new ColumnExpression("bar", fromTable),
                    new SqlParameter { ParameterName = "@bar" }));
            Assert.Equal("SELECT * FROM dbo.from f WHERE f.foo = @foo AND f.bar = @bar", builder.ToString());
            Assert.Equal(2, builder.Parameters.Count());

            var addressCol = new AliasedExpression<Expression>(
                new Expression("CONCAT(Street, Number)"), "Address");
            var zipCityCol = new AliasedExpression<Expression>(
                new Expression("CONCAT(Zip, City)"), "ZipCity");

            builder = new SqlExpressionBuilderSelect();
            builder
                .From(fromTable)
                .Select(addressCol)
                .Select(zipCityCol)
                .Where(
                    new Compare<string, string>(addressCol.Alias, "@address"));
            builder.Where(
                new Compare<string, IDbDataParameter>(zipCityCol.Alias, new SqlParameter { ParameterName = "@zc" }));
            Assert.Equal(
                "SELECT CONCAT(Street, Number) AS Address, CONCAT(Zip, City) AS ZipCity FROM dbo.from f " +
                    "WHERE Address = @address AND ZipCity = @zc", builder.ToString());
            Assert.Equal(2, builder.Parameters.Count());
        }
    }
}