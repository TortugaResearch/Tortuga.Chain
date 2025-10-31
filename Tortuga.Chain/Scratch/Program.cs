// See https://aka.ms/new-console-template for more information
using Tortuga.Chain;
using Tortuga.Chain.Metadata;

Console.WriteLine("Hello, World!");

var connectionString = "User ID=chain; Password=toor; Host=localhost; Port=5433; Database=Scratch; Pooling=true;";
var dataSource = new PostgreSqlDataSource(connectionString);

var table = dataSource.DatabaseMetadata.GetTableOrView("cl_content.training");

var columns = table.Columns;

foreach (var column in columns)

{
	Console.WriteLine($"{column.SqlName}\t{column.DbType}\t{column.MaxLength}\t{column.ClrTypeName(NameGenerationOptions.CSharp | NameGenerationOptions.NullableReferenceTypes)}");
}
	
