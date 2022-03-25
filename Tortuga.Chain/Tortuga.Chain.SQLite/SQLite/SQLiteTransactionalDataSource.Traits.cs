using System.Data.SQLite;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SQLite;

[UseTrait(typeof(Traits.TransactionDataSourceTrait<SQLiteDataSource, SQLiteConnection, SQLiteTransaction, SQLiteCommand>))]
partial class SQLiteTransactionalDataSource
{

	private partial void AdditionalDispose()
	{
		if (m_LockToken != null)
			m_LockToken.Dispose();
	}
}
