using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

partial class SqlServerAppenders
{



	/// <summary>
	/// Overrides which index will be used for a query.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="indexName">The name of the index.</param>
	/// <remarks>This method does not protect against SQL injection attacks.</remarks>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithIndex(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, string indexName)
	{
		if (string.IsNullOrEmpty(indexName))
			throw new ArgumentException($"{nameof(indexName)} is null or empty.", nameof(indexName));
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var command = tableDbCommand as ISupportsTableHints;
		if (command == null)
			throw new ArgumentException($"The command {tableDbCommand.GetType().FullName} does not support table hints.", nameof(tableDbCommand));

		//We can't univerally verify the index exists because it may be on an indexed view. Or the index is on a table and the query is against a view.

		command.AddTableHint($"INDEX ([{indexName}])");

		return tableDbCommand;
	}

	/// <summary>
	/// Overrides which index will be used for a query.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="indexName">The name of the index.</param>
	/// <remarks>This method does not protect against SQL injection attacks.</remarks>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> WithIndex<TObject>(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> tableDbCommand, string indexName)
	where TObject : class
	{
		WithIndex((TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption>)tableDbCommand, indexName);
		return tableDbCommand;
	}

	/// <summary>
	/// Overrides which index will be used for a query.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="index">The index to use.</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithIndex(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, IndexMetadata<AbstractObjectName, AbstractDbType> index)
	{
		if (index == null)
			throw new ArgumentNullException(nameof(index), $"{nameof(index)} is null.");

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var command = tableDbCommand as ISupportsTableHints;
		if (command == null)
			throw new ArgumentException($"The command {tableDbCommand.GetType().FullName} does not support table hints.", nameof(tableDbCommand));

		command.AddTableHint($"INDEX ([{index.Name}])");

		return tableDbCommand;
	}

	/// <summary>
	/// Overrides which index will be used for a query.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="index">The index to use.</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> WithIndex<TObject>(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> tableDbCommand, IndexMetadata<AbstractObjectName, AbstractDbType> index)
	where TObject : class
	{
		WithIndex((TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption>)tableDbCommand, index);
		return tableDbCommand;
	}
}