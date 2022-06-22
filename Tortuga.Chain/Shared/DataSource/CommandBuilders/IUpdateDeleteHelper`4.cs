using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits;

interface IUpdateDeleteHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	ObjectDbCommandBuilder<TCommand, TParameter, TArgument> OnUpdateObject<TArgument>(TObjectName tableName, TArgument argumentValue, UpdateOptions options)
		where TArgument : class;

	ObjectDbCommandBuilder<TCommand, TParameter, TArgument> OnDeleteObject<TArgument>(TObjectName tableName, TArgument argumentValue, DeleteOptions options)
	where TArgument : class;
}




