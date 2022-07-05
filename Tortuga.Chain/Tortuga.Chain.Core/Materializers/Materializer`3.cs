using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// This is the base class for materializers that return a value. Most operation are not executed without first attaching a materializer subclass.
/// Implements the <see cref="Tortuga.Chain.Materializers.Materializer{TCommand, TParameter}"/>
/// Implements the <see cref="Tortuga.Chain.ILink{TResult}"/>
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <seealso cref="Tortuga.Chain.Materializers.Materializer{TCommand, TParameter}" />
/// <seealso cref="Tortuga.Chain.ILink{TResult}" />
/// <seealso cref="Materializer{TCommand, TParameter}" />
public abstract class Materializer<TCommand, TParameter, TResult> : Materializer<TCommand, TParameter>, ILink<TResult>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Materializer{TCommand, TParameter, TResult}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The associated operation.</param>
	protected Materializer(DbCommandBuilder<TCommand, TParameter> commandBuilder) : base(commandBuilder) { }

	/// <summary>
	/// Gets the data source that is associated with this materializer or appender.
	/// </summary>
	/// <value>The data source.</value>
	public IDataSource DataSource => CommandBuilder.DataSource;

	/// <summary>
	/// Gets the type converter associated with this materializer.
	/// </summary>
	protected MaterializerTypeConverter Converter => CommandBuilder.DataSource.DatabaseMetadata.Converter;

	/// <summary>
	/// Execute the operation synchronously.
	/// </summary>
	/// <returns></returns>
	public abstract TResult Execute(object? state = null);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public async Task<TResult> ExecuteAsync(object? state = null) => await ExecuteAsync(CancellationToken.None, state).ConfigureAwait(false);

	/// <summary>
	/// Execute the operation asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns></returns>
	public abstract Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object? state = null);
}
