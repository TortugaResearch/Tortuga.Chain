using Microsoft.Extensions.Configuration;
using System.Data.OleDb;

namespace Tests;

[TestClass]
public static class Setup
{
	const string s_DatabaseFileName = "AccessTestDatabase.mdb";

	static int s_RunOnce;

	[AssemblyCleanup]
	public static void AssemblyCleanup()
	{
		//File.Delete(databaseFileName);
	}

	[AssemblyInitialize]
	public static void AssemblyInit(TestContext context)
	{
		TestBase.SetupTestBase();
	}

	public static void CreateDatabase()
	{
		if (Interlocked.CompareExchange(ref s_RunOnce, 1, 0) == 1)
			return;

		File.Delete(s_DatabaseFileName);

		var templateFile = new FileInfo(Path.Combine(@"Databases", s_DatabaseFileName));
		templateFile.CopyTo(s_DatabaseFileName);

		var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();

		var connectionString = configuration.GetSection("ConnectionStrings").GetChildren().First().Value;

		//var cat = new ADOX.Catalog();
		//cat.Create(connectionString);

		var dbConnection = new OleDbConnection(connectionString);

		using (dbConnection)
		{
			dbConnection.Open();

			string sql = @"
CREATE TABLE Employee
(
	EmployeeKey COUNTER PRIMARY KEY,
	FirstName TEXT(30) NOT NULL,
	MiddleName TEXT(30) NULL,
	LastName TEXT(30) NOT NULL,
	Title TEXT(100) null,
	ManagerKey LONG NULL REFERENCES Employee(EmployeeKey),
	OfficePhone TEXT(15) NULL,
	CellPhone TEXT(15) NULL,
	CreatedDate DateTime NOT NULL DEFAULT NOW(),
	UpdatedDate DateTime NULL,
	EmployeeId TEXT(50) NOT NULL,
	Gender Char(1) NOT NULL,
	Status Char(1) NULL
)";

			string sql2 = @"CREATE TABLE Customer
(
	CustomerKey COUNTER PRIMARY KEY,
	FullName TEXT(100) NULL,
	State TEXT(2) NOT NULL,
	CreatedByKey INTEGER NULL,
	UpdatedByKey INTEGER NULL,
	CreatedDate DATETIME NULL DEFAULT NOW(),
	UpdatedDate DATETIME NULL,
	DeletedFlag BIT NOT NULL DEFAULT 0,
	DeletedDate DateTime NULL,
	DeletedByKey INTEGER NULL,
	BirthDay DATETIME NULL,
	PreferredCallTime DATETIME NULL
)";

			const string sql3 = "CREATE VIEW EmployeeLookup AS SELECT FirstName, LastName, EmployeeKey, EmployeeId FROM Employee";

			const string sql4 = @"CREATE VIEW EmployeeWithManager
AS
SELECT  Employee_1.EmployeeKey ,
		Employee_1.FirstName ,
		Employee_1.MiddleName ,
		Employee_1.LastName ,
		Employee_1.Title ,
		Employee_1.ManagerKey ,
		Employee_1.OfficePhone ,
		Employee_1.CellPhone ,
		Employee_1.CreatedDate ,
		Employee_1.UpdatedDate ,
		Employee_1.EmployeeId,
		Employee_1.Gender,
		Employee_2.EmployeeKey AS ManagerEmployeeKey ,
		Employee_2.FirstName AS ManagerFirstName ,
		Employee_2.MiddleName AS ManagerMiddleName ,
		Employee_2.LastName AS ManagerLastName ,
		Employee_2.Title AS ManagerTitle ,
		Employee_2.ManagerKey AS ManagerManagerKey ,
		Employee_2.OfficePhone AS ManagerOfficePhone ,
		Employee_2.CellPhone AS ManagerCellPhone ,
		Employee_2.CreatedDate AS ManagerCreatedDate ,
		Employee_2.UpdatedDate AS ManagerUpdatedDate ,
		Employee_2.Gender AS ManagerGender
FROM    Employee AS Employee_1
		LEFT JOIN Employee AS Employee_2 ON Employee_2.EmployeeKey = Employee_1.ManagerKey";

			string sql5 = @"CREATE TABLE Location
(
	LocationKey COUNTER PRIMARY KEY,
	LocationName TEXT(100) NULL
)";

			using (var command = new OleDbCommand(sql, dbConnection))
				command.ExecuteNonQuery();

			using (var command = new OleDbCommand(sql2, dbConnection))
				command.ExecuteNonQuery();

			using (var command = new OleDbCommand(sql3, dbConnection))
				command.ExecuteNonQuery();

			using (var command = new OleDbCommand(sql4, dbConnection))
				command.ExecuteNonQuery();

			using (var command = new OleDbCommand(sql5, dbConnection))
				command.ExecuteNonQuery();

			sql = "INSERT INTO Employee ([FirstName], [MiddleName], [LastName], [Title], [ManagerKey], [EmployeeId], Gender) VALUES (@FirstName, @MiddleName, @LastName, @Title, @ManagerKey, @EmployeeId, @Gender)";

			sql2 = "INSERT INTO Employee ([FirstName], [MiddleName], [LastName], [Title], [ManagerKey], [CreatedDate], [EmployeeId], Gender) VALUES (@FirstName, @MiddleName, @LastName, @Title, @ManagerKey, @CreatedDate, @EmployeeId, @Gender)";

			//Date/Time format - 4/30/2016 5:25:17 PM
			const string DateTimeFormat = "M/d/yyyy h:mm:ss tt";

			using (var command = new OleDbCommand(sql, dbConnection))
			{
				//command.Parameters.AddWithValue("@EmployeeKey", DBNull.Value);
				command.Parameters.AddWithValue("@FirstName", "Tom");
				command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
				command.Parameters.AddWithValue("@LastName", "Jones");
				command.Parameters.AddWithValue("@Title", "CEO");
				command.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
				command.Parameters.AddWithValue("@EmployeeId", Guid.NewGuid().ToString());
				command.Parameters.AddWithValue("@Gender", 'X');
				command.ExecuteNonQuery();
			}

			using (var command = new OleDbCommand(sql2, dbConnection))
			{
				//command.Parameters.AddWithValue("@EmployeeKey", DBNull.Value);
				command.Parameters.AddWithValue("@FirstName", "Tom");
				command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
				command.Parameters.AddWithValue("@LastName", "Jones");
				command.Parameters.AddWithValue("@Title", "CEO");
				command.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
				command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString(DateTimeFormat));
				command.Parameters.AddWithValue("@EmployeeId", Guid.NewGuid().ToString());
				command.Parameters.AddWithValue("@Gender", 'X');
				command.ExecuteNonQuery();
			}

			using (var command = new OleDbCommand(sql2, dbConnection))
			{
				//command.Parameters.AddWithValue("@EmployeeKey", DBNull.Value);
				command.Parameters.AddWithValue("@FirstName", "Tom");
				command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
				command.Parameters.AddWithValue("@LastName", "Jones");
				command.Parameters.AddWithValue("@Title", "CEO");
				command.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
				var param = command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
				param.OleDbType = OleDbType.Date;
				command.Parameters.AddWithValue("@EmployeeId", Guid.NewGuid().ToString());
				command.Parameters.AddWithValue("@Gender", 'X');
				command.ExecuteNonQuery();
			}

			using (var command = new OleDbCommand("SELECT @@IDENTITY", dbConnection))
			{
				var key = command.ExecuteScalar();

				using (var command2 = new OleDbCommand("UPDATE Employee SET MiddleName = 'Frank' WHERE EmployeeKey = @EmployeeKey", dbConnection))
				{
					command2.Parameters.AddWithValue("@EmployeeKey", key);
					var updateCount = command2.ExecuteNonQuery();
				}
			}
		}
	}
}
