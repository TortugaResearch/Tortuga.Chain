using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsFromTrait<TCommand, TParameter, TObjectName, TDbType, TLimitOption> : ISupportsFrom
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
	where TLimitOption : struct
{
	[Container(RegisterInterface = true)]
	internal IFromHelper<TCommand, TParameter, TObjectName, TDbType, TLimitOption> DataSource { get; set; } = null!;

	ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName)
	{
		return From(DataSource.DatabaseMetadata.ParseObjectName(tableOrViewName));
	}

	ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, object filterValue, FilterOptions filterOptions)
	{
		return From(DataSource.DatabaseMetadata.ParseObjectName(tableOrViewName), filterValue, filterOptions);
	}

	ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, string whereClause)
	{
		return From(DataSource.DatabaseMetadata.ParseObjectName(tableOrViewName), whereClause);
	}

	ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, string whereClause, object argumentValue)
	{
		return From(DataSource.DatabaseMetadata.ParseObjectName(tableOrViewName), whereClause, argumentValue);
	}

	ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>()
	{
		return From<TObject>();
	}

	ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(string whereClause)
	{
		return From<TObject>(whereClause);
	}

	ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(string whereClause, object argumentValue)
	{
		return From<TObject>(whereClause, argumentValue);
	}

	ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(object filterValue)
	{
		return From<TObject>(filterValue);
	}


	/// <summary>
	/// Creates an operation to directly query a table or view
	/// </summary>
	/// <param name="tableOrViewName"></param>
	/// <returns></returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> From(TObjectName tableOrViewName)
	{
		return DataSource.OnFromTableOrView<object>(tableOrViewName, null, null);
	}

	/// <summary>
	/// Creates an operation to directly query a table or view
	/// </summary>
	/// <param name="tableOrViewName"></param>
	/// <param name="whereClause"></param>
	/// <returns></returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> From(TObjectName tableOrViewName, string whereClause)
	{
		return DataSource.OnFromTableOrView<object>(tableOrViewName, whereClause, null);
	}

	/// <summary>
	/// Creates an operation to directly query a table or view
	/// </summary>
	/// <param name="tableOrViewName"></param>
	/// <param name="whereClause"></param>
	/// <param name="argumentValue"></param>
	/// <returns></returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> From(TObjectName tableOrViewName, string whereClause, object argumentValue)
	{
		return DataSource.OnFromTableOrView<object>(tableOrViewName, whereClause, argumentValue);
	}

	/// <summary>
	/// Creates an operation to directly query a table or view
	/// </summary>
	/// <param name="tableOrViewName">Name of the table or view.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <returns>TableDbCommandBuilder&lt;TCommand, TParameter, TLimitOption&gt;.</returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> From(TObjectName tableOrViewName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
	{
		return DataSource.OnFromTableOrView<object>(tableOrViewName, filterValue, filterOptions);
	}

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <returns></returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> From<TObject>()
		where TObject : class
	{
		return DataSource.OnFromTableOrView<TObject>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, null, null);
	}

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <returns></returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> From<TObject>(string whereClause)
		where TObject : class
	{
		return DataSource.OnFromTableOrView<TObject>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, whereClause, null);
	}

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
	/// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
	/// <returns></returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> From<TObject>(string whereClause, object argumentValue)
		where TObject : class
	{
		return DataSource.OnFromTableOrView<TObject>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, whereClause, argumentValue);
	}

	/// <summary>
	/// This is used to directly query a table or view.
	/// </summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <returns>TableDbCommandBuilder&lt;TCommand, TParameter, TLimitOption, TObject&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> From<TObject>(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		where TObject : class
	{
		return DataSource.OnFromTableOrView<TObject>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name, filterValue, filterOptions);
	}


}



