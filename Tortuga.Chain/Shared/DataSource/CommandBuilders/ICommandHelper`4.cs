using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Traits;

interface ICommandHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TParameter, TObjectName, TDbType>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	List<TParameter> GetParameters(SqlBuilder<TDbType> builder);
}
