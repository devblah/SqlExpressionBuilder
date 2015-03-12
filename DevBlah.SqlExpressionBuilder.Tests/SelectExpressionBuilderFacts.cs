using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.MsSql;
using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class SelectExpressionBuilderFacts
    {
        [Fact]
        public void BindParameter_BindAndRebindParameterFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From("table", "t");
            builder.Where("t.foo = @foo");

            var param = new SqlParameter("@foo", SqlDbType.Int) { Value = 5 };
            builder.BindParameter(param);
            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(param, builder.Parameters.First());

            // Rebind by new param
            var newParam = new SqlParameter("@foo", SqlDbType.Int) { Value = 6 };
            builder.BindParameter(newParam);
            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(newParam, builder.Parameters.First());

            // Rebind by overwrite
            builder.BindParameter("@foo", 7);
            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(newParam, builder.Parameters.First());
            Assert.Equal(7, builder.Parameters.First().Value);
        }

        [Fact]
        public void BindParameter_DoesNotThrowOnParameterFoundFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From("table", "t");

            builder.Where("t.foo = @foo");

            Assert.DoesNotThrow(() => builder.BindParameter("@foo", DbType.Int32, 12));
        }

        [Fact]
        public void BindParameter_DoesNotThrowOnSupressedWarningFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From("table", "t");

            Assert.DoesNotThrow(() => builder.BindParameter("@foo", DbType.Int32, 12, true));
        }

        [Fact]
        public void BindParameter_ThrowsOnParameterNotFoundFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From("table", "t");

            Assert.Throws<InvalidOperationException>(() => builder.BindParameter("@foo", DbType.Int32, 12));

            Assert.Throws<InvalidOperationException>(() => builder.BindParameter("@foo", 12));
        }

        [Fact]
        public void FillCommand()
        {
            var command = new SqlCommand();

            var builder = new MsSqlSelectExpressionBuilder();
            var fromTable = new Table("table", "t");
            var param = new SqlParameter("@bla", SqlDbType.VarChar) { Value = 5 };

            builder.From(fromTable);
            builder.Where("t.col = @bla", new[] { param });

            builder.FillCommand(command);

            Assert.Equal(1, command.Parameters.Count);
            Assert.Equal(param, command.Parameters.Cast<SqlParameter>().First());

            Assert.Equal(builder.ToString(), command.CommandText);
        }

        [Fact]
        public void FillCommand_ThrowsIfIncompleteFact()
        {
            var command = new SqlCommand();

            var builder = new MsSqlSelectExpressionBuilder();
            var fromTable = new Table("table", "t");
            var param = new SqlParameter("@bla", SqlDbType.VarChar);

            builder.From(fromTable);
            builder.Where("t.col = @bla", new[] { param });

            Assert.Throws<Exception>(() => builder.FillCommand(command));
        }

        [Fact]
        public void GetSqlString_ThrowsIfFromStatementMissing()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            Assert.Throws<InvalidOperationException>(() => builder.GetSqlString());
        }

        [Fact]
        public void GroupFacts()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            var fromTable = new Table("table", "t");
            var joinTable = new Table("join", "j");

            builder.From(fromTable);
            builder.Select("Id", fromTable);
            builder.Select(new AliasedExpression<CountExpression>(
                new CountExpression(joinTable.GetColumn("moo")), "count"));
            builder.JoinLeft(fromTable.GetColumn("Id"), joinTable.GetColumn("fromId"));
            builder.Group(new[] { "t.Id" });

            const string expected =
                "SELECT t.Id, COUNT(j.moo) AS count " +
                "FROM table t " +
                "LEFT JOIN join j ON t.Id = j.fromId " +
                "GROUP BY t.Id";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void JoinInnerQueryFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");
            var joinTable8 = new Table("dbo.join8", "j8");
            var joinTable9 = new Table("dbo.join9", "j9");
            var joinTableA = new Table("dbo.joinA", "jA");

            var builder = new MsSqlSelectExpressionBuilder();
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

            builder.JoinInner(joinTable7.GetColumn("Id"), joinTable8.GetColumn("j7_Id"));
            builder.JoinInner(joinTable8.GetColumn("Id"), joinTable9.GetColumn("j8_Id"), new[] { "col1", "col2" });
            builder.JoinInner(joinTable9.GetColumn("Id"), joinTableA.GetColumn("j9_Id"), CompareOperations.Equals);

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2, j9.col1, j9.col2 " +
                "FROM dbo.from f " +
                "INNER JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                "INNER JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                "INNER JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                "INNER JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                "INNER JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                "INNER JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                "INNER JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id " +
                "INNER JOIN dbo.join8 j8 ON j7.Id = j8.j7_Id " +
                "INNER JOIN dbo.join9 j9 ON j8.Id = j9.j8_Id " +
                "INNER JOIN dbo.joinA jA ON j9.Id = jA.j9_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinLeftQueryFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");
            var joinTable8 = new Table("dbo.join8", "j8");
            var joinTable9 = new Table("dbo.join9", "j9");
            var joinTableA = new Table("dbo.joinA", "jA");

            var builder = new MsSqlSelectExpressionBuilder();
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

            builder.JoinLeft(joinTable7.GetColumn("Id"), joinTable8.GetColumn("j7_Id"));
            builder.JoinLeft(joinTable8.GetColumn("Id"), joinTable9.GetColumn("j8_Id"), new[] { "col1", "col2" });
            builder.JoinLeft(joinTable9.GetColumn("Id"), joinTableA.GetColumn("j9_Id"), CompareOperations.Equals);

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2, j9.col1, j9.col2 " +
                "FROM dbo.from f " +
                "LEFT JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                "LEFT JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                "LEFT JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                "LEFT JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                "LEFT JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                "LEFT JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                "LEFT JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id " +
                "LEFT JOIN dbo.join8 j8 ON j7.Id = j8.j7_Id " +
                "LEFT JOIN dbo.join9 j9 ON j8.Id = j9.j8_Id " +
                "LEFT JOIN dbo.joinA jA ON j9.Id = jA.j9_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinOuterQueryFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");
            var joinTable8 = new Table("dbo.join8", "j8");
            var joinTable9 = new Table("dbo.join9", "j9");
            var joinTableA = new Table("dbo.joinA", "jA");

            var builder = new MsSqlSelectExpressionBuilder();
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

            builder.JoinOuter(joinTable7.GetColumn("Id"), joinTable8.GetColumn("j7_Id"));
            builder.JoinOuter(joinTable8.GetColumn("Id"), joinTable9.GetColumn("j8_Id"), new[] { "col1", "col2" });
            builder.JoinOuter(joinTable9.GetColumn("Id"), joinTableA.GetColumn("j9_Id"), CompareOperations.Equals);

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2, j9.col1, j9.col2 " +
                "FROM dbo.from f " +
                "OUTER JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                "OUTER JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                "OUTER JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                "OUTER JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                "OUTER JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                "OUTER JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                "OUTER JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id " +
                "OUTER JOIN dbo.join8 j8 ON j7.Id = j8.j7_Id " +
                "OUTER JOIN dbo.join9 j9 ON j8.Id = j9.j8_Id " +
                "OUTER JOIN dbo.joinA jA ON j9.Id = jA.j9_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinReferencedTableExistsCheck_ExceptionDoesNotThrowFact()
        {
            var fromTable = new Table("from", "f");
            var joinTable1 = new Table("join1", "j1");

            // Works
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);

            Assert.DoesNotThrow(() => builder.JoinInner(fromTable.GetColumn("bla"), joinTable1.GetColumn("bla")));
        }

        [Fact]
        public void JoinReferencedTableExistsCheck_ExceptionDoesNotThrowOnMultipleJoins()
        {
            var fromTable = new Table("from", "f");
            var joinTable1 = new Table("join1", "j1");
            var joinTable2 = new Table("join2", "j2");

            var builder = new MsSqlSelectExpressionBuilder();

            builder.From(fromTable)
                .JoinInner(fromTable.GetColumn("bla"), joinTable1.GetColumn("bla"));

            Assert.DoesNotThrow(() => builder.JoinInner(joinTable1.GetColumn("bla"), joinTable2.GetColumn("bla")));
        }

        [Fact]
        public void JoinReferencedTableExistsCheck_ExceptionThrownOnReferencedTableNotFirstArgumentFact()
        {
            var fromTable = new Table("from", "f");
            var joinTable1 = new Table("join1", "j1");

            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);

            // first table has to be the referenced table
            Assert.Throws<InvalidOperationException>(
                () => builder.JoinInner(joinTable1.GetColumn("bla"), fromTable.GetColumn("bla")));
        }

        [Fact]
        public void JoinRightQueryFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable1 = new Table("dbo.join1", "j1");
            var joinTable3 = new Table("dbo.join3", "j3");
            var joinTable4 = new Table("dbo.join4", "j4");
            var joinTable5 = new Table("dbo.join5", "j5");
            var joinTable6 = new Table("dbo.join6", "j6");
            var joinTable7 = new Table("dbo.join7", "j7");
            var joinTable8 = new Table("dbo.join8", "j8");
            var joinTable9 = new Table("dbo.join9", "j9");
            var joinTableA = new Table("dbo.joinA", "jA");

            var builder = new MsSqlSelectExpressionBuilder();
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

            builder.JoinRight(joinTable7.GetColumn("Id"), joinTable8.GetColumn("j7_Id"));
            builder.JoinRight(joinTable8.GetColumn("Id"), joinTable9.GetColumn("j8_Id"), new[] { "col1", "col2" });
            builder.JoinRight(joinTable9.GetColumn("Id"), joinTableA.GetColumn("j9_Id"), CompareOperations.Equals);

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, j7.col2, j9.col1, j9.col2 " +
                "FROM dbo.from f " +
                "RIGHT JOIN dbo.join1 j1 ON f.Id = j1.f_Id " +
                "RIGHT JOIN dbo.join2 j2 ON j1.Id = j2.j1_Id " +
                "RIGHT JOIN dbo.join3 j3 ON j2.Id = j3.j2_Id " +
                "RIGHT JOIN dbo.join4 j4 ON j3.Id = j4.j3_Id AND j4.col1 = @param1 " +
                "RIGHT JOIN dbo.join5 j5 ON j4.Id = j5.j4_Id AND j5.col1 = @param2 " +
                "RIGHT JOIN dbo.join6 j6 ON j5.Id = j6.j5_Id " +
                "RIGHT JOIN dbo.join7 j7 ON j6.Id = j7.j6_Id " +
                "RIGHT JOIN dbo.join8 j8 ON j7.Id = j8.j7_Id " +
                "RIGHT JOIN dbo.join9 j9 ON j8.Id = j9.j8_Id " +
                "RIGHT JOIN dbo.joinA jA ON j9.Id = jA.j9_Id";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleFromQueryFact()
        {
            var fromTable1 = new Table("from1", "f1");
            var fromTable2 = new Table("from2", "f2");

            var builder = new MsSqlSelectExpressionBuilder();
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
        public void OrderQueryFact()
        {
            string expectedTemplate = "SELECT * FROM dbo.from f ";
            var fromTable = new Table("dbo.from", "f");
            var joinTable = new Table("dbo.join", "j");

            var builder = new MsSqlSelectExpressionBuilder();
            builder
                .From(fromTable)
                .Order("bla")
                .Order("blubb", OrderOptions.Desc);

            string expected = expectedTemplate + "ORDER BY f.bla ASC, f.blubb DESC";
            Assert.Equal(expected, builder.ToString());

            // Fails
            bool throws = false;
            builder = new MsSqlSelectExpressionBuilder();
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
            builder = new MsSqlSelectExpressionBuilder();
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
        public void SelectStatementFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var joinTable = new Table("dbo.join", "j");

            var builder = new MsSqlSelectExpressionBuilder();
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

            builder.Distinct();
            Assert.Equal(
                "SELECT DISTINCT j.foo, j.bar, CONCAT(Street, Number) AS Address " +
                    "FROM dbo.from f LEFT JOIN dbo.join j ON was = wer",
                builder.ToString());
        }

        [Fact]
        public void SelectStatement_ThrowsIfNoDefaultTableSelected()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            Assert.Throws<InvalidOperationException>(() => builder.Select("foo"));
        }

        [Fact]
        public void SelectStatement_ThrowsIfWrongColumnName()
        {
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From("table", "t");

            Assert.Throws<ArgumentException>(() => builder.Select("t.b.foo"));
        }

        [Fact]
        public void SimpleSelectFromFact()
        {
            var fromTable = new Table("from", "f");

            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);

            Assert.Equal("SELECT * FROM from f", builder.ToString());

            var expr = new AliasedExpression<CountExpression>(
                new CountExpression(new ColumnExpression("Id", fromTable)), "Anzahl");
            Assert.Equal("SELECT COUNT(f.Id) AS Anzahl FROM from f", builder.GetSingleSelectString(expr));
            Assert.Equal("SELECT * FROM from f", builder.ToString());
        }

        [Fact]
        public void WhereQuery_AliasedWhereFact()
        {
            var fromTable = new Table("dbo.from", "f");

            var addressCol = new AliasedExpression<Expression>(
                new Expression("CONCAT(Street, Number)"), "Address");
            var zipCityCol = new AliasedExpression<Expression>(
                new Expression("CONCAT(Zip, City)"), "ZipCity");

            var builder = new MsSqlSelectExpressionBuilder();
            builder
                .From(fromTable)
                .Select(addressCol)
                .Select(zipCityCol)
                .Where(
                    new Compare<string, string>(addressCol.Alias, "@address"));
            builder.Where(
                new Compare<string, SqlParameter>(zipCityCol.Alias, new SqlParameter { ParameterName = "@zc" }));
            Assert.Equal(
                "SELECT CONCAT(Street, Number) AS Address, CONCAT(Zip, City) AS ZipCity FROM dbo.from f " +
                    "WHERE Address = @address AND ZipCity = @zc", builder.ToString());
            Assert.Equal(2, builder.Parameters.Count());
        }

        [Fact]
        public void WhereQuery_ComparerFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);
            builder.Where(fromTable.GetColumn("foo"), "@foo");
            builder.Where(fromTable.GetColumn("bar"), new SqlParameter { ParameterName = "@bar" });
            builder.Where(fromTable.GetColumn("baz"), Expression.Null, CompareOperations.IsNot);
            builder.Where(fromTable.GetColumn("moo"),
                new Expression("(SELECT value FROM options o WHERE key = 'mooKey')"));

            Assert.Equal("SELECT * FROM dbo.from f WHERE f.foo = @foo AND f.bar = @bar AND f.baz IS NOT NULL " +
                "AND f.moo = (SELECT value FROM options o WHERE key = 'mooKey')",
                builder.ToString());
            Assert.Equal(2, builder.Parameters.Count());
        }

        [Fact]
        public void WhereQuery_SimpleFact()
        {
            var fromTable = new Table("dbo.from", "f");

            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);
            builder.Where("bla = @bla OR blubb = @bla");
            Assert.Equal("SELECT * FROM dbo.from f WHERE bla = @bla OR blubb = @bla", builder.ToString());
            Assert.Equal(1, builder.Parameters.Count());
        }

        [Fact]
        public void WhereQuery_SimpleWithParameterFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);
            builder.Where("bla = @bla OR blubb = @bla", new[] { new SqlParameter { ParameterName = "@bla" } });
            Assert.Equal("SELECT * FROM dbo.from f WHERE bla = @bla OR blubb = @bla", builder.ToString());
            Assert.Equal(1, builder.Parameters.Count());
        }
    }
}