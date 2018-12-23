using Xunit;
using Xunit.Abstractions;

namespace Tests.Core
{
    public class MetadataTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
        public static BasicData Basic = new BasicData(s_DataSources.Values);
        public static TableData Tables = new TableData(s_DataSources.Values);
        public static ViewData Views = new ViewData(s_DataSources.Values);

        public MetadataTests(ITestOutputHelper output) : base(output)
        {
        }

#if SQL_SERVER || OLE_SQL_SERVER

        [Theory, MemberData("Basic")]
        public void DatabaseName(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var databaseName = dataSource.DatabaseMetadata.DatabaseName;
                Assert.IsFalse(string.IsNullOrWhiteSpace(databaseName), "Database name wasn't returned");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || MySQL || OLE_SQL_SERVER

        [Theory, MemberData("Basic")]
        public void DefaultSchema(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var defaultSchema = dataSource.DatabaseMetadata.DefaultSchema;
                Assert.IsFalse(string.IsNullOrWhiteSpace(defaultSchema), "Default schema name wasn't returned");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

        [Theory, MemberData("Basic")]
        public void Preload(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Preload();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Tables")]
        public void GetTable(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Tables")]
        public void GetTable_LowerCase(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName.ToLower());
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Tables")]
        public void GetTable_UpperCase(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName.ToUpper());
                Assert.Equal(tableName, table.Name.ToString());
                Assert.NotEmpty(table.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Views")]
        public void GetView(string assemblyName, string dataSourceName, DataSourceType mode, string viewName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.Reset();
                var view = dataSource.DatabaseMetadata.GetTableOrView(viewName);
                Assert.Equal(viewName, view.Name.ToString());
                Assert.NotEmpty(view.Columns);
            }
            finally
            {
                Release(dataSource);
            }
        }

#if SQL_SERVER || POSTGRESQL || OLE_SQL_SERVER

        [Theory, MemberData("Prime")]
        public void VerifyFunction1(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTableFunctions();
                var function = dataSource.DatabaseMetadata.GetTableFunction(TableFunction1Name);
                Assert.IsNotNull(function, $"Error reading function {TableFunction1Name}");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if POSTGRESQL

        [Theory, MemberData("Prime")]
        public void GetTableWithDefaultSchema(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView("employee");

                Assert.AreEqual(DefaultSchema, table.Name.Schema, "Incorrect default schema was used");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void GetSchemaList(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var schemaList = dataSource.DatabaseMetadata.DefaultSchemaList;

                Assert.IsNotNull(schemaList, "Schema list is null");
                Assert.NotEmpty(schemaList, "Schema list is empty");

                foreach (var schema in schemaList)
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(schema), "Empty schema name found in list");
                    Assert.IsFalse(schema.StartsWith(" "), "Leading space in schema name");
                    Assert.IsFalse(schema.EndsWith(" "), "Trailing space in schema name");
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQL_SERVER || OLE_SQL_SERVER
        [Theory, MemberData("Prime")]
        public void VerifyFunction2(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.DatabaseMetadata.PreloadTableFunctions();
                var function = dataSource.DatabaseMetadata.GetTableFunction(TableFunction2Name);
                Assert.IsNotNull(function, $"Error reading function {TableFunction2Name}");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}