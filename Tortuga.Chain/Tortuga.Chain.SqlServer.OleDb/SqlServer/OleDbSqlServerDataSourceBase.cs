using System.Data.OleDb;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
	/// <summary>
	/// Class SqlServerDataSourceBase.
	/// </summary>
	public abstract partial class OleDbSqlServerDataSourceBase : DataSource<OleDbConnection, OleDbTransaction, OleDbCommand, OleDbParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerDataSourceBase"/> class.
		/// </summary>
		/// <param name="settings">Optional settings value.</param>
		protected OleDbSqlServerDataSourceBase(SqlServerDataSourceSettings? settings) : base(settings)
		{
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		/// <value>The database metadata.</value>
		public abstract new OleDbSqlServerMetadataCache DatabaseMetadata { get; }

		/// <summary>
		/// Called when Database.DatabaseMetadata is invoked.
		/// </summary>
		/// <returns></returns>
		protected override IDatabaseMetadataCache OnGetDatabaseMetadata() => DatabaseMetadata;

	}
}
