using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsUpdateTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsUpdate
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{

	[Container(RegisterInterface = true)]
	internal IUpdateDeleteHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IObjectDbCommandBuilder<TArgument> ISupportsUpdate.Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
		=> DataSource.OnUpdateObject(DataSource.ParseObjectName(tableName), argumentValue, options);

	IObjectDbCommandBuilder<TArgument> ISupportsUpdate.Update<TArgument>(TArgument argumentValue, UpdateOptions options)
		=> DataSource.OnUpdateObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);


	/// <summary>
	/// Update an object in the specified table.
	/// </summary>
	/// <typeparam name="TArgument"></typeparam>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The update options.</param>
	/// <returns></returns>
	[Expose]
	public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Update<TArgument>(TArgument argumentValue, UpdateOptions options = UpdateOptions.None) where TArgument : class
		=> DataSource.OnUpdateObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);



	/// <summary>
	/// Update an object in the specified table.
	/// </summary>
	/// <param name="tableName"></param>
	/// <param name="argumentValue"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	[Expose]
	public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Update<TArgument>(TObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
		where TArgument : class
		=> DataSource.OnUpdateObject(tableName, argumentValue, options);

}



