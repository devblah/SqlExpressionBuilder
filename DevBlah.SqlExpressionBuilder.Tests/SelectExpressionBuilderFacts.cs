using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.Meta.Conditions;
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

            var param = new ParameterExpression("@foo", DbType.Int32) { Value = 5 };
            builder.Where("t.foo = @foo", new[] { param });

            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(param.ParameterName, builder.Parameters.First().ParameterName);

            // Rebind 
            var newParam = new SqlParameter("@foo", SqlDbType.Int) { Value = 6 };
            builder.BindParameter(newParam);
            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(newParam.ParameterName, builder.Parameters.First().ParameterName);
            Assert.Equal(6, builder.Parameters.First().Value);

            // Rebind
            builder.BindParameter("@foo", 7);
            Assert.Equal(1, builder.Parameters.Count());
            Assert.Equal(newParam.ParameterName, builder.Parameters.First().ParameterName);
            Assert.Equal(7, builder.Parameters.First().Value);
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
            var param = new ParameterExpression("@bla", DbType.String) { Value = 5 };

            builder.From(fromTable);
            builder.Where(fromTable.GetColumn("col"), param);

            builder.FillCommand(command);

            Assert.Equal(1, command.Parameters.Count);

            // TODO adjust
            //Assert.Equal(param, command.Parameters.Cast<SqlParameter>().First());

            Assert.Equal(builder.ToString(), command.CommandText);
        }

        [Fact]
        public void FillCommand_ThrowsIfIncompleteFact()
        {
            var command = new SqlCommand();

            var builder = new MsSqlSelectExpressionBuilder();
            var fromTable = new Table("table", "t");
            var param = new ParameterExpression("@bla", DbType.String);

            builder.From(fromTable);
            builder.Where(fromTable.GetColumn("col"), param);

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
            builder.JoinLeft(joinTable.GetColumn("fromId"), fromTable.GetColumn("Id"));
            builder.Group(new[] { "t.Id" });

            const string expected =
                "SELECT t.Id, COUNT(j.moo) AS count " +
                "FROM table t " +
                "LEFT JOIN join j ON (j.fromId = t.Id) " +
                "GROUP BY t.Id";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void JoinInnerQueryFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            // FROM
            var fromTable = new Table("dbo.from", "f");
            builder.From(fromTable);

            // JOIN 1
            var joinTable1 = new Table("dbo.join1", "j1");
            builder.JoinInner(joinTable1, "j1.f_Id = f.Id");

            // JOIN 2
            builder.JoinInner("dbo.join2", "j2", "j2.j1_Id = j1.Id");

            // JOIN 3
            var joinTable3 = new Table("dbo.join3", "j3");
            builder.JoinInner(joinTable3, "j3.j2_Id = j2.Id", new[] { "col1", "col2", "col3" });

            // JOIN 4
            var joinTable4 = new Table("dbo.join4", "j4");
            builder.JoinInner(
                joinTable4,
                "j4.j3_Id = j3.Id AND j4.col1 = @param1",
                new List<ParameterExpression> { new ParameterExpression { ParameterName = "@param1" } });

            // JOIN 5
            var joinTable5 = new Table("dbo.join5", "j5");
            builder.JoinInner(
                joinTable5,
                "j5.j4_Id = j4.Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new ParameterExpression { ParameterName = "@param2" } });


            // JOIN 6
            var joinTable6 = new Table("dbo.join6", "j6");
            builder.JoinInner(joinTable6, new ConditionSet
            {
                new SimpleSubCondition("j6.Year > j5.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable6.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable5.GetColumn("foo"))
            });

            // JOIN 7
            var joinTable7 = new Table("dbo.join7", "j7");
            builder.JoinInner(joinTable7, new ConditionSet
            {
                new SimpleSubCondition("j7.Year > j6.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable7.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable6.GetColumn("foo"))
            }, new[] { "col1" });

            // JOIN 8
            var joinTable8 = new Table("dbo.join8", "j8");
            builder.JoinInner(joinTable8, new Expression("j8.bla"), CompareOperations.Is, Expression.Null);

            // JOIN 9
            var joinTable9 = new Table("dbo.join9", "j9");
            builder.JoinInner(joinTable9.GetColumn("foo"), CompareOperations.IsNot, Expression.Null);

            // JOIN 10
            var joinTableA = new Table("dbo.joinA", "jA");
            builder.JoinInner(joinTableA, new Expression("jA.j9_Id"), joinTable9.GetColumn("Id"));

            // JOIN 11
            var joinTableB = new Table("dbo.joinB", "jB");
            builder.JoinInner(joinTableB.GetColumn("jA_Id"), joinTableA.GetColumn("Id"));

            // JOIN 12
            var joinTableC = new Table("dbo.joinC", "jC");
            builder.JoinInner(joinTableC, new Expression("jC.bla"), CompareOperations.Is, Expression.Null,
                new[] { "col4" });

            // JOIN 13
            var joinTableD = new Table("dbo.joinD", "jD");
            builder.JoinInner(joinTableD.GetColumn("foo"), CompareOperations.IsNot, Expression.Null, new[] { "col8" });

            // JOIN 14
            var joinTableE = new Table("dbo.joinE", "jE");
            builder.JoinInner(joinTableE, new Expression("jE.jD_Id"), joinTableD.GetColumn("Id"), new[] { "col2" });

            // JOIN 15
            var joinTableF = new Table("dbo.joinF", "jF");
            builder.JoinInner(joinTableF.GetColumn("jE_Id"), joinTableE.GetColumn("Id"), new[] { "col4" });

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, jC.col4, jD.col8, jE.col2, jF.col4 " +
                "FROM dbo.from f " +
                "INNER JOIN dbo.join1 j1 ON (j1.f_Id = f.Id) " +
                "INNER JOIN dbo.join2 j2 ON (j2.j1_Id = j1.Id) " +
                "INNER JOIN dbo.join3 j3 ON (j3.j2_Id = j2.Id) " +
                "INNER JOIN dbo.join4 j4 ON (j4.j3_Id = j3.Id AND j4.col1 = @param1) " +
                "INNER JOIN dbo.join5 j5 ON (j5.j4_Id = j4.Id AND j5.col1 = @param2) " +
                "INNER JOIN dbo.join6 j6 ON (j6.Year > j5.Year OR j6.foo IS NOT j5.foo) " +
                "INNER JOIN dbo.join7 j7 ON (j7.Year > j6.Year OR j7.foo IS NOT j6.foo) " +
                "INNER JOIN dbo.join8 j8 ON (j8.bla IS NULL) " +
                "INNER JOIN dbo.join9 j9 ON (j9.foo IS NOT NULL) " +
                "INNER JOIN dbo.joinA jA ON (jA.j9_Id = j9.Id) " +
                "INNER JOIN dbo.joinB jB ON (jB.jA_Id = jA.Id) " +
                "INNER JOIN dbo.joinC jC ON (jC.bla IS NULL) " +
                "INNER JOIN dbo.joinD jD ON (jD.foo IS NOT NULL) " +
                "INNER JOIN dbo.joinE jE ON (jE.jD_Id = jD.Id) " +
                "INNER JOIN dbo.joinF jF ON (jF.jE_Id = jE.Id)";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinLeftQueryFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            // FROM
            var fromTable = new Table("dbo.from", "f");
            builder.From(fromTable);

            // JOIN 1
            var joinTable1 = new Table("dbo.join1", "j1");
            builder.JoinLeft(joinTable1, "j1.f_Id = f.Id");

            // JOIN 2
            builder.JoinLeft("dbo.join2", "j2", "j2.j1_Id = j1.Id");

            // JOIN 3
            var joinTable3 = new Table("dbo.join3", "j3");
            builder.JoinLeft(joinTable3, "j3.j2_Id = j2.Id", new[] { "col1", "col2", "col3" });

            // JOIN 4
            var joinTable4 = new Table("dbo.join4", "j4");
            builder.JoinLeft(
                joinTable4,
                "j4.j3_Id = j3.Id AND j4.col1 = @param1",
                new List<ParameterExpression> { new ParameterExpression { ParameterName = "@param1" } });

            // JOIN 5
            var joinTable5 = new Table("dbo.join5", "j5");
            builder.JoinLeft(
                joinTable5,
                "j5.j4_Id = j4.Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new ParameterExpression { ParameterName = "@param2" } });


            // JOIN 6
            var joinTable6 = new Table("dbo.join6", "j6");
            builder.JoinLeft(joinTable6, new ConditionSet
            {
                new SimpleSubCondition("j6.Year > j5.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable6.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable5.GetColumn("foo"))
            });

            // JOIN 7
            var joinTable7 = new Table("dbo.join7", "j7");
            builder.JoinLeft(joinTable7, new ConditionSet
            {
                new SimpleSubCondition("j7.Year > j6.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable7.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable6.GetColumn("foo"))
            }, new[] { "col1" });

            // JOIN 8
            var joinTable8 = new Table("dbo.join8", "j8");
            builder.JoinLeft(joinTable8, new Expression("j8.bla"), CompareOperations.Is, Expression.Null);

            // JOIN 9
            var joinTable9 = new Table("dbo.join9", "j9");
            builder.JoinLeft(joinTable9.GetColumn("foo"), CompareOperations.IsNot, Expression.Null);

            // JOIN 10
            var joinTableA = new Table("dbo.joinA", "jA");
            builder.JoinLeft(joinTableA, new Expression("jA.j9_Id"), joinTable9.GetColumn("Id"));

            // JOIN 11
            var joinTableB = new Table("dbo.joinB", "jB");
            builder.JoinLeft(joinTableB.GetColumn("jA_Id"), joinTableA.GetColumn("Id"));

            // JOIN 12
            var joinTableC = new Table("dbo.joinC", "jC");
            builder.JoinLeft(joinTableC, new Expression("jC.bla"), CompareOperations.Is, Expression.Null,
                new[] { "col4" });

            // JOIN 13
            var joinTableD = new Table("dbo.joinD", "jD");
            builder.JoinLeft(joinTableD.GetColumn("foo"), CompareOperations.IsNot, Expression.Null, new[] { "col8" });

            // JOIN 14
            var joinTableE = new Table("dbo.joinE", "jE");
            builder.JoinLeft(joinTableE, new Expression("jE.jD_Id"), joinTableD.GetColumn("Id"), new[] { "col2" });

            // JOIN 15
            var joinTableF = new Table("dbo.joinF", "jF");
            builder.JoinLeft(joinTableF.GetColumn("jE_Id"), joinTableE.GetColumn("Id"), new[] { "col4" });

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, jC.col4, jD.col8, jE.col2, jF.col4 " +
                "FROM dbo.from f " +
                "LEFT JOIN dbo.join1 j1 ON (j1.f_Id = f.Id) " +
                "LEFT JOIN dbo.join2 j2 ON (j2.j1_Id = j1.Id) " +
                "LEFT JOIN dbo.join3 j3 ON (j3.j2_Id = j2.Id) " +
                "LEFT JOIN dbo.join4 j4 ON (j4.j3_Id = j3.Id AND j4.col1 = @param1) " +
                "LEFT JOIN dbo.join5 j5 ON (j5.j4_Id = j4.Id AND j5.col1 = @param2) " +
                "LEFT JOIN dbo.join6 j6 ON (j6.Year > j5.Year OR j6.foo IS NOT j5.foo) " +
                "LEFT JOIN dbo.join7 j7 ON (j7.Year > j6.Year OR j7.foo IS NOT j6.foo) " +
                "LEFT JOIN dbo.join8 j8 ON (j8.bla IS NULL) " +
                "LEFT JOIN dbo.join9 j9 ON (j9.foo IS NOT NULL) " +
                "LEFT JOIN dbo.joinA jA ON (jA.j9_Id = j9.Id) " +
                "LEFT JOIN dbo.joinB jB ON (jB.jA_Id = jA.Id) " +
                "LEFT JOIN dbo.joinC jC ON (jC.bla IS NULL) " +
                "LEFT JOIN dbo.joinD jD ON (jD.foo IS NOT NULL) " +
                "LEFT JOIN dbo.joinE jE ON (jE.jD_Id = jD.Id) " +
                "LEFT JOIN dbo.joinF jF ON (jF.jE_Id = jE.Id)";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinOuterQueryFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            // FROM
            var fromTable = new Table("dbo.from", "f");
            builder.From(fromTable);

            // JOIN 1
            var joinTable1 = new Table("dbo.join1", "j1");
            builder.JoinOuter(joinTable1, "j1.f_Id = f.Id");

            // JOIN 2
            builder.JoinOuter("dbo.join2", "j2", "j2.j1_Id = j1.Id");

            // JOIN 3
            var joinTable3 = new Table("dbo.join3", "j3");
            builder.JoinOuter(joinTable3, "j3.j2_Id = j2.Id", new[] { "col1", "col2", "col3" });

            // JOIN 4
            var joinTable4 = new Table("dbo.join4", "j4");
            builder.JoinOuter(
                joinTable4,
                "j4.j3_Id = j3.Id AND j4.col1 = @param1",
                new List<ParameterExpression> { new ParameterExpression { ParameterName = "@param1" } });

            // JOIN 5
            var joinTable5 = new Table("dbo.join5", "j5");
            builder.JoinOuter(
                joinTable5,
                "j5.j4_Id = j4.Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new ParameterExpression { ParameterName = "@param2" } });


            // JOIN 6
            var joinTable6 = new Table("dbo.join6", "j6");
            builder.JoinOuter(joinTable6, new ConditionSet
            {
                new SimpleSubCondition("j6.Year > j5.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable6.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable5.GetColumn("foo"))
            });

            // JOIN 7
            var joinTable7 = new Table("dbo.join7", "j7");
            builder.JoinOuter(joinTable7, new ConditionSet
            {
                new SimpleSubCondition("j7.Year > j6.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable7.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable6.GetColumn("foo"))
            }, new[] { "col1" });

            // JOIN 8
            var joinTable8 = new Table("dbo.join8", "j8");
            builder.JoinOuter(joinTable8, new Expression("j8.bla"), CompareOperations.Is, Expression.Null);

            // JOIN 9
            var joinTable9 = new Table("dbo.join9", "j9");
            builder.JoinOuter(joinTable9.GetColumn("foo"), CompareOperations.IsNot, Expression.Null);

            // JOIN 10
            var joinTableA = new Table("dbo.joinA", "jA");
            builder.JoinOuter(joinTableA, new Expression("jA.j9_Id"), joinTable9.GetColumn("Id"));

            // JOIN 11
            var joinTableB = new Table("dbo.joinB", "jB");
            builder.JoinOuter(joinTableB.GetColumn("jA_Id"), joinTableA.GetColumn("Id"));

            // JOIN 12
            var joinTableC = new Table("dbo.joinC", "jC");
            builder.JoinOuter(joinTableC, new Expression("jC.bla"), CompareOperations.Is, Expression.Null,
                new[] { "col4" });

            // JOIN 13
            var joinTableD = new Table("dbo.joinD", "jD");
            builder.JoinOuter(joinTableD.GetColumn("foo"), CompareOperations.IsNot, Expression.Null, new[] { "col8" });

            // JOIN 14
            var joinTableE = new Table("dbo.joinE", "jE");
            builder.JoinOuter(joinTableE, new Expression("jE.jD_Id"), joinTableD.GetColumn("Id"), new[] { "col2" });

            // JOIN 15
            var joinTableF = new Table("dbo.joinF", "jF");
            builder.JoinOuter(joinTableF.GetColumn("jE_Id"), joinTableE.GetColumn("Id"), new[] { "col4" });

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, jC.col4, jD.col8, jE.col2, jF.col4 " +
                "FROM dbo.from f " +
                "OUTER JOIN dbo.join1 j1 ON (j1.f_Id = f.Id) " +
                "OUTER JOIN dbo.join2 j2 ON (j2.j1_Id = j1.Id) " +
                "OUTER JOIN dbo.join3 j3 ON (j3.j2_Id = j2.Id) " +
                "OUTER JOIN dbo.join4 j4 ON (j4.j3_Id = j3.Id AND j4.col1 = @param1) " +
                "OUTER JOIN dbo.join5 j5 ON (j5.j4_Id = j4.Id AND j5.col1 = @param2) " +
                "OUTER JOIN dbo.join6 j6 ON (j6.Year > j5.Year OR j6.foo IS NOT j5.foo) " +
                "OUTER JOIN dbo.join7 j7 ON (j7.Year > j6.Year OR j7.foo IS NOT j6.foo) " +
                "OUTER JOIN dbo.join8 j8 ON (j8.bla IS NULL) " +
                "OUTER JOIN dbo.join9 j9 ON (j9.foo IS NOT NULL) " +
                "OUTER JOIN dbo.joinA jA ON (jA.j9_Id = j9.Id) " +
                "OUTER JOIN dbo.joinB jB ON (jB.jA_Id = jA.Id) " +
                "OUTER JOIN dbo.joinC jC ON (jC.bla IS NULL) " +
                "OUTER JOIN dbo.joinD jD ON (jD.foo IS NOT NULL) " +
                "OUTER JOIN dbo.joinE jE ON (jE.jD_Id = jD.Id) " +
                "OUTER JOIN dbo.joinF jF ON (jF.jE_Id = jE.Id)";

            string actual = builder.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void JoinRightQueryFact()
        {
            var builder = new MsSqlSelectExpressionBuilder();

            // FROM
            var fromTable = new Table("dbo.from", "f");
            builder.From(fromTable);

            // JOIN 1
            var joinTable1 = new Table("dbo.join1", "j1");
            builder.JoinRight(joinTable1, "j1.f_Id = f.Id");

            // JOIN 2
            builder.JoinRight("dbo.join2", "j2", "j2.j1_Id = j1.Id");

            // JOIN 3
            var joinTable3 = new Table("dbo.join3", "j3");
            builder.JoinRight(joinTable3, "j3.j2_Id = j2.Id", new[] { "col1", "col2", "col3" });

            // JOIN 4
            var joinTable4 = new Table("dbo.join4", "j4");
            builder.JoinRight(
                joinTable4,
                "j4.j3_Id = j3.Id AND j4.col1 = @param1",
                new List<ParameterExpression> { new ParameterExpression { ParameterName = "@param1" } });

            // JOIN 5
            var joinTable5 = new Table("dbo.join5", "j5");
            builder.JoinRight(
                joinTable5,
                "j5.j4_Id = j4.Id AND j5.col1 = @param2",
                new[] { "col1", "col2" },
                new[] { new ParameterExpression { ParameterName = "@param2" } });


            // JOIN 6
            var joinTable6 = new Table("dbo.join6", "j6");
            builder.JoinRight(joinTable6, new ConditionSet
            {
                new SimpleSubCondition("j6.Year > j5.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable6.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable5.GetColumn("foo"))
            });

            // JOIN 7
            var joinTable7 = new Table("dbo.join7", "j7");
            builder.JoinRight(joinTable7, new ConditionSet
            {
                new SimpleSubCondition("j7.Year > j6.Year"),
                new ExpressionSubCondition(
                    ConnectOperations.Or,
                    joinTable7.GetColumn("foo"),
                    CompareOperations.IsNot,
                    joinTable6.GetColumn("foo"))
            }, new[] { "col1" });

            // JOIN 8
            var joinTable8 = new Table("dbo.join8", "j8");
            builder.JoinRight(joinTable8, new Expression("j8.bla"), CompareOperations.Is, Expression.Null);

            // JOIN 9
            var joinTable9 = new Table("dbo.join9", "j9");
            builder.JoinRight(joinTable9.GetColumn("foo"), CompareOperations.IsNot, Expression.Null);

            // JOIN 10
            var joinTableA = new Table("dbo.joinA", "jA");
            builder.JoinRight(joinTableA, new Expression("jA.j9_Id"), joinTable9.GetColumn("Id"));

            // JOIN 11
            var joinTableB = new Table("dbo.joinB", "jB");
            builder.JoinRight(joinTableB.GetColumn("jA_Id"), joinTableA.GetColumn("Id"));

            // JOIN 12
            var joinTableC = new Table("dbo.joinC", "jC");
            builder.JoinRight(joinTableC, new Expression("jC.bla"), CompareOperations.Is, Expression.Null,
                new[] { "col4" });

            // JOIN 13
            var joinTableD = new Table("dbo.joinD", "jD");
            builder.JoinRight(joinTableD.GetColumn("foo"), CompareOperations.IsNot, Expression.Null, new[] { "col8" });

            // JOIN 14
            var joinTableE = new Table("dbo.joinE", "jE");
            builder.JoinRight(joinTableE, new Expression("jE.jD_Id"), joinTableD.GetColumn("Id"), new[] { "col2" });

            // JOIN 15
            var joinTableF = new Table("dbo.joinF", "jF");
            builder.JoinRight(joinTableF.GetColumn("jE_Id"), joinTableE.GetColumn("Id"), new[] { "col4" });

            const string expected =
                "SELECT j3.col1, j3.col2, j3.col3, j5.col1, j5.col2, j7.col1, jC.col4, jD.col8, jE.col2, jF.col4 " +
                "FROM dbo.from f " +
                "RIGHT JOIN dbo.join1 j1 ON (j1.f_Id = f.Id) " +
                "RIGHT JOIN dbo.join2 j2 ON (j2.j1_Id = j1.Id) " +
                "RIGHT JOIN dbo.join3 j3 ON (j3.j2_Id = j2.Id) " +
                "RIGHT JOIN dbo.join4 j4 ON (j4.j3_Id = j3.Id AND j4.col1 = @param1) " +
                "RIGHT JOIN dbo.join5 j5 ON (j5.j4_Id = j4.Id AND j5.col1 = @param2) " +
                "RIGHT JOIN dbo.join6 j6 ON (j6.Year > j5.Year OR j6.foo IS NOT j5.foo) " +
                "RIGHT JOIN dbo.join7 j7 ON (j7.Year > j6.Year OR j7.foo IS NOT j6.foo) " +
                "RIGHT JOIN dbo.join8 j8 ON (j8.bla IS NULL) " +
                "RIGHT JOIN dbo.join9 j9 ON (j9.foo IS NOT NULL) " +
                "RIGHT JOIN dbo.joinA jA ON (jA.j9_Id = j9.Id) " +
                "RIGHT JOIN dbo.joinB jB ON (jB.jA_Id = jA.Id) " +
                "RIGHT JOIN dbo.joinC jC ON (jC.bla IS NULL) " +
                "RIGHT JOIN dbo.joinD jD ON (jD.foo IS NOT NULL) " +
                "RIGHT JOIN dbo.joinE jE ON (jE.jD_Id = jD.Id) " +
                "RIGHT JOIN dbo.joinF jF ON (jF.jE_Id = jE.Id)";

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
            expectedTemplate += "INNER JOIN dbo.join j ON (wayne = wayne) ";
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
                "SELECT f.muh, f.maeh, j.muh, j.maeh, j.foo FROM dbo.from f LEFT JOIN dbo.join j ON (was = wer)",
                builder.ToString());

            builder.Select(new[] { "foo", "bar" }, joinTable, ExpressionOptions.Overwrite);
            Assert.Equal(
                "SELECT j.foo, j.bar FROM dbo.from f LEFT JOIN dbo.join j ON (was = wer)", builder.ToString());

            builder.Select(new AliasedExpression<Expression>(new Expression("CONCAT(Street, Number)"), "Address"));
            Assert.Equal(
                "SELECT j.foo, j.bar, CONCAT(Street, Number) AS Address " +
                    "FROM dbo.from f LEFT JOIN dbo.join j ON (was = wer)",
                builder.ToString());

            builder.Distinct();
            Assert.Equal(
                "SELECT DISTINCT j.foo, j.bar, CONCAT(Street, Number) AS Address " +
                    "FROM dbo.from f LEFT JOIN dbo.join j ON (was = wer)",
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
                .Where(new Expression(addressCol.Alias), "@address");
            Assert.Equal(
                "SELECT CONCAT(Street, Number) AS Address, CONCAT(Zip, City) AS ZipCity FROM dbo.from f " +
                    "WHERE (Address = @address)", builder.ToString());
        }

        [Fact]
        public void WhereQuery_ComparerFact()
        {
            var fromTable = new Table("dbo.from", "f");
            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);
            builder.Where(fromTable.GetColumn("foo"), "@foo");
            builder.Where(fromTable.GetColumn("baz"), Expression.Null, CompareOperations.IsNot);
            builder.Where(fromTable.GetColumn("moo"),
                new Expression("(SELECT value FROM options o WHERE key = 'mooKey')"));

            Assert.Equal("SELECT * FROM dbo.from f WHERE (f.foo = @foo AND f.baz IS NOT NULL " +
                "AND f.moo = (SELECT value FROM options o WHERE key = 'mooKey'))",
                builder.ToString());
        }

        [Fact]
        public void WhereQuery_SimpleFact()
        {
            var fromTable = new Table("dbo.from", "f");

            var builder = new MsSqlSelectExpressionBuilder();
            builder.From(fromTable);
            builder.Where("bla = @bla OR blubb = @bla");
            Assert.Equal("SELECT * FROM dbo.from f WHERE (bla = @bla OR blubb = @bla)", builder.ToString());
        }

        //[Fact]
        //public void WhereQuery_SimpleWithParameterFact()
        //{
        //    var fromTable = new Table("dbo.from", "f");
        //    var builder = new MsSqlSelectExpressionBuilder();
        //    builder.From(fromTable);
        //    builder.Where("bla = @bla OR blubb = @bla", new[] { new DbParameterProxy("@bla") });
        //    Assert.Equal("SELECT * FROM dbo.from f WHERE bla = @bla OR blubb = @bla", builder.ToString());
        //    Assert.Equal(1, builder.Parameters.Count());
        //}
    }
}