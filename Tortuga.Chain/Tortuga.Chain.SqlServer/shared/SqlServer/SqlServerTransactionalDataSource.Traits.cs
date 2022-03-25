using Tortuga.Shipwright;

namespace Tortuga.Chain.SqlServer;

[UseTrait(typeof(Traits.TransactionDataSourceTrait<SqlServerDataSource, SqlConnection, SqlTransaction, SqlCommand>))]
partial class SqlServerTransactionalDataSource
{

}
