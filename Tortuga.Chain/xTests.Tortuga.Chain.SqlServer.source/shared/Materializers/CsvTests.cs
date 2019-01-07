#if Csv_Missing
namespace Tests.CommandBuilders
{
    public class CsvTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public CsvTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData(nameof(Prime))]
        public void SerializeToString(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();

                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                var csv = dataSource.From(EmployeeTableName, new { Title = key }).ToCsv().Execute();
                Assert.IsNotNull(csv, "Serialization didn't return any data.");

                using (var stream = new StringWriter())
                {
                    dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(stream).Execute();
                    Assert.AreEqual(csv, stream.ToString(), "stream version didn't match string version");
                }

                var filteredColumns = dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(desiredColumns: new[] { "EmployeeKey", "FirstName", "LastName" }).Execute();

                var filteredColumns2 = dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(typeof(EmployeeWithName)).Execute();
                Assert.AreEqual(filteredColumns, filteredColumns2, "Filtered column data should have matched.");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public async Task SerializeToStringAsync(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();

                for (var i = 0; i < 10; i++)
                    await dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().ExecuteAsync();

                var csv = await dataSource.From(EmployeeTableName, new { Title = key }).ToCsv().ExecuteAsync();
                Assert.IsNotNull(csv, "Serialization didn't return any data.");

                using (var stream = new StringWriter())
                {
                    await dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(stream).ExecuteAsync();
                    Assert.AreEqual(csv, stream.ToString(), "stream version didn't match string version");
                }

                var filteredColumns = await dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(desiredColumns: new[] { "EmployeeKey", "FirstName", "LastName" }).ExecuteAsync();

                var filteredColumns2 = await dataSource.From(EmployeeTableName, new { Title = key }).ToCsv(typeof(EmployeeWithName)).ExecuteAsync();
                Assert.AreEqual(filteredColumns, filteredColumns2, "Filtered column data should have matched.");
            }
            finally
            {
                Release(dataSource);
            }
        }
    }
}

#endif