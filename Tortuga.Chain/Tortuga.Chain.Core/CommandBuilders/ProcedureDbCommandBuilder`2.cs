using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

#pragma warning disable IDE0301 // Simplify collection initialization

/// <summary>
/// This is the base class for command builders that represent stored procedures.
/// </summary>
public abstract class ProcedureDbCommandBuilder<TCommand, TParameter> : MultipleTableDbCommandBuilder<TCommand, TParameter>, IProcedureDbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.ProcedureDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The argument value.</param>
	protected ProcedureDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, object? argumentValue)
		: base(dataSource)
	{
		ArgumentValue = argumentValue;
	}

	/// <summary>
	/// Gets the argument value.
	/// </summary>
	/// <value>The argument value.</value>
	protected object? ArgumentValue { get; }

	/// <summary>
	/// Captures the output parameters as a dictionary.
	/// </summary>
	/// <returns>The output parameters as a dictionary.</returns>
	/// <remarks>This will throw an exception if there are no output parameters.</remarks>
	public ILink<Dictionary<string, object?>> AsOutputs()
	{
		return new AsOutputsMaterializer<TCommand, TParameter>(this);
	}

	/// <summary>
	/// Captures the output parameters and use them to populate a new object.
	/// </summary>
	/// <typeparam name="TObject">The type that will hold the output parameters.</typeparam>
	/// <returns>The output parameters as an object.</returns>
	/// <remarks>This will throw an exception if there are no output parameters.</remarks>
	public ILink<TObject> AsOutputs<TObject>() where TObject : class, new()
	{
		return new AsOutputsMaterializer<TCommand, TParameter, TObject>(this);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public sealed override ColumnMetadata? TryGetColumn(string columnName) => null;

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetColumns() => ImmutableList<ColumnMetadata>.Empty;

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => ImmutableList<ColumnMetadata>.Empty;
}
