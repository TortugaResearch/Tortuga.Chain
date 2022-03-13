using System.Data.SQLite;
using Tortuga.Chain.SQLite;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<SQLiteTransactionalDataSource, SQLiteOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, SQLiteConnectionStringBuilder>))]
partial class SQLiteDataSource
{

	private partial SQLiteTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		IDisposable? lockToken = null;
		if (!DisableLocks)
			lockToken = SyncLock.WriterLock();

		var connection = CreateConnection();
		SQLiteTransaction transaction;
		if (isolationLevel == null)
			transaction = connection.BeginTransaction();
		else
			transaction = connection.BeginTransaction(isolationLevel.Value);

		return new SQLiteTransactionalDataSource(this, forwardEvents, connection, transaction, lockToken);
	}

	private partial async Task<SQLiteTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		IDisposable? lockToken = null;
		if (!DisableLocks)
			lockToken = await SyncLock.WriterLockAsync();

		var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
		SQLiteTransaction transaction;
		if (isolationLevel == null)
			transaction = connection.BeginTransaction();
		else
			transaction = connection.BeginTransaction(isolationLevel.Value);

		return new SQLiteTransactionalDataSource(this, forwardEvents, connection, transaction, lockToken);
	}

	private partial AbstractConnection OnCreateConnection()
	{
		var con = new SQLiteConnection(ConnectionString);
		con.Open();

		//TODO: Research any potential PRAGMA/Rollback options
		if (EnforceForeignKeys.HasValue)
		{
			var mode = EnforceForeignKeys == true ? "ON" : "OFF";
			using (var cmd = new SQLiteCommand($"PRAGMA foreign_keys = {mode};", con))
				cmd.ExecuteNonQuery();
		}

		return con;
	}

	private partial async Task<AbstractConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new SQLiteConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);

		//TODO: Add in needed PRAGMA statements

		return con;
	}

	private partial SQLiteOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction)
		=> new SQLiteOpenDataSource(this, connection, transaction);

}

