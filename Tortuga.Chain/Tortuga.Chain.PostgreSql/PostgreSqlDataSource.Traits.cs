﻿using Npgsql;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.PostgreSql;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<AbstractDataSource, PostgreSqlTransactionalDataSource, PostgreSqlOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand, NpgsqlConnectionStringBuilder>))]
partial class PostgreSqlDataSource
{
	private partial PostgreSqlTransactionalDataSource OnBeginTransaction(IsolationLevel? isolationLevel, bool forwardEvents)
	{
		return new PostgreSqlTransactionalDataSource(this, isolationLevel, forwardEvents);
	}

	private partial async Task<PostgreSqlTransactionalDataSource> OnBeginTransactionAsync(IsolationLevel? isolationLevel, bool forwardEvents, CancellationToken cancellationToken)
	{
		var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
		NpgsqlTransaction transaction;
		if (isolationLevel.HasValue)
			transaction = await connection.BeginTransactionAsync(isolationLevel.Value, cancellationToken).ConfigureAwait(false);
		else
			transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
		return new PostgreSqlTransactionalDataSource(this, forwardEvents, connection, transaction);
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
		var con = new NpgsqlConnection(ConnectionString);
		con.Open();

		//TODO: Research server settings.

		return con;
	}

	private partial async Task<AbstractConnection> OnCreateConnectionAsync(CancellationToken cancellationToken)
	{
		var con = new NpgsqlConnection(ConnectionString);
		await con.OpenAsync(cancellationToken).ConfigureAwait(false);

		//TODO: Research server settings.

		return con;
	}

	private partial PostgreSqlOpenDataSource OnCreateOpenDataSource(AbstractConnection connection, AbstractTransaction? transaction)
		=> new PostgreSqlOpenDataSource(this, connection, transaction);
}
