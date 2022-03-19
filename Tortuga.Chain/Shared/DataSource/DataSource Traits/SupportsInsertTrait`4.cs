using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits
{
	[Trait]
	class SupportsInsertTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsInsert
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
	{
		[Container(RegisterInterface = true)]
		internal IInsertHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;


		IObjectDbCommandBuilder<TArgument> ISupportsInsert.Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
		{
			return DataSource.OnInsertObject(DataSource.ParseObjectName(tableName), argumentValue, options);
		}

		IObjectDbCommandBuilder<TArgument> ISupportsInsert.Insert<TArgument>(TArgument argumentValue, InsertOptions options)
		{
			return DataSource.OnInsertObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
		}



		/// <summary>
		/// Inserts an object into the specified table.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options for how the insert occurs.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Insert<TArgument>(TArgument argumentValue, InsertOptions options = InsertOptions.None) where TArgument : class
		{
			return Insert(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);
		}


		/// <summary>
		/// Creates an operation used to perform an insert operation.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Insert<TArgument>(TObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
		where TArgument : class
		{
			return DataSource.OnInsertObject(tableName, argumentValue, options);
		}




	}
}



