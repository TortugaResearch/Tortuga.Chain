using System.Data.OleDb;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SqlServer;


[UseTrait(typeof(Traits.TransactionDataSourceTrait<OleDbSqlServerDataSource, OleDbConnection, OleDbTransaction, OleDbCommand>))]
partial class OleDbSqlServerTransactionalDataSource
{
	private partial void AdditionalDispose() { }

}
