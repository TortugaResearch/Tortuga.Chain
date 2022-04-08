using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the DeleteSet command.
/// </summary>
public interface ISupportsDeleteSet
{
	/// <summary>
	/// Delete multiple records using a where expression.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <returns>IMultipleRowDbCommandBuilder.</returns>
	IMultipleRowDbCommandBuilder DeleteSet(string tableName, string whereClause);

	/// <summary>
	/// Delete multiple records using a where expression.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value for the where clause.</param>
	/// <returns>IMultipleRowDbCommandBuilder.</returns>
	IMultipleRowDbCommandBuilder DeleteSet(string tableName, string whereClause, object? argumentValue);

	/// <summary>
	/// Delete multiple records using a filter object.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	IMultipleRowDbCommandBuilder DeleteSet(string tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None);
}
