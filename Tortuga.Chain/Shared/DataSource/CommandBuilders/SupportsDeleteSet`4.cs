using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
sealed class SupportsDeleteSet<TCommand, TParameter, TObjectName, TDbType> : ISupportsDeleteSet
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IUpdateDeleteSetHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IMultipleRowDbCommandBuilder ISupportsDeleteSet.DeleteSet(string tableName, string whereClause)
	{
		return DeleteSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), whereClause, null);
	}

	IMultipleRowDbCommandBuilder ISupportsDeleteSet.DeleteSet(string tableName, string whereClause, object? argumentValue)
	{
		return DeleteSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), whereClause, argumentValue);
	}

	IMultipleRowDbCommandBuilder ISupportsDeleteSet.DeleteSet(string tableName, object filterValue, FilterOptions filterOptions)
	{
		return DeleteSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), filterValue, filterOptions);
	}

	/// <summary>
	/// Delete multiple records using a filter object.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> DeleteSet(TObjectName tableName, object filterValue, FilterOptions filterOptions = FilterOptions.None)
	{
		var table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
		if (!DataSource.AuditRules.UseSoftDelete(table))
			return DataSource.OnDeleteSet(tableName, filterValue, filterOptions);

		return DataSource.OnUpdateSet(tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(filterValue, filterOptions);
	}

	/// <summary>
	/// Delete multiple records using a filter object.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The options.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> DeleteSet<TObject>(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		where TObject : class
	{
		var tableName = DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
		return DeleteSet(tableName, filterValue, filterOptions);
	}

	/// <summary>
	/// Delete multiple records using a where expression.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value for the where clause.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> DeleteSet(TObjectName tableName, string whereClause, object? argumentValue = null)
	{
		var table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
		if (!DataSource.AuditRules.UseSoftDelete(table))
			return DataSource.OnDeleteSet(tableName, whereClause, argumentValue);

		return DataSource.OnUpdateSet(tableName, null, UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected).WithFilter(whereClause, argumentValue);
	}

	/// <summary>
	/// Delete multiple records using a where expression.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value for the where clause.</param>
	[Expose]
	public MultipleRowDbCommandBuilder<TCommand, TParameter> DeleteSet<TObject>(string whereClause, object? argumentValue = null)
		where TObject : class
	{
		var tableName = DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name;
		return DeleteSet(tableName, whereClause, argumentValue);
	}
}
