using MySqlConnector;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql;

/// <summary>
/// Class MySqlTableOrViewMetadata.
/// </summary>
/// <seealso cref="TableOrViewMetadata{ MySqlObjectName, MySqlDbType}" />
public class MySqlTableOrViewMetadata : TableOrViewMetadata<MySqlObjectName, MySqlDbType>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlTableOrViewMetadata" /> class.
	/// </summary>
	/// <param name="metadataCache">The metadata cache.</param>
	/// <param name="name">The name.</param>
	/// <param name="isTable">if set to <c>true</c> is a table.</param>
	/// <param name="columns">The columns.</param>
	/// <param name="engine">The engine.</param>
	internal MySqlTableOrViewMetadata(DatabaseMetadataCache<MySqlObjectName, MySqlDbType> metadataCache, MySqlObjectName name, bool isTable, ColumnMetadataCollection<MySqlDbType> columns, string? engine) : base(metadataCache, name, isTable, columns)
	{
		Engine = engine;
	}

	/// <summary>
	/// Gets the engine.
	/// </summary>
	/// <value>
	/// The engine.
	/// </value>
	public string? Engine { get; }
}