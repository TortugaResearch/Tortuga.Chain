using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Metadata for a database table value function.
	/// </summary>
	/// <typeparam name="TName">The type used to represent database object names.</typeparam>
	/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
	public sealed class TableFunctionMetadata<TName, TDbType> : TableFunctionMetadata
		where TName : struct
		where TDbType : struct
	{
		readonly SqlBuilder<TDbType> m_Builder;

		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.TableFunctionMetadata{TName, TDbType}"/> class.</summary>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="columns">The columns.</param>
		public TableFunctionMetadata(TName name, ParameterMetadataCollection<TDbType> parameters, ColumnMetadataCollection<TDbType> columns) : base(name.ToString()!, parameters?.GenericCollection!, columns?.GenericCollection!)
		{
			Name = name;
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null.");
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
			m_Builder = new SqlBuilder<TDbType>(Name.ToString()!, Columns, Parameters);
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
		public new TName Name { get; }

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
	}
}
