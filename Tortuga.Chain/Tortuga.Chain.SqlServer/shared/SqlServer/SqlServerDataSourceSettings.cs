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
}
