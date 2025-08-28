using MySqlConnector;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql;

/// <summary>
/// Class MySqlDataSourceBase.
/// </summary>
[SuppressMessage("Design", "CA1200")]
public abstract partial class MySqlDataSourceBase : DataSource<MySqlConnection, MySqlTransaction, MySqlCommand, MySqlParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSourceBase"/> class.
	/// </summary>
	/// <param name="settings">Optional settings object.</param>
	protected MySqlDataSourceBase(DataSourceSettings? settings) : base(settings)
	{
	}

	/// <summary>
	/// Gets the database metadata.
	/// </summary>
	public abstract new MySqlMetadataCache DatabaseMetadata { get; }

	/// <summary>
	/// Gets a record by its primary key.
	/// </summary>
	/// <typeparam name="TObject">The type of the object. Used to determine which table will be read.</typeparam>
	/// <param name="key">The key.</param>
	/// <returns></returns>
	/// <remarks>This only works on tables that have a scalar primary key.</remarks>
	public SingleRowDbCommandBuilder<MySqlCommand, MySqlParameter, TObject> GetByKey<TObject>(ulong key) where TObject : class
	{
		return GetByKey<TObject, ulong>(key);
	}

	/// <summary>
	/// Gets a set of records by a key list.
	/// </summary>
	/// <typeparam name="TObject">The type of the returned object.</typeparam>
	/// <param name="keys">The keys.</param>
	public MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> GetByKeyList<TObject>(IEnumerable<ulong> keys) where TObject : class
	{
		return GetByKeyList<TObject, ulong>(keys);
	}

	/// <summary>
	/// Called when Database.DatabaseMetadata is invoked.
	/// </summary>
	/// <returns></returns>
	protected override IDatabaseMetadataCache OnGetDatabaseMetadata() => DatabaseMetadata;
}
