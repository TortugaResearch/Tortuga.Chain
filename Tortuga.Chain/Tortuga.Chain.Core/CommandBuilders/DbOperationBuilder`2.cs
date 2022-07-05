using System.ComponentModel;
using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class from which all other operation builders are created.
/// </summary>
/// <typeparam name="TConnection">The type of the t connection.</typeparam>
/// <typeparam name="TTransaction">The type of the t transaction.</typeparam>
/// <seealso cref="DbCommandBuilder" />
public abstract class DbOperationBuilder<TConnection, TTransaction> : DbCommandBuilder
	where TConnection : DbConnection
	where TTransaction : DbTransaction
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DbOperationBuilder{TConnection, TTransaction}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <exception cref="ArgumentNullException">dataSource</exception>
	protected DbOperationBuilder(IOperationDataSource<TConnection, TTransaction> dataSource)
	{
		DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
		StrictMode = dataSource.StrictMode;
		SequentialAccessMode = dataSource.SequentialAccessMode;
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IOperationDataSource<TConnection, TTransaction> DataSource { get; }

	/// <summary>
	/// Indicates this operation has no result set.
	/// </summary>
	/// <returns>ILink&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
	public override ILink<int?> AsNonQuery() => new Operation<TConnection, TTransaction>(this);

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public abstract OperationExecutionToken<TConnection, TTransaction> Prepare();

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override ColumnMetadata? TryGetColumn(string columnName) => null;

	/// <summary>
	/// Implementation the specified operation.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
	protected internal abstract int? Implementation(TConnection connection, TTransaction? transaction);

	/// <summary>
	/// Implementation the specified operation.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>Task&lt;System.Nullable&lt;System.Int32&gt;&gt;.</returns>
	protected internal abstract Task<int?> ImplementationAsync(TConnection connection, TTransaction? transaction, CancellationToken cancellationToken);
}
