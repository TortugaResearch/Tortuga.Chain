using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// Class ColumnSelectingMaterializer.
/// Implements the <see cref="Materializer{TCommand, TParameter, TResult}" />
/// Implements the <see cref="IColumnSelectingMaterializer{TResult}" />
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <typeparam name="TResult">The type of the t result.</typeparam>
/// <seealso cref="Materializer{TCommand, TParameter, TResult}" />
/// <seealso cref="IColumnSelectingMaterializer{TResult}" />
public abstract class ColumnSelectingMaterializer<TCommand, TParameter, TResult> : Materializer<TCommand, TParameter, TResult>, IColumnSelectingMaterializer<TResult>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnSelectingMaterializer{TCommand, TParameter, TResult}"/> class.
	/// </summary>
	/// <param name="commandBuilder">The associated operation.</param>
	protected ColumnSelectingMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder) : base(commandBuilder)
	{
	}

	/// <summary>
	/// Columns to ignore when generating the list of desired columns.
	/// </summary>
	/// <value>The excluded columns.</value>
	protected IReadOnlyList<string>? ExcludedColumns { get; set; }

	/// <summary>
	/// Only include the indicated columns when generating the list of desired columns.
	/// </summary>
	/// <value>The included columns.</value>
	protected IReadOnlyList<string>? IncludedColumns { get; set; }

	/// <summary>
	/// Returns the list of columns the materializer would like to have.
	/// </summary>
	/// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
	/// <exception cref="InvalidOperationException">Cannot specify both included and excluded columns/properties.</exception>
	public override IReadOnlyList<string> DesiredColumns()
	{
		if (IncludedColumns != null && ExcludedColumns != null)
			throw new InvalidOperationException("Cannot specify both included and excluded columns/properties.");

		if (IncludedColumns != null)
			return IncludedColumns;

		if (ExcludedColumns != null)
		{
			var result = CommandBuilder.TryGetColumns();
			if (result.Count == 0)
				throw new InvalidOperationException("Cannot exclude columns with this command builder.");
			result = result.Where(x => !ExcludedColumns.Contains(x.SqlName)).ToList();
			if (result.Count == 0)
				throw new MappingException("All columns were excluded. The available columns were " + string.Join(", ", CommandBuilder.TryGetColumns().Select(x => x.SqlName)));
			return result.Select(x => x.SqlName).ToList();
		}

		return AllColumns;
	}

	/// <summary>
	/// Excludes the properties from the list of what will be populated in the object.
	/// </summary>
	/// <param name="propertiesToOmit">The properties to omit.</param>
	public virtual ILink<TResult> ExceptProperties(params string[] propertiesToOmit)
	{
		if (propertiesToOmit == null || propertiesToOmit.Length == 0)
			return this;

		ExcludedColumns = propertiesToOmit.ToArray(); //make a defensive copy.
		return this;
	}

	/// <summary>
	/// Limits the list of properties to populate to just the indicated list.
	/// </summary>
	/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	public virtual ILink<TResult> WithProperties(params string[] propertiesToPopulate)
	{
		if (propertiesToPopulate == null || propertiesToPopulate.Length == 0)
			return this;

		IncludedColumns = propertiesToPopulate.ToArray(); //make a defensive copy.
		return this;
	}
}
