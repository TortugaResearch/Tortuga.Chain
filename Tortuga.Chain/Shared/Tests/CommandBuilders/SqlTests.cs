#if SQL_SERVER_MDS

using Microsoft.Data.SqlClient;
using Tortuga.Chain;

#elif SQL_SERVER_OLEDB
using System.Data.OleDb;
#elif MYSQL
using MySqlConnector;
#elif POSTGRESQL
using Npgsql;
#elif ACCESS
using System.Data.OleDb;
#elif SQLITE
using System.Data.SQLite;
#endif

namespace Tests.CommandBuilders;

[TestClass]
public class SqlCallTests : TestBase
{
#if SQL_SERVER_MDS
	const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
	static readonly object CheckParameter1 = new { @State = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
	static SqlParameter SqlParameter1 => new SqlParameter("@State", "CA") { Size = 100, SqlDbType = SqlDbType.NVarChar };
#elif SQL_SERVER_OLEDB
	const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = ?;";
	static readonly object CheckParameter1 = new { @State = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
	static OleDbParameter SqlParameter1 => new OleDbParameter("@State", "CA") { Size = 100, DbType = DbType.String};
#elif MYSQL
	const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
	static readonly object CheckParameter1 = new { @State = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "p_State", "CA" } };
	static MySqlParameter SqlParameter1 => new MySqlParameter("@State", "CA") { Size = 100, MySqlDbType = MySqlDbType.String };
#elif POSTGRESQL
	const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @param_state;";
	static readonly object CheckParameter1 = new { @param_state = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
	static NpgsqlParameter SqlParameter1 => new NpgsqlParameter("@param_state", "CA") { Size = 100, NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar };
#elif ACCESS
	const string CheckA = @"SELECT Count(*) FROM Customer c WHERE c.State = @param_state;";
	static readonly object CheckParameter1 = new { @param_state = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
	static OleDbParameter SqlParameter1 => new OleDbParameter("@param_state", "CA") { Size = 100, DbType = DbType.String};
#elif SQLITE
	const string CheckA = @"SELECT Count(*) FROM Customer c WHERE c.State = @param_state;";
	static readonly object CheckParameter1 = new { @param_state = "CA" };
	static readonly object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
	static SQLiteParameter SqlParameter1 => new SQLiteParameter("@param_state", "CA") { Size = 100, DbType = DbType.String };
#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Object(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_MDS

	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void Sql_Object_TypeDefault(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName).WithSettings(new() { DefaultStringType = SqlDbType.NChar });
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void Sql_Object_SizeDefault(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName).WithSettings(new() { DefaultVarCharLength = 80, DefaultNVarCharLength = 40 });
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Object_SizeOverride(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1, new() { StringLength = 50 }).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Object_SizeOverride_TooSmall(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1, new() { StringLength = 1 }).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Object_SizeOverride_Max(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			//Normally this would be kept in a static readonly field.
			var defaults = new SqlServerParameterDefaults() { StringType = SqlDbType.VarChar, StringLength = SqlServerParameterDefaults.Max };

			var countA = dataSource.Sql(CheckA, CheckParameter1, defaults).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Object_TypeOverride(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, CheckParameter1, new() { StringType = SqlDbType.Char }).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Dictionary(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, DictParameter1a).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sql_Param(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var countA = dataSource.Sql(CheckA, SqlParameter1).ToInt32().Execute();
			Assert.IsTrue(countA >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}
}
