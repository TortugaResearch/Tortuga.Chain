using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for a database table value function.
/// </summary>
/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
public sealed class ScalarFunctionMetadata<TObjectName, TDbType> : ScalarFunctionMetadata
	where TObjectName : struct
	where TDbType : struct
{
	readonly SqlBuilder<TDbType> m_Builder;

	/// <summary>
	/// Initializes a new instance of the <see cref="ScalarFunctionMetadata{TObjectName, TDbType}"/> class.
	/// </summary>
	/// <param name="name">The name of the scalar function.</param>
	/// <param name="parameters">The parameters.</param>
	/// <param name="typeName">Name of the return type.</param>
	/// <param name="dbType">Return type.</param>
	/// <param name="isNullable">if set to <c>true</c> if the return type is nullable.</param>
	/// <param name="maxLength">The maximum length of the return value.</param>
	/// <param name="precision">The precision of the return value.</param>
	/// <param name="scale">The scale of the return value.</param>
	/// <param name="fullTypeName">Full name of the return type.</param>
	public ScalarFunctionMetadata(TObjectName name, ParameterMetadataCollection<TDbType> parameters, string typeName, TDbType? dbType, bool isNullable, int? maxLength, int? precision, int? scale, string fullTypeName) : base(name.ToString()!, parameters?.GenericCollection!, typeName, dbType, isNullable, maxLength, precision, scale, fullTypeName)
	{
		Name = name;
		Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} is null");
		m_Builder = new SqlBuilder<TDbType>(Name.ToString()!, Parameters);
		DbType = dbType;
	}

	/// <summary>
	/// Gets the return type used by the database.
	/// </summary>
	public new TDbType? DbType { get; }

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
}
