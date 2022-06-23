using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access.CommandBuilders;

/// <summary>
/// Base class that describes a Access database command.
/// </summary>
internal abstract class AccessObjectCommand<TArgument> : ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument>
	where TArgument : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AccessObjectCommand{TArgument}" /> class
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	protected AccessObjectCommand(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue)
		: base(dataSource, argumentValue)
	{
		Table = ((AccessDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Gets the table metadata.
	/// </summary>
	public TableOrViewMetadata<OleDbParameter, AccessObjectName, OleDbType> Table { get; }

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected override TableOrViewMetadata OnGetTable() => Table;
}
