using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerObjectCommand.
/// </summary>
/// <typeparam name="TArgument">The type of the argument.</typeparam>
internal abstract class SqlServerObjectCommand<TArgument> : ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument>
	where TArgument : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerObjectCommand{TArgument}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	protected SqlServerObjectCommand(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue)
		: base(dataSource, argumentValue)
	{
		Table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new SqlServerDataSourceBase DataSource
	{
		get { return (SqlServerDataSourceBase)base.DataSource; }
	}

	/// <summary>
	/// Gets the table metadata.
	/// </summary>
	/// <value>The metadata.</value>
	public SqlServerTableOrViewMetadata<SqlDbType> Table { get; }

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected override TableOrViewMetadata OnGetTable() => Table;
}