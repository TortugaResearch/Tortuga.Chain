using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits
{
	interface IInsertBatchHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObjectName : struct
		where TDbType : struct
	{
		//This is needed because the trait generator can't create a matching Func<...> property with the TObject constraint.
		DbCommandBuilder<TCommand, TParameter> OnInsertBatch<TObject>(TObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	where TObject : class;

	}
}
