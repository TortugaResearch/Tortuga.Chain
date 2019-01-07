using Tests;
using Xunit.Abstractions;

namespace xTests.Tortuga.Chain.MySql.source.Custom
{
    public class UnitTest1 : TestBase
    {
        public UnitTest1(ITestOutputHelper output) : base(output)
        {
        }

        /*
        [Fact]
        public void AffectedRowsCount()
        {
            var original = new ChangeTrackingEmployee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var employeeKey = s_PrimaryDataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
            var newTitle = "President";

            using (var con = new MySqlConnection(s_PrimaryConnectionString))
            {
                con.Open();

                var sql = "UPDATE `hr`.`employee` SET `FirstName` = @FirstName, `LastName` = @LastName, `Title` = @Title WHERE `EmployeeKey` = @EmployeeKey;";

                using (var cmd1 = new MySqlCommand(sql, con))
                {
                    cmd1.Parameters.AddWithValue("@FirstName", original.FirstName);
                    cmd1.Parameters.AddWithValue("@LastName", original.LastName);
                    cmd1.Parameters.AddWithValue("@Title", newTitle);
                    cmd1.Parameters.AddWithValue("@EmployeeKey", employeeKey);

                    var affectedRows = cmd1.ExecuteNonQuery();
                    Assert.AreEqual(1, affectedRows, "Affected rows returned the wrong value on the second attempt.");
                }

                using (var cmd2 = new MySqlCommand(sql, con))
                {
                    cmd2.Parameters.AddWithValue("@FirstName", original.FirstName);
                    cmd2.Parameters.AddWithValue("@LastName", original.LastName);
                    cmd2.Parameters.AddWithValue("@Title", newTitle);
                    cmd2.Parameters.AddWithValue("@EmployeeKey", employeeKey);

                    var affectedRows = cmd2.ExecuteNonQuery();
                    Assert.AreEqual(1, affectedRows, "Affected rows returned the wrong value on the second attempt.");
                }
            }
        }
            */
    }
}