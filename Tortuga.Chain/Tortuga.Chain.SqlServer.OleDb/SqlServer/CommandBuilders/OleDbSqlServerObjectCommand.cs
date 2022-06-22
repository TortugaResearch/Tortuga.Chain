using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class OleDbSqlServerObjectCommand.
/// </summary>
/// <typeparam name="TArgument">The type of the argument.</typeparam>
internal abstract class OleDbSqlServerObjectCommand<TArgument> : ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument>
		where TArgument : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerObjectCommand{TArgument}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	protected OleDbSqlServerObjectCommand(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue)
		: base(dataSource, argumentValue)
	{
		Table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new OleDbSqlServerDataSourceBase DataSource
	{
		get { return (OleDbSqlServerDataSourceBase)base.DataSource; }
	}

	/// <summary>
	/// Gets the table metadata.
	/// </summary>
	/// <value>The metadata.</value>
	public SqlServerTableOrViewMetadata<OleDbType> Table { get; }

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected override TableOrViewMetadata OnGetTable() => Table;
}
