using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class SqlServerDataSourceTests
    {
        [TestMethod]
        public void SqlServerDataSourceTests_Ctr()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerTestDatabase"].ConnectionString;
            var dataSource = new SqlServerDataSource(connectionString);
            dataSource.TestConnection();
        }

        [TestMethod]
        public void SqlServerDataSourceTests_CreateFromConfig()
        {
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            dataSource.TestConnection();
        }


        [TestMethod]
        public void SqlServerDataSourceTests_XactAbort()
        {
            const string sql = @"DECLARE @Option bit  = 0;
IF ( (16384 & @@OPTIONS) = 16384 ) SET @Option = 1;
SELECT @Option AS [Option];";

            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");

            var settingOriginal = dataSource.Sql(sql).ToBoolean().Execute();

            dataSource.Settings.XactAbort = true;
            var settingOnA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingOnB = dataSource.GetEffectiveSettings();
            Assert.IsTrue(settingOnA, "XACT_ABORT should have been turned on.");
            Assert.IsTrue(settingOnB.XactAbort, "XACT_ABORT should have been turned on in effective settings.");

            dataSource.Settings.XactAbort = false;
            var settingOffA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingOffB = dataSource.GetEffectiveSettings();
            Assert.IsFalse(settingOffA, "XACT_ABORT should have been turned off.");
            Assert.IsFalse(settingOffB.XactAbort, "XACT_ABORT should have been turned off in effective settings.");

            dataSource.Settings.XactAbort = null;
            var settingDefaultA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingDefaultB = dataSource.GetEffectiveSettings();
            Assert.AreEqual(settingOriginal, settingDefaultA, "XACT_ABORT should have returned to the default setting");
            Assert.AreEqual(settingOriginal, settingDefaultB.XactAbort, "XACT_ABORT should have returned to the default setting in effective settings");

        }

        [TestMethod]
        public void SqlServerDataSourceTests_ArithAbort()
        {
            const string sql = @"DECLARE @Option bit  = 0;
IF ( (64 & @@OPTIONS) = 64 ) SET @Option = 1;
SELECT @Option AS [Option];";

            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");

            var settingOriginal = dataSource.Sql(sql).ToBoolean().Execute();
            var settingOriginalB = dataSource.GetEffectiveSettings();

            dataSource.Settings.ArithAbort = true;
            var settingOnA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingOnB = dataSource.GetEffectiveSettings();
            Assert.IsTrue(settingOnA, "ARITHABORT should have been turned on.");
            Assert.IsTrue(settingOnB.ArithAbort, "ARITHABORT should have been turned on in effective settings.");

            dataSource.Settings.ArithAbort = false;
            var settingOffA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingOffB = dataSource.GetEffectiveSettings();
            Assert.IsFalse(settingOffA, "ARITHABORT should have been turned off.");
            Assert.IsFalse(settingOffB.ArithAbort, "ARITHABORT should have been turned off in effective settings.");

            dataSource.Settings.ArithAbort = null;
            var settingDefaultA = dataSource.Sql(sql).ToBoolean().Execute();
            var settingDefaultB = dataSource.GetEffectiveSettings();
            Assert.AreEqual(settingOriginal, settingDefaultA, "ARITHABORT should have returned to the default setting");
            Assert.AreEqual(settingOriginal, settingDefaultB.ArithAbort, "ARITHABORT should have returned to the default setting in effective settings");

        }


        [TestMethod]
        public void SqlServerDataSourceTests_GetEffectiveSettings()
        {
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            var settings = dataSource.GetEffectiveSettings();
            Debug.WriteLine($"AnsiNullDefaultOff = {settings.AnsiNullDefaultOff}");
            Debug.WriteLine($"AnsiNullDefaultOn = {settings.AnsiNullDefaultOn}");
            Debug.WriteLine($"AnsiNulls = {settings.AnsiNulls}");
            Debug.WriteLine($"AnsiPadding = {settings.AnsiPadding}");
            Debug.WriteLine($"AnsiWarning = {settings.AnsiWarning}");
            Debug.WriteLine($"ArithAbort = {settings.ArithAbort}");
            Debug.WriteLine($"ArithIgnore = {settings.ArithIgnore}");
            Debug.WriteLine($"ConcatNullYieldsNull = {settings.ConcatNullYieldsNull}");
            Debug.WriteLine($"CursorCloseOnCommit = {settings.CursorCloseOnCommit}");
            Debug.WriteLine($"DisableDeferredConstraintChecking = {settings.DisableDeferredConstraintChecking}");
            Debug.WriteLine($"NoCount = {settings.NoCount}");
            Debug.WriteLine($"NumericRoundAbort = {settings.NumericRoundAbort}");
            Debug.WriteLine($"QuotedIdentifier = {settings.QuotedIdentifier}");
            Debug.WriteLine($"XactAbort = {settings.XactAbort}");
        }

        [TestMethod]
        public async Task SqlServerDataSourceTests_GetEffectiveSettingsAsync()
        {
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            var settings = await dataSource.GetEffectiveSettingsAsync();
            Debug.WriteLine($"AnsiNullDefaultOff = {settings.AnsiNullDefaultOff}");
            Debug.WriteLine($"AnsiNullDefaultOn = {settings.AnsiNullDefaultOn}");
            Debug.WriteLine($"AnsiNulls = {settings.AnsiNulls}");
            Debug.WriteLine($"AnsiPadding = {settings.AnsiPadding}");
            Debug.WriteLine($"AnsiWarning = {settings.AnsiWarning}");
            Debug.WriteLine($"ArithAbort = {settings.ArithAbort}");
            Debug.WriteLine($"ArithIgnore = {settings.ArithIgnore}");
            Debug.WriteLine($"ConcatNullYieldsNull = {settings.ConcatNullYieldsNull}");
            Debug.WriteLine($"CursorCloseOnCommit = {settings.CursorCloseOnCommit}");
            Debug.WriteLine($"DisableDeferredConstraintChecking = {settings.DisableDeferredConstraintChecking}");
            Debug.WriteLine($"NoCount = {settings.NoCount}");
            Debug.WriteLine($"NumericRoundAbort = {settings.NumericRoundAbort}");
            Debug.WriteLine($"QuotedIdentifier = {settings.QuotedIdentifier}");
            Debug.WriteLine($"XactAbort = {settings.XactAbort}");
        }

        [TestMethod]
        public void SqlServerDataSourceTests_OnError()
        {
            int count = 0;
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            dataSource.ExecutionError += (s, e) => count++;
            try
            {
                dataSource.Sql("THIS ISN'T VALID SQL").Execute();
                Assert.Fail("excected a sql exception");
            }
            catch (SqlException)
            {
                Assert.AreEqual(1, count, "Expected one error event notification");
            }
        }

        [TestMethod]
        public async Task SqlServerDataSourceTests_OnErrorAsync()
        {
            int count = 0;
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            dataSource.ExecutionError += (s, e) => count++;
            try
            {
                await dataSource.Sql("THIS ISN'T VALID SQL").ExecuteAsync();
                Assert.Fail("excected a sql exception");
            }
            catch (SqlException)
            {
                Assert.AreEqual(1, count, "Expected one error event notification");
            }
        }
    }
}
