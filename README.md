# SqlExpressionBuilder

## What is it?
Instead of building your SQL queries with string concatenation or with a string builder, you build it in an object oriented way

````C#
var builder = new SqlExpressionBuilderSelect();
builder.From("dbo.Example", "e");
builder.Select(new[] { "Col1", "Col2" });
builder.Where("e.Col3 = 5");
````

````builder.ToString()```` results in:
````SQL
SELECT e.Col1, e.Col2 FROM dbo.Example e WHERE e.Col3 = 5
````
## Installation
with NuGet:
````
Install-Package DevBlah.SqlExpressionBuilder
````

## Advanced Example
````C#
ISqlExpressionBuilder builder = new SqlExpressionBuilderSelect();

// products
var productsTable = new Table("dbo.Products", "p");
ExpressionColumn productsTablePk = productsTable.GetColumn("Id");
builder.From(productsTable);
builder.Select(new[] { "Id", "Name" }, productsTable);

// categories
var categoriesTable = new Table("dbo.Categories", "c");
ExpressionColumn categoryIdPk = categoriesTable.GetColumn("Id");
builder.JoinInner(new Compare<ExpressionColumn, ExpressionColumn>(
    productsTable.GetColumn("categoryId"),
    categoryIdPk));
builder.Where(new Compare<ExpressionColumn, IDbDataParameter>(
    CompareOperations.GreaterThan,
    categoryIdPk,
    new SqlParameter("@categoryId", SqlDbType.Int) { Value = 5 }));

// stock
var stockTable = new Table("dbo.stock", "s");
builder.JoinLeft(new Compare<ExpressionColumn, ExpressionColumn>(
    productsTablePk,
    stockTable.GetColumn("productId")));
builder.Select(new[] {"Count", "Sold"}, stockTable);
````
results in:
````SQL
SELECT p.Id, p.Name, s.Count, s.Sold 
FROM dbo.Products p 
INNER JOIN dbo.Categories c ON p.categoryId = c.Id 
LEFT JOIN dbo.stock s ON p.Id = s.productId 
WHERE c.Id > @categoryId
````

## Where do I need this?
It is useful if you need to adjust your SQL query by parameters. This could be a very complex scenario, where the this library helps you to keep your code clean and structured.

## Only MS SQL?
As you can see in the advanced example, the Sql Expression Builder is not coupled to a specific implementation, although it was only used with MS SQL and SybaseSQL yet. If you want to use the builder for other database implementations, feel free to contribute...

## ToDos
 - Enhance Documentation
 - Build a generic solution for query limiting (Ideas?)
 - Add More Api Methods for easier usage

## More?
coming soon...
