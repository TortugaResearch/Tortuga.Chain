using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AbstractDataSource, SqlServerTransactionalDataSource, SqlServerOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, SqlConnectionStringBuilder>))]
partial class SqlServerDataSource
{

	private partial SqlServerTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		return BeginTransaction(null, isolationLevel, forwardEvents);
	}

	private partial Task<SqlServerTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		return BeginTransactionAsync(null, isolationLevel, forwardEvents, cancellationToken);

	}

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

	private partial AbstractConnection OnCreateConnection()
	{
		var con = new SqlConnection(ConnectionString);
		con.Open();

		if (DatabaseMetadata.ServerDefaultSettings == null)
		{
			var temp = new SqlServerEffectiveSettings();
			temp.Reload(con, null);
			Thread.MemoryBarrier();
			DatabaseMetadata.ServerDefaultSettings = temp;
		}

		var sql = BuildConnectionSettingsOverride();

		if (sql != null)
			using (var cmd = new SqlCommand(sql, con))
				cmd.ExecuteNonQuery();

		return con;
	}

	private partial async Task<AbstractConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new SqlConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);

		if (DatabaseMetadata.ServerDefaultSettings == null)
		{
			var temp = new SqlServerEffectiveSettings();
			await temp.ReloadAsync(con, null).ConfigureAwait(false);
			Thread.MemoryBarrier();
			DatabaseMetadata.ServerDefaultSettings = temp;
		}

		var sql = BuildConnectionSettingsOverride();

		if (sql != null)
			using (var cmd = new SqlCommand(sql, con))
				await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

		return con;
	}

	private partial SqlServerOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction)
		=> new SqlServerOpenDataSource(this, connection, transaction);
}

