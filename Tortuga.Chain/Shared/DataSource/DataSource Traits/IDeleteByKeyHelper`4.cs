using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits
{
	interface IDeleteByKeyHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObjectName : struct
		where TDbType : struct
	{
		MultipleRowDbCommandBuilder<TCommand, TParameter> OnDeleteByKeyList<TKey>(TObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options);
	}
}



