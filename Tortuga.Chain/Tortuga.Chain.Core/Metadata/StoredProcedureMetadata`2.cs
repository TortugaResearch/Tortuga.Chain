using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class StoredProcedureMetadata.
	/// </summary>
	/// <typeparam name="TName">The type used to represent database object names.</typeparam>
	/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
	public sealed class StoredProcedureMetadata<TName, TDbType> : StoredProcedureMetadata
		where TName : struct
		where TDbType : struct
	{
		readonly SqlBuilder<TDbType> m_Builder;

		/// <summary>
		/// Initializes a new instance of the <see cref="StoredProcedureMetadata{TName, TDbType}" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="parameters">The parameters.</param>
		public StoredProcedureMetadata(TName name, ParameterMetadataCollection<TDbType> parameters) : base(name.ToString()!, parameters?.GenericCollection!)
		{
			Name = name;
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null.");

			m_Builder = new SqlBuilder<TDbType>(Name.ToString()!, Parameters);
		}

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
		/// <returns></returns>
		public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode) => m_Builder.Clone(strictMode);
	}
}
