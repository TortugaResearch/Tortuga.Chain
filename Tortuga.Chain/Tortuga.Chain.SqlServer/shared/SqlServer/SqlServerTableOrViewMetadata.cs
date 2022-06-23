using System.Data.Common;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Class SqlServerTableOrViewMetadata.
/// </summary>
/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
/// <typeparam name="TDbType">The type of the t database type.</typeparam>
/// <seealso cref="TableOrViewMetadata{TParameter, SqlServerObjectName, TDbType}" />
public class SqlServerTableOrViewMetadata<TParameter, TDbType> : TableOrViewMetadata<TParameter, SqlServerObjectName, TDbType>
	where TParameter : DbParameter
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerTableOrViewMetadata{TParameter, TDbType}" /> class.
	/// </summary>
	/// <param name="metadataCache">The metadata cache.</param>
	/// <param name="name">The name.</param>
	/// <param name="isTable">if set to <c>true</c> is a table.</param>
	/// <param name="columns">The columns.</param>
	/// <param name="hasTriggers">if set to <c>true</c> has triggers.</param>
	public SqlServerTableOrViewMetadata(DatabaseMetadataCache<TParameter, SqlServerObjectName, TDbType> metadataCache, SqlServerObjectName name, bool isTable, ColumnMetadataCollection<TDbType> columns, bool hasTriggers) : base(metadataCache, name, isTable, columns)
	{
		HasTriggers = hasTriggers;
	}

	/// <summary>
	/// Gets a value indicating whether the associated table has triggers.
	/// </summary>
	/// <value><c>true</c> if this instance has triggers; otherwise, <c>false</c>.</value>
	/// <remarks>This affects SQL generation.</remarks>
	public bool HasTriggers { get; }
}
