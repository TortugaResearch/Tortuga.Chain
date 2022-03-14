using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsDeleteByKeyList<TCommand, TParameter, TObjectName, TDbType> : ISupportsDeleteByKeyList, ISupportsDeleteByKey
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	[Owner(RegisterInterface = true)]
	internal ICommandHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	ISingleRowDbCommandBuilder ISupportsDeleteByKey.DeleteByKey<TKey>(string tableName, TKey key, DeleteOptions options)
	{
		throw new NotImplementedException();
	}

	ISingleRowDbCommandBuilder ISupportsDeleteByKey.DeleteByKey(string tableName, string key, DeleteOptions options)
	{
		throw new NotImplementedException();
	}

	IMultipleRowDbCommandBuilder ISupportsDeleteByKeyList.DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options)
	{
		throw new NotImplementedException();
	}
}

interface IDeleteByKeyHelper<TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	MultipleRowDbCommandBuilder<TCommand, TParameter> OnDeleteByKeyList<TKey>(TObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None);
}