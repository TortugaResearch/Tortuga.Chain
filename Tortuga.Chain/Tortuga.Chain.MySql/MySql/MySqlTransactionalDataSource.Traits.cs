using MySqlConnector;
using Tortuga.Shipwright;

namespace Tortuga.Chain.MySql;

[UseTrait(typeof(Traits.TransactionDataSourceTrait<MySqlDataSource, MySqlConnection, MySqlTransaction, MySqlCommand>))]
partial class MySqlTransactionalDataSource
{



}
