using System.ComponentModel;
using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// This is the base class from which all other command builders are created.
	/// </summary>
	/// <typeparam name="TCommand">The type of the command used.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	public abstract class DbCommandBuilder<TCommand, TParameter> : DbCommandBuilder
		where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>Initializes a new instance of the <see cref="DbCommandBuilder{TCommand, TParameter}"/> class.</summary>
		/// <param name="dataSource">The data source.</param>
		/// <exception cref="ArgumentNullException">dataSource</exception>
		protected DbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
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
		public ICommandDataSource<TCommand, TParameter> DataSource { get; }

		/// <summary>
		/// Indicates this operation has no result set.
		/// </summary>
		/// <returns></returns>
		public override sealed ILink<int?> AsNonQuery() { return new NonQueryMaterializer<TCommand, TParameter>(this); }

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer">The materializer.</param>
		/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract CommandExecutionToken<TCommand, TParameter> Prepare(Materializer<TCommand, TParameter> materializer);
	}
}
