using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsUpsertTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsUpsert
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{

	[Container(RegisterInterface = true)]
	internal IUpsertHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;



	IObjectDbCommandBuilder<TArgument> ISupportsUpsert.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
	{
		return DataSource.OnInsertOrUpdateObject(DataSource.DatabaseMetadata.ParseObjectName(tableName), argumentValue, options);
	}

	IObjectDbCommandBuilder<TArgument> ISupportsUpsert.Upsert<TArgument>(TArgument argumentValue, UpsertOptions options)
	{
		return DataSource.OnInsertOrUpdateObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
	}


	/// <summary>
	/// Creates a operation used to perform an "upsert" operation.
	/// </summary>
	/// <param name="tableName"></param>
	/// <param name="argumentValue"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	[Expose]
	public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Upsert<TArgument>(TObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
	where TArgument : class
	{
		return DataSource.OnInsertOrUpdateObject(tableName, argumentValue, options);
	}

	/// <summary>
	/// Perform an insert or update operation as appropriate.
	/// </summary>
	/// <typeparam name="TArgument"></typeparam>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options for how the insert/update occurs.</param>
	/// <returns></returns>
	[Expose]
	public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Upsert<TArgument>(TArgument argumentValue, UpsertOptions options = UpsertOptions.None) where TArgument : class
	{
		return DataSource.OnInsertOrUpdateObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
	}

}



