using System.Data.OleDb;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access
{
	/// <summary>
	/// Base class that represents a Access Data Source.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	[SuppressMessage("Design", "CA1200")]
	public abstract partial class AccessDataSourceBase : DataSource<OleDbConnection, OleDbTransaction, OleDbCommand, OleDbParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessDataSourceBase"/> class.
		/// </summary>
		/// <param name="settings">Optional settings object.</param>
		protected AccessDataSourceBase(AccessDataSourceSettings? settings) : base(settings)
		{
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		/// <value>The database metadata.</value>
		public abstract new AccessMetadataCache DatabaseMetadata { get; }

		/// <summary>
		/// Called when Database.DatabaseMetadata is invoked.
		/// </summary>
		/// <returns></returns>
		protected override IDatabaseMetadataCache OnGetDatabaseMetadata() => DatabaseMetadata;
	}
}
