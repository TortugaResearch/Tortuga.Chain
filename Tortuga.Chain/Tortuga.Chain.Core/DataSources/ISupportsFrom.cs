using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support the From command.
/// </summary>
public interface ISupportsFrom
{
	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <exception cref="ArgumentException">
	/// tableName is empty.;tableName
	/// or
	/// Table or view named + tableName +  could not be found. Check to see if the user has permissions to execute this procedure.
	/// </exception>
	ITableDbCommandBuilder From(string tableOrViewName);

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
	ITableDbCommandBuilder From(string tableOrViewName, string whereClause);

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
	/// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
	ITableDbCommandBuilder From(string tableOrViewName, string whereClause, object argumentValue);

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <returns>ITableDbCommandBuilder.</returns>
	/// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
	ITableDbCommandBuilder From(string tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None);

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	ITableDbCommandBuilder<TObject> From<TObject>() where TObject : class;

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <returns></returns>
	ITableDbCommandBuilder<TObject> From<TObject>(string whereClause) where TObject : class;

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
	/// <returns></returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	ITableDbCommandBuilder<TObject> From<TObject>(string whereClause, object argumentValue) where TObject : class;

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
	/// <returns></returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	ITableDbCommandBuilder<TObject> From<TObject>(object filterValue) where TObject : class;
}
