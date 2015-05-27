using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DevBlah.SqlExpressionBuilder.Meta;
using DevBlah.SqlExpressionBuilder.MsSql;
using Xunit;

namespace DevBlah.SqlExpressionBuilder.Tests
{
    public class InsertExpressionBuilderFact
    {
        private ColumnSet _columnSet = new ColumnSet
        {
            {"Id", DbType.Int32},
            {"Content", DbType.String, 200},
            {"CreatedAt", DbType.DateTime}
        };

        [Fact]
        public void AddingRow_DoesNotThrowOnColumnMissingWhenIgnoring()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet, true);

            var row = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            };

            Assert.DoesNotThrow(() => builder.AddRow(row));
        }

        [Fact]
        public void AddingRow_DoesNotThrowOnIdentical()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            var row = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            };

            Assert.DoesNotThrow(() => builder.AddRow(row));
        }

        [Fact]
        public void AddingRow_DoesThrowOnDifferentRowsWhenIgnoringColumnMissing()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet, true);

            var row = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}
            };

            Assert.DoesNotThrow(() => builder.AddRow(row));

            var nextRow = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            };

            Assert.Throws<InvalidOperationException>(() => builder.AddRow(nextRow));
        }

        [Fact]
        public void AddingRow_ThrowsOnColumnMissing()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            var row = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}
            };

            Assert.Throws<InvalidOperationException>(() => builder.AddRow(row));
        }

        [Fact]
        public void AddingRow_ThrowsOnColumnNotExists()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            var row = new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now},
                {"NotExists", null}
            };

            Assert.Throws<InvalidOperationException>(() => builder.AddRow(row));
        }

        [Fact]
        public void Parameters_GetEmpty()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            List<SqlParameter> parameters = builder.Parameters.ToList();

            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void Parameters_GetCorrectParameters()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            var date = DateTime.Now;

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", date}
            });

            builder.AddRow(new Row
            {
                {"Id", 6}, 
                {"Content", "bar"}, 
                {"CreatedAt", null}
            });

            List<SqlParameter> parameters = builder.Parameters.ToList();

            Assert.Equal(6, parameters.Count);

            Assert.Equal("@Id_0", parameters[0].ParameterName);
            Assert.Equal(5, parameters[0].Value);

            Assert.Equal("@Content_0", parameters[1].ParameterName);
            Assert.Equal("foo", parameters[1].Value);

            Assert.Equal("@CreatedAt_0", parameters[2].ParameterName);
            Assert.Equal(date, parameters[2].Value);

            Assert.Equal("@Id_1", parameters[3].ParameterName);
            Assert.Equal(6, parameters[3].Value);

            Assert.Equal("@Content_1", parameters[4].ParameterName);
            Assert.Equal("bar", parameters[4].Value);

            Assert.Equal("@CreatedAt_1", parameters[5].ParameterName);
            Assert.Equal(DBNull.Value, parameters[5].Value);
        }

        [Fact]
        public void FillCommand_GetCorrectParameters()
        {
            var sqlCommand = new SqlCommand();
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            var date = DateTime.Now;

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", date}
            });

            builder.AddRow(new Row
            {
                {"Id", 6}, 
                {"Content", "bar"}, 
                {"CreatedAt", null}
            });

            builder.FillCommand(sqlCommand);

            Assert.Equal(builder.ToString(), sqlCommand.CommandText);
            Assert.Equal(builder.Parameters.Count(), sqlCommand.Parameters.Count);
        }


        [Fact]
        public void ToString_ThrowsIfNoRowsSet()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            Assert.Throws<InvalidOperationException>(() => builder.ToString());
        }

        [Fact]
        public void ToString_ColumnSetAndRowColumnsIdenticalSingleRowFact()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            });

            const string expected =
                "INSERT INTO table (Id, Content, CreatedAt) " +
                "VALUES (@Id_0, @Content_0, @CreatedAt_0)";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void ToString_ColumnSetAndRowColumnsIdenticalMultipleRowsFact()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet);

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            });

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}, 
                {"CreatedAt", DateTime.Now}
            });

            const string expected =
                "INSERT INTO table (Id, Content, CreatedAt) " +
                "VALUES (@Id_0, @Content_0, @CreatedAt_0), (@Id_1, @Content_1, @CreatedAt_1)";

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void ToString_ColumnSetAndRowColumnsNotIdenticalMultipleRowsFact()
        {
            var builder = new MsSqlInsertExpressionBuilder("table", _columnSet, true);

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}
            });

            builder.AddRow(new Row
            {
                {"Id", 5}, 
                {"Content", "foo"}
            });

            const string expected =
                "INSERT INTO table (Id, Content) VALUES (@Id_0, @Content_0), (@Id_1, @Content_1)";

            Assert.Equal(expected, builder.ToString());
        }
    }
}
