using System.Data.OleDb;
using Tortuga.Chain.SqlServer;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<OleDbSqlServerTransactionalDataSource, OleDbSqlServerOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand>))]
partial class OleDbSqlServerDataSource
{

	private partial OleDbSqlServerTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		return BeginTransaction(null, isolationLevel, forwardEvents);
	}

	private partial Task<OleDbSqlServerTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		return BeginTransactionAsync(null, isolationLevel, forwardEvents, cancellationToken);

	}

	private partial AbstractConnection OnCreateConnection()
	{
		var con = new OleDbConnection(ConnectionString);
		con.Open();

		if (m_ServerDefaultSettings == null)
		{
			var temp = new OleDbSqlServerEffectiveSettings();
			temp.Reload(con, null);
			Thread.MemoryBarrier();
			m_ServerDefaultSettings = temp;
		}

		var sql = BuildConnectionSettingsOverride();

		if (sql != null)
			using (var cmd = new OleDbCommand(sql, con))
				cmd.ExecuteNonQuery();

		return con;
	}

	private partial async Task<AbstractConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new OleDbConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);

		if (m_ServerDefaultSettings == null)
		{
			var temp = new OleDbSqlServerEffectiveSettings();
			await temp.ReloadAsync(con, null).ConfigureAwait(false);
			Thread.MemoryBarrier();
			m_ServerDefaultSettings = temp;
		}

		var sql = BuildConnectionSettingsOverride();

		if (sql != null)
			using (var cmd = new OleDbCommand(sql, con))
				await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

		return con;
	}

	private partial OleDbSqlServerOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction)
		=> new OleDbSqlServerOpenDataSource(this, connection, transaction);

}

