using System.Data.SQLite;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite;

[UseTrait(typeof(TransactionDataSourceTrait<SQLiteDataSource, SQLiteConnection, SQLiteTransaction, SQLiteCommand>))]
partial class SQLiteTransactionalDataSource : IHasOnDispose
{
	void IHasOnDispose.OnDispose()
	{
		if (m_LockToken != null)
			m_LockToken.Dispose();
	}
}
