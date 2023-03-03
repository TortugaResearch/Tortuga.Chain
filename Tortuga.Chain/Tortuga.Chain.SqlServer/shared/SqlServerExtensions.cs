using System.Collections.Concurrent;
using Tortuga.Chain.SqlServer;

namespace Tortuga.Chain;

/// <summary>
/// Class SqlServerExtensions.
/// </summary>
public static class SqlServerExtensions
{
	readonly static ConcurrentDictionary<string, SqlServerDataSource> s_CachedDataSources = new ConcurrentDictionary<string, SqlServerDataSource>();

	/// <summary>
	/// Returns a data source wrapped around the connection.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <returns>SqlServerOpenDataSource.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static SqlServerOpenDataSource AsDataSource(this SqlConnection connection)
	{
		if (connection == null)
			throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
		if (connection.State == ConnectionState.Closed)
			connection.Open();

		var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new SqlServerDataSource(cs));
		return new SqlServerOpenDataSource(dataSourceBase, connection, null);
	}

	/// <summary>
	/// Returns a data source wrapped around the transaction.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	/// <returns>SqlServerOpenDataSource.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static SqlServerOpenDataSource AsDataSource(this SqlConnection connection, SqlTransaction transaction)
	{
		if (connection == null)
			throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
		if (connection.State == ConnectionState.Closed)
			connection.Open();

		var dataSourceBase = s_CachedDataSources.GetOrAdd(connection.ConnectionString, cs => new SqlServerDataSource(cs));
		return new SqlServerOpenDataSource(dataSourceBase, connection, transaction);
	}

	///// <summary>
	///// Set the default type/size for parameters to address performance issues in SQL Server.
	///// </summary>
	///// <param name="commandBuilder"/>
	///// <param name="stringLength">Sets the default length of string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.</param>
	///// <param name="stringType">Sets the default type of string parameters. This is used when the query builder cannot determine the best parameter type.</param>
	///// <remarks>Set stringType if encountering performance issues from type conversions in the execution plan. Set stringLength when encountering an excessive number of execution plans that only differ by the length of a string.</remarks>
	//public static SqlCallCommandBuilder<SqlCommand, SqlParameter> WithDefaults(this SqlCallCommandBuilder<SqlCommand, SqlParameter> commandBuilder, SqlDbType? stringType, int? stringLength)
	//{
	//	return ((SqlServerSqlCall)commandBuilder).WithDefaults(new SqlServerParameterDefaults() { StringType = stringType, StringLength = stringLength });
	//}


	///// <summary>
	///// Set the default type/size for parameters to address performance issues in SQL Server.
	///// </summary>
	//public static SqlCallCommandBuilder<SqlCommand, SqlParameter> WithDefaults2(this SqlCallCommandBuilder<SqlCommand, SqlParameter> commandBuilder, SqlServerParameterDefaults defaults)
	//{
	//	return ((SqlServerSqlCall)commandBuilder).WithDefaults(defaults);
	//}

}
