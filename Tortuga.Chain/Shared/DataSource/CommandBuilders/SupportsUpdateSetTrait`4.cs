using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsUpdateSetTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsUpdateSet
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{
	[Container(RegisterInterface = true)]
	internal IUpdateDeleteSetHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IUpdateSetDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, string updateExpression, UpdateOptions options)
	{
		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), updateExpression, options);
	}

	IUpdateSetDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
	{
		//`updateArgumentValue` isn't optional because it messes up method overloading with the `options` parameter.

		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), updateExpression, updateArgumentValue, options);
	}

	IUpdateSetDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, object newValues, UpdateOptions options)
	{
		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.ParseObjectName(tableName), newValues, options);
	}

	/// <summary>
	/// Update multiple records using an update expression.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="updateExpression">The update expression.</param>
	/// <param name="updateArgumentValue">The argument value.</param>
	/// <param name="options">The update options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet(TObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options = UpdateOptions.None)
	{
		//`updateArgumentValue` isn't optional because it messes up method overloading with the `options` parameter.

		return DataSource.OnUpdateSet(tableName, updateExpression, updateArgumentValue, options);
	}

	/// <summary>
	/// Update multiple records using an update value.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="newValues">The new values to use.</param>
	/// <param name="options">The options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet(TObjectName tableName, object newValues, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateSet(tableName, newValues, options);
	}

	/// <summary>
	/// Update multiple records using an update expression.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="updateExpression">The update expression.</param>
	/// <param name="options">The update options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet(TObjectName tableName, string updateExpression, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateSet(tableName, updateExpression, null, options);
	}

	/// <summary>
	/// Update multiple records using an update expression.
	/// </summary>
	/// <typeparam name="TObject">Class used to determine the table name.</typeparam>
	/// <param name="updateExpression">The update expression.</param>
	/// <param name="updateArgumentValue">The argument for the update expression.</param>
	/// <param name="options">The update options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet<TObject>(string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None)
	{
		//`updateArgumentValue` isn't optional because it messes up method overloading with the `options` parameter.

		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, updateExpression, updateArgumentValue, options);
	}

	/// <summary>
	/// Update multiple records using an update value.
	/// </summary>
	/// <typeparam name="TObject">Class used to determine the table name.</typeparam>
	/// <param name="newValues">The new values to use.</param>
	/// <param name="options">The options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet<TObject>(object newValues, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, newValues, options);
	}

	/// <summary>
	/// Update multiple records using an update expression.
	/// </summary>
	/// <typeparam name="TObject">Class used to determine the table name.</typeparam>
	/// <param name="updateExpression">The update expression.</param>
	/// <param name="options">The update options.</param>
	/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
	[Expose]
	public IUpdateSetDbCommandBuilder<TCommand, TParameter> UpdateSet<TObject>(string updateExpression, UpdateOptions options = UpdateOptions.None)
	{
		return DataSource.OnUpdateSet(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, updateExpression, null, options);
	}
}
