using Npgsql;
using Tortuga.Shipwright;

namespace Tortuga.Chain.PostgreSql;

[UseTrait(typeof(Traits.TransactionDataSourceTrait<PostgreSqlDataSource, NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand>))]
partial class PostgreSqlTransactionalDataSource
{
	private partial void AdditionalDispose() { }

}
