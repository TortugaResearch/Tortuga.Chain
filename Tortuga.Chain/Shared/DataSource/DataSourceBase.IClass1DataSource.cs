using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase : ICrudDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase : ICrudDataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase : ICrudDataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase : ICrudDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase : ICrudDataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase : ICrudDataSource

#endif
	{

		IMultipleRowDbCommandBuilder ISupportsDeleteWithFilter.DeleteWithFilter(string tableName, string whereClause)
		{
			return DeleteWithFilter(tableName, whereClause);
		}

		IMultipleRowDbCommandBuilder ISupportsDeleteWithFilter.DeleteWithFilter(string tableName, string whereClause, object argumentValue)
		{
			return DeleteWithFilter(tableName, whereClause, argumentValue);
		}

		IMultipleRowDbCommandBuilder ISupportsDeleteWithFilter.DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions)
		{
			return DeleteWithFilter(tableName, filterValue, filterOptions);
		}

		ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName)
		{
			return From(tableOrViewName);
		}

		ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, object filterValue, FilterOptions filterOptions)
		{
			return From(tableOrViewName, filterValue, filterOptions);
		}

		ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, string whereClause)
		{
			return From(tableOrViewName, whereClause);
		}

		ITableDbCommandBuilder ISupportsFrom.From(string tableOrViewName, string whereClause, object argumentValue)
		{
			return From(tableOrViewName, whereClause, argumentValue);
		}

		ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>()
		{
			return From<TObject>();
		}

		ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(string whereClause)
		{
			return From<TObject>(whereClause);
		}

		ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(string whereClause, object argumentValue)
		{
			return From<TObject>(whereClause, argumentValue);
		}

		ITableDbCommandBuilder<TObject> ISupportsFrom.From<TObject>(object filterValue)
		{
			return From<TObject>(filterValue);
		}

		ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey<TKey>(string tableName, TKey key)
		{
			return GetByKey(tableName, key);
		}

		ISingleRowDbCommandBuilder ISupportsGetByKey.GetByKey(string tableName, string key)
		{
			return GetByKey(tableName, key);
		}

		IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
		{
			return GetByKeyList(tableName, keyColumn, keys);
		}

		IMultipleRowDbCommandBuilder ISupportsGetByKeyList.GetByKeyList<T>(string tableName, IEnumerable<T> keys)
		{
			return GetByKeyList(tableName, keys);
		}

		IObjectDbCommandBuilder<TArgument> ISupportsInsert.Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
		{
			return Insert(tableName, argumentValue, options);
		}

		IObjectDbCommandBuilder<TArgument> ISupportsInsert.Insert<TArgument>(TArgument argumentValue, InsertOptions options)
		{
			return Insert(argumentValue, options);
		}



		IUpdateManyDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, string updateExpression, UpdateOptions options)
		{
			return UpdateSet(tableName, updateExpression, options);
		}

		IUpdateManyDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
		{
			return UpdateSet(tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder ISupportsUpdateSet.UpdateSet(string tableName, object newValues, UpdateOptions options)
		{
			return UpdateSet(tableName, newValues, options);
		}
	}
}
