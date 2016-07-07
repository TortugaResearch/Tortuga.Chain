using System;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using Xunit;


[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tests
{


    public class Setup
    {
        private const string databaseFileName = "AccessTestDatabase.mdb";



        public static void AssemblyInit()
        {
            File.Delete(databaseFileName);


            var connectionString = ConfigurationManager.ConnectionStrings["AccessTestDatabase"].ConnectionString;

            var cat = new ADOX.Catalog();
            cat.Create(connectionString);

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
    CreatedDate DateTime NOT NULL DEFAULT NOW(),
    UpdatedDate DateTime NULL
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
	DeletedByKey INTEGER NULL
)";

                string sql3 = @"CREATE VIEW EmployeeLookup AS SELECT FirstName, LastName, EmployeeKey FROM Employee";

                using (var command = new OleDbCommand(sql, dbConnection))
                    command.ExecuteNonQuery();

                using (var command = new OleDbCommand(sql2, dbConnection))
                    command.ExecuteNonQuery();

                using (var command = new OleDbCommand(sql3, dbConnection))
                    command.ExecuteNonQuery();

                sql = @"INSERT INTO Employee ([FirstName], [MiddleName], [LastName], [Title], [ManagerKey]) VALUES (@FirstName, @MiddleName, @LastName, @Title, @ManagerKey)";

                sql2 = @"INSERT INTO Employee ([FirstName], [MiddleName], [LastName], [Title], [ManagerKey], [CreatedDate]) VALUES (@FirstName, @MiddleName, @LastName, @Title, @ManagerKey, @CreatedDate)";

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

        public static void AssemblyCleanup()
        {
            //File.Delete(databaseFileName);
        }



    }
}
