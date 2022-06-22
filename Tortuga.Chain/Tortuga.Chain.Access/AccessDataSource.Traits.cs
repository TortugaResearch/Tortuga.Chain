using System.Data.OleDb;
using Tortuga.Chain.Access;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AbstractDataSource, AccessTransactionalDataSource, AccessOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, OleDbConnectionStringBuilder>))]
partial class AccessDataSource
{

	private partial AccessTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		var connection = CreateConnection();
		OleDbTransaction transaction;
		if (isolationLevel == null)
			transaction = connection.BeginTransaction();
		else
			transaction = connection.BeginTransaction(isolationLevel.Value);

		return new AccessTransactionalDataSource(this, forwardEvents, connection, transaction);

	}

	private partial async Task<AccessTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
		OleDbTransaction transaction;
		if (isolationLevel == null)
			transaction = connection.BeginTransaction();
		else
			transaction = connection.BeginTransaction(isolationLevel.Value);

		return new AccessTransactionalDataSource(this, forwardEvents, connection, transaction);

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

	private partial OleDbConnection OnCreateConnection()
	{
		var con = new AbstractConnection(ConnectionString);
		con.Open();

		//TODO: Research any potential PRAGMA/Rollback options

		return con;
	}

	private partial async Task<OleDbConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new AbstractConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);
		return con;
	}

	private partial AccessOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction) => new AccessOpenDataSource(this, connection, transaction);
}

