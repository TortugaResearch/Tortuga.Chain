using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits;

interface IUpsertHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	ObjectDbCommandBuilder<TCommand, TParameter, TArgument> OnInsertOrUpdateObject<TArgument>(TObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class;

}



