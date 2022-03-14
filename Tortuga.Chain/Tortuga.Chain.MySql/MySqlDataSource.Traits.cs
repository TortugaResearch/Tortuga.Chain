using MySqlConnector;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.MySql;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AbstractDataSource, MySqlTransactionalDataSource, MySqlOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, MySqlConnectionStringBuilder>))]
partial class MySqlDataSource
{

	private partial MySqlTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		return new MySqlTransactionalDataSource(this, isolationLevel, forwardEvents);
	}

	private partial async Task<MySqlTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
		MySqlTransaction transaction;
		if (isolationLevel.HasValue)
			transaction = connection.BeginTransaction(isolationLevel.Value);
		else
			transaction = connection.BeginTransaction();
		return new MySqlTransactionalDataSource(this, forwardEvents, connection, transaction);
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
		var con = new AbstractConnection(ConnectionString);
		con.Open();

		//TODO: Research server settings.

		return con;
	}

	private partial async Task<AbstractConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new AbstractConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);

		//TODO: Research server settings.

		return con;
	}

	private partial MySqlOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction)
		=> new MySqlOpenDataSource(this, connection, transaction);
}

