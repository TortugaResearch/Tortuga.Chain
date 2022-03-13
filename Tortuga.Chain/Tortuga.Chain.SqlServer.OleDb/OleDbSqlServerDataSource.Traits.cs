using System.Data.OleDb;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AbstractDataSource, OleDbSqlServerTransactionalDataSource, OleDbSqlServerOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, OleDbConnectionStringBuilder>))]
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

	private partial AbstractDataSource OnCloneWithOverrides(ICacheAdapter? cache, IEnumerable<AuditRule>? additionalRules, object? userValue)
	{
		var result = WithSettings(null);
		if (cache != null)
			result.m_Cache = cache;
		if (additionalRules != null)
			result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
		if (userValue != null)
			result.UserValue = userValue;
		return result;
	}


}

