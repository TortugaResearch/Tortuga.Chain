using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SQLite;
using System.IO;

namespace Tests
{
    [TestClass]
    public class Setup
    {
        private const string databaseFileName = "SQLiteTestDatabase.sqlite";
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            File.Delete(databaseFileName);

            SQLiteConnection.CreateFile(databaseFileName);
            var m_dbConnection = new SQLiteConnection("Data Source=SQLiteTestDatabase.sqlite;Version=3;");
            m_dbConnection.Open();


            string sql = @"
CREATE TABLE Employee
(
	EmployeeKey INTEGER PRIMARY KEY,
	FirstName nvarChar(25) NOT NULL,
	MiddleName nvarChar(25) NULL,
	LastName nVarChar(25) NOT NULL,
	Title nVarChar(100) null,
	ManagerKey INT NULL REferences Employee(EmployeeKey)
)";

            using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
            {
                command.ExecuteNonQuery();
            }

            sql = @"INSERT INTO Employee ([EmployeeKey], [FirstName], [MiddleName], [LastName], [Title], [ManagerKey]) VALUES (@EmployeeKey, @FirstName, @MiddleName, @LastName, @Title, @ManagerKey); SELECT [EmployeeKey], [FirstName], [MiddleName], [LastName], [Title], [ManagerKey] FROM Employee WHERE ROWID = last_insert_rowid();";

            for (var i = 0; i < 10; i++)
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.Parameters.AddWithValue("@EmployeeKey", DBNull.Value);
                    command.Parameters.AddWithValue("@FirstName", "Tom");
                    command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", "Jones");
                    command.Parameters.AddWithValue("@Title", "CEO");
                    command.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
                    var key = command.ExecuteScalar();
                }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            File.Delete(databaseFileName);
        }
    }
}
