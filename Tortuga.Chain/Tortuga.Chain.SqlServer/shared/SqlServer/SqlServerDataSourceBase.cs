using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Class SqlServerDataSourceBase.
/// </summary>
public abstract partial class SqlServerDataSourceBase : DataSource<SqlConnection, SqlTransaction, SqlCommand, SqlParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSourceBase"/> class.
	/// </summary>
	/// <param name="settings">Optional settings value.</param>
	protected SqlServerDataSourceBase(SqlServerDataSourceSettings? settings) : base(settings)
	{
	}

	/// <summary>
	/// Gets the database metadata.
	/// </summary>
	/// <value>The database metadata.</value>
	public abstract new SqlServerMetadataCache DatabaseMetadata { get; }

	/// <summary>
	/// Called when Database.DatabaseMetadata is invoked.
	/// </summary>
	/// <returns></returns>
	protected override IDatabaseMetadataCache OnGetDatabaseMetadata() => DatabaseMetadata;

	/// <summary>
	/// Gets the default type of string parameters. This is used when the query builder cannot determine the best parameter type.
	/// </summary>
	/// <remarks>Set this if encountering performance issues from type conversions in the execution plan.</remarks>
	public abstract SqlDbType? DefaultStringType { get; }

	/// <summary>
	/// Gets the default length of varChar string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this if encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public abstract int? DefaultVarCharLength { get; }

	/// <summary>
	/// Gets the default length of nVarChar string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this if encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public abstract int? DefaultNVarCharLength { get; }
}
