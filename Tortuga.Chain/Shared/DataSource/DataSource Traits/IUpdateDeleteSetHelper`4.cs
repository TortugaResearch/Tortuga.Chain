using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits
{
	interface IUpdateDeleteSetHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
	   where TCommand : DbCommand
	   where TParameter : DbParameter
	   where TObjectName : struct
	   where TDbType : struct
	{


		IUpdateSetDbCommandBuilder<TCommand, TParameter> OnUpdateSet(TObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options);

		IUpdateSetDbCommandBuilder<TCommand, TParameter> OnUpdateSet(TObjectName tableName, object? newValues, UpdateOptions options);

	}
}



