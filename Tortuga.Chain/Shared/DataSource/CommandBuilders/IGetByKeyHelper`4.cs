using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Traits
{
	interface IGetByKeyHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObjectName : struct
		where TDbType : struct
	{
		SingleRowDbCommandBuilder<TCommand, TParameter> OnGetByKey<TObject, TKey>(TObjectName tableName, ColumnMetadata<TDbType> keyColumn, TKey key)
			where TObject : class;

		MultipleRowDbCommandBuilder<TCommand, TParameter> OnGetByKeyList<TObject, TKey>(TObjectName tableName, ColumnMetadata<TDbType> keyColumn, IEnumerable<TKey> keys)
			where TObject : class;
	}
}



