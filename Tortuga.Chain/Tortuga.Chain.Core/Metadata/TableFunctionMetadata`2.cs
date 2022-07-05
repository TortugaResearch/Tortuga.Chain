using System.Collections.Immutable;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for a database table value function.
/// </summary>
/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
public sealed class TableFunctionMetadata<TObjectName, TDbType> : TableFunctionMetadata
	where TObjectName : struct
	where TDbType : struct
{
	readonly SqlBuilder<TDbType> m_Builder;
	readonly ImmutableArray<SortExpression> m_DefaultSortOrder;

	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.TableFunctionMetadata{TObjectName, TDbType}"/> class.</summary>
	/// <param name="name">The name.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="columns">The columns.</param>
	public TableFunctionMetadata(TObjectName name, ParameterMetadataCollection<TDbType> parameters, ColumnMetadataCollection<TDbType> columns) : base(name.ToString()!, parameters?.GenericCollection!, columns?.GenericCollection!)
	{
		Name = name;
		Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null.");
		Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
		m_Builder = new SqlBuilder<TDbType>(Name.ToString()!, Columns, Parameters);

		if (columns.Count == 0)
			m_DefaultSortOrder = ImmutableArray<SortExpression>.Empty;
		else
			m_DefaultSortOrder = Columns.Select(c => new SortExpression(c.SqlName)).ToImmutableArray();
	}

	/// <summary>
	/// Gets the columns.
	/// </summary>
	/// <value>
	/// The columns.
	/// </value>
	public new ColumnMetadataCollection<TDbType> Columns { get; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public new TObjectName Name { get; }

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	/// <value>
	/// The parameters.
	/// </value>
	public new ParameterMetadataCollection<TDbType> Parameters { get; }

	/// <summary>
	/// Creates a SQL builder.
	/// </summary>
	/// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
	/// <returns></returns>
	public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode) => m_Builder.Clone(strictMode);

	/// <summary>
	/// Gets the default sort order. This is based on the first N columns.
	/// </summary>
	/// <param name="columnsToUse">Sort by [columnsToUse] columns. If not specified, returns all columns.</param>
	/// <returns>If no valid sort columns are found, this will return an empty array.</returns>
	public IEnumerable<SortExpression> GetDefaultSortOrder(int columnsToUse = int.MaxValue)
	{
		if (columnsToUse >= m_DefaultSortOrder.Length)
			return m_DefaultSortOrder;
		else
			return m_DefaultSortOrder.Take(columnsToUse);
	}
}
