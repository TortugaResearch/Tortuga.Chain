using System.Data.Common;

namespace Traits;

interface IInsertBulkHelper<TInsertBulkCommand, TCommand, TParameter, TObjectName, TDbType> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
	where TInsertBulkCommand : class
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
{
	public TInsertBulkCommand OnInsertBulk(TObjectName tableName, DataTable dataTable);

	public TInsertBulkCommand OnInsertBulk(TObjectName tableName, IDataReader dataReader);
}



