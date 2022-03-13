using System.Data.OleDb;
using Tortuga.Chain.Access;
using Tortuga.Chain.AuditRules;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AccessDataSource, AccessTransactionalDataSource, AccessOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, OleDbConnectionStringBuilder>))]
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

	private partial Tortuga.Chain.AccessDataSource OnCloneWithOverrides(Tortuga.Chain.Core.ICacheAdapter? cache, System.Collections.Generic.IEnumerable<Tortuga.Chain.AuditRules.AuditRule>? additionalRules, System.Object? userValue)
	{
		var result = WithSettings(null);
		if (cache != null)
			result.m_Cache = cache;
		if (additionalRules != null)
			result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
		if (userValue != null)
			result.AuditRules = userValue;
	}


}

