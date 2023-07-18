using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// This allows overriding connection options.
/// </summary>
public class SqlServerDataSourceSettings : DataSourceSettings
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSourceSettings"/> class.
	/// </summary>
	public SqlServerDataSourceSettings()
	{
	}

#if SQL_SERVER_SDS || SQL_SERVER_MDS

	internal SqlServerDataSourceSettings(SqlServerDataSource dataSource, bool forwardEvents = false)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		DefaultCommandTimeout = dataSource.DefaultCommandTimeout;
		StrictMode = dataSource.StrictMode;
		SequentialAccessMode = dataSource.SequentialAccessMode;
		SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
		SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
		ArithAbort = dataSource.ArithAbort;
		XactAbort = dataSource.XactAbort;
		DefaultStringType = dataSource.DefaultStringType;
		DefaultVarCharLength = dataSource.DefaultVarCharLength;
		DefaultNVarCharLength = dataSource.DefaultNVarCharLength;
	}

#elif SQL_SERVER_OLEDB

	internal SqlServerDataSourceSettings(OleDbSqlServerDataSource dataSource, bool forwardEvents = false)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		DefaultCommandTimeout = dataSource.DefaultCommandTimeout;
		StrictMode = dataSource.StrictMode;
		SequentialAccessMode = dataSource.SequentialAccessMode;
		SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents;
		ArithAbort = dataSource.ArithAbort;
		XactAbort = dataSource.XactAbort;
	}

#endif

	/// <summary>
	/// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
	/// </summary>
	/// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
	public bool? ArithAbort { get; set; }

	/// <summary>
	/// Rolls back a transaction if a Transact-SQL statement raises a run-time error.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
	public bool? XactAbort { get; set; }

	/// <summary>
	/// Gets or sets the default type of string parameters. This is used when the query builder cannot determine the best parameter type.
	/// </summary>
	/// <remarks>Set this if encountering performance issues from type conversions in the execution plan.</remarks>
	public SqlDbType? DefaultStringType { get; set; }

	/// <summary>
	/// Gets or sets the default length of varChar string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this is encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public int? DefaultVarCharLength { get; set; }


	/// <summary>
	/// Gets or sets the default length of nVarChar string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this is encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public int? DefaultNVarCharLength { get; set; }
}
