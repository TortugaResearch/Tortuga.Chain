using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

partial class SqlServerAppenders
{


	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get all historical records.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> WithHistory<TObject>(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> tableDbCommand)
		where TObject : class
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(null, null, HistoryQueryMode.All);
		return tableDbCommand;
	}



	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get all historical records.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithHistory(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand)
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(null, null, HistoryQueryMode.All);
		return tableDbCommand;
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get all historical records.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	public static MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> WithHistory<TObject>(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> tableDbCommand)
		where TObject : class
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand.GetInterface<ISupportsHistory>();
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(null, null, HistoryQueryMode.All);

		return tableDbCommand.RewrapAsMultipleRow();
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get all historical records.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	public static MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> WithHistory(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> tableDbCommand)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand as ISupportsHistory;
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(null, null, HistoryQueryMode.All);

		return (MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter>)tableDbCommand;
	}


	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="asOfDate">As Of Date to use</param>
	public static SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> WithHistory(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> tableDbCommand, DateTime asOfDate)
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand as ISupportsHistory;
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(asOfDate, null, HistoryQueryMode.AsOfDate);

		return tableDbCommand;
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="asOfDate">As Of Date to use</param>
	public static SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> WithHistory<TObject>(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> tableDbCommand, DateTime asOfDate)
		where TObject : class
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand.GetInterface<ISupportsHistory>();
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(asOfDate, null, HistoryQueryMode.AsOfDate);

		return tableDbCommand;
	}


	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date range.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="fromDate">The lower date range.</param>
	/// <param name="toDate">The upper date range.</param>
	/// <param name="mode">The mode determines how the bounds checking will be applied (i.e. inclusive or exclusive).</param>
	public static MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> WithHistory(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> tableDbCommand, DateTime fromDate, DateTime toDate, HistoryQueryMode mode)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand as ISupportsHistory;
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(fromDate, toDate, mode);

		return (MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter>)tableDbCommand;
	}


	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date range.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="fromDate">The lower date range.</param>
	/// <param name="toDate">The upper date range.</param>
	/// <param name="mode">The mode determines how the bounds checking will be applied (i.e. inclusive or exclusive).</param>
	public static MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> WithHistory<TObject>(this SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter, TObject> tableDbCommand, DateTime fromDate, DateTime toDate, HistoryQueryMode mode)
		where TObject : class
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		var history = tableDbCommand.GetInterface<ISupportsHistory>();
		if (history == null)
			throw new InvalidOperationException("History options are not supported by this command builder.");
		history.AddHistoryClause(fromDate, toDate, mode);

		return tableDbCommand.RewrapAsMultipleRow();
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="asOfDate">As Of Date to use</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> WithHistory<TObject>(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> tableDbCommand, DateTime asOfDate)
		where TObject : class
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(asOfDate, null, HistoryQueryMode.AsOfDate);
		return tableDbCommand;
	}


	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="asOfDate">As Of Date to use</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithHistory(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, DateTime asOfDate)
	{

		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(asOfDate, null, HistoryQueryMode.AsOfDate);
		return tableDbCommand;
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date range.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="fromDate">The lower date range.</param>
	/// <param name="toDate">The upper date range.</param>
	/// <param name="mode">The mode determines how the bounds checking will be applied (i.e. inclusive or exclusive).</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> WithHistory(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption> tableDbCommand, DateTime fromDate, DateTime toDate, HistoryQueryMode mode)
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(fromDate, toDate, mode);
		return tableDbCommand;
	}

	/// <summary>
	/// Adds a FOR SYSTEM_TIME cluse to the query to get the historical record for a specific date range.
	/// </summary>
	/// <param name="tableDbCommand">The table database command.</param>
	/// <param name="fromDate">The lower date range.</param>
	/// <param name="toDate">The upper date range.</param>
	/// <param name="mode">The mode determines how the bounds checking will be applied (i.e. inclusive or exclusive).</param>
	public static TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> WithHistory<TObject>(this TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> tableDbCommand, DateTime fromDate, DateTime toDate, HistoryQueryMode mode)
		where TObject : class
	{
		if (tableDbCommand == null)
			throw new ArgumentNullException(nameof(tableDbCommand), $"{nameof(tableDbCommand)} is null.");

		((ISupportsHistory)tableDbCommand).AddHistoryClause(fromDate, toDate, mode);
		return tableDbCommand;
	}




}
