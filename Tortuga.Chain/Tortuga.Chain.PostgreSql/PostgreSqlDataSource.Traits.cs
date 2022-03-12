using Npgsql;
using Tortuga.Chain.PostgreSql;
using Tortuga.Shipwright;

namespace Tortuga.Chain;

[UseTrait(typeof(Traits.RootDataSourceTrait<PostgreSqlTransactionalDataSource, PostgreSqlOpenDataSource, AbstractConnection, AbstractTransaction, AbstractCommand>))]
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
			transaction = connection.BeginTransaction(isolationLevel.Value);
		else
			transaction = connection.BeginTransaction();
		return new PostgreSqlTransactionalDataSource(this, forwardEvents, connection, transaction);

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

