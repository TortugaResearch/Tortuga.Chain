using System.Data.Common;

namespace Tortuga.Chain.Core;

/// <summary>
/// The implementation of an operation from an OperationBuilder.
/// </summary>
/// <typeparam name="TConnection">The type of the connection.</typeparam>
/// <typeparam name="TTransaction">The type of the transaction.</typeparam>
/// <param name="connection">The connection.</param>
/// <param name="transaction">The transaction.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
public delegate Task<int?> OperationImplementationAsync<TConnection, TTransaction>(TConnection connection, TTransaction? transaction, CancellationToken cancellationToken)
	where TConnection : DbConnection
	where TTransaction : DbTransaction;
