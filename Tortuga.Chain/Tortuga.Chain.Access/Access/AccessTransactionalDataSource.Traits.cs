using System.Data.OleDb;
using Tortuga.Shipwright;

namespace Tortuga.Chain.Access;

[UseTrait(typeof(Traits.TransactionDataSourceTrait<AccessDataSource, OleDbConnection, OleDbTransaction, OleDbCommand>))]
partial class AccessTransactionalDataSource
{
}
