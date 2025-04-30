using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

#pragma warning disable IDE0301 // Simplify collection initialization

/// <summary>
/// Class SqlCallCommandBuilder.
/// Implements the <see cref="MultipleTableDbCommandBuilder{TCommand, TParameter}" />
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <seealso cref="MultipleTableDbCommandBuilder{TCommand, TParameter}" />
public abstract class SqlCallCommandBuilder<TCommand, TParameter> : MultipleTableDbCommandBuilder<TCommand, TParameter>
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GenericDbSqlCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
	protected SqlCallCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource, string sqlStatement, object? argumentValue) : base(dataSource)
	{
		SqlStatement = sqlStatement;
		ArgumentValue = argumentValue;
	}

	/// <summary>
	/// Gets the argument value.
	/// </summary>
	/// <value>The argument value.</value>
	protected object? ArgumentValue { get; }

	/// <summary>
	/// Gets the SQL statement.
	/// </summary>
	/// <value>The SQL statement.</value>
	protected string SqlStatement { get; }

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
