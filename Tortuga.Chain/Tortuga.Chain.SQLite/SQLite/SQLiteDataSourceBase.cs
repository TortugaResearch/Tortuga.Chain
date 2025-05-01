﻿using Nito.AsyncEx;
using System.Data.SQLite;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite;

/// <summary>
/// Base class that represents a SQLite Data Source.
/// </summary>
[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
[SuppressMessage("Design", "CA1200")]
public abstract partial class SQLiteDataSourceBase : DataSource<SQLiteConnection, SQLiteTransaction, SQLiteCommand, SQLiteParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SQLiteDataSourceBase"/> class.
	/// </summary>
	/// <param name="settings">Optional settings object.</param>
	protected SQLiteDataSourceBase(SQLiteDataSourceSettings? settings) : base(settings)
	{
		if (settings != null)
		{
			DisableLocks = settings.DisableLocks ?? false;
		}
	}

	/// <summary>
	/// Gets the database metadata.
	/// </summary>
	/// <value>The database metadata.</value>
	public abstract new SQLiteMetadataCache DatabaseMetadata { get; }

	/// <summary>
	/// Normally we use a reader/writer lock to avoid simultaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexpected exceptions.
	/// </summary>
	public bool DisableLocks { get; }

	/// <summary>
	/// Gets the synchronize lock used during execution of database operations.
	/// </summary>
	/// <value>The synchronize lock.</value>
	internal abstract AsyncReaderWriterLock SyncLock { get; }

	/// <summary>
	/// Called when Database.DatabaseMetadata is invoked.
	/// </summary>
	/// <returns></returns>
	protected override IDatabaseMetadataCache OnGetDatabaseMetadata() => DatabaseMetadata;
}
