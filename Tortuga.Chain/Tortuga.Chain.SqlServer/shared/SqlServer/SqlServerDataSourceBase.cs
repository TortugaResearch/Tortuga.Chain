using Tortuga.Chain.Core;
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

	private protected void CommandFixup(CommandExecutionToken<SqlCommand, SqlParameter> executionToken, SqlCommand cmd)
	{
#if SQL_SERVER_MDS
		if (!executionToken.HasOutputParameters)
			cmd.EnableOptimizedParameterBinding = true;
#endif
	}
}