#if !OleDb_Missing

using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase : IClass2DataSource
    {
        IDatabaseMetadataCache IDataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return Sql(sqlStatement, argumentValue);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Delete<TArgument>(TArgument argumentValue, DeleteOptions options)
        {
            return Delete(argumentValue, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKeyList<T>(string tableName, string keyColumn, IEnumerable<T> keys)
        {
            return GetByKeyList(tableName, keyColumn, keys);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue, FilterOptions filterOptions)
        {
            return From(tableOrViewName, filterValue, filterOptions);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>()
        {
            return From<TObject>();
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(string whereClause)
        {
            return From<TObject>(whereClause);
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(string whereClause, object argumentValue)
        {
            return From<TObject>(whereClause, argumentValue);
        }

        ITableDbCommandBuilder IClass1DataSource.From<TObject>(object filterValue)
        {
            return From<TObject>(filterValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey<TKey>(string tableName, TKey key)
        {
            return GetByKey(tableName, key);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey(string tableName, string key)
        {
            return GetByKey(tableName, key);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKeyList<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKeyList(tableName, keys);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
        {
            return Insert(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Insert<TArgument>(TArgument argumentValue, InsertOptions options)
        {
            return Insert(argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Update<TArgument>(TArgument argumentValue, UpdateOptions options)
        {
            return Update(argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
        {
            return Upsert(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Upsert<TArgument>(TArgument argumentValue, UpsertOptions options)
        {
            return Upsert(argumentValue, options);
        }

        IMultipleTableDbCommandBuilder IClass2DataSource.Procedure(string procedureName)
        {
            return Procedure(procedureName);
        }

        IMultipleTableDbCommandBuilder IClass2DataSource.Procedure(string procedureName, object argumentValue)
        {
            return Procedure(procedureName, argumentValue);
        }

        ITableDbCommandBuilder IClass2DataSource.TableFunction(string functionName)
        {
            return TableFunction(functionName);
        }

        ITableDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue)
        {
            return TableFunction(functionName, functionArgumentValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.DeleteByKey<TKey>(string tableName, TKey key, DeleteOptions options)
        {
            return DeleteByKey(tableName, key, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.DeleteByKey(string tableName, string key, DeleteOptions options)
        {
            return DeleteByKey(tableName, key, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.DeleteByKeyList<TKey>(string tableName, IEnumerable<TKey> keys, DeleteOptions options)
        {
            return DeleteByKeyList(tableName, keys, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.UpdateByKey<TArgument, TKey>(string tableName, TArgument newValues, TKey key, UpdateOptions options)
        {
            return UpdateByKey(tableName, newValues, key, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.UpdateByKey<TArgument>(string tableName, TArgument newValues, string key, UpdateOptions options)
        {
            return UpdateByKey(tableName, newValues, key, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.UpdateByKeyList<TArgument, TKey>(string tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options)
        {
            return UpdateByKeyList(tableName, newValues, keys, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.DeleteWithFilter(string tableName, string whereClause)
        {
            return DeleteWithFilter(tableName, whereClause);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.DeleteWithFilter(string tableName, string whereClause, object argumentValue)
        {
            return DeleteWithFilter(tableName, whereClause, argumentValue);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.DeleteWithFilter(string tableName, object filterValue, FilterOptions filterOptions)
        {
            return DeleteWithFilter(tableName, filterValue, filterOptions);
        }

        IUpdateManyCommandBuilder IClass1DataSource.UpdateSet(string tableName, string updateExpression, UpdateOptions options)
        {
            return UpdateSet(tableName, updateExpression, options);
        }

        IUpdateManyCommandBuilder IClass1DataSource.UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options)
        {
            return UpdateSet(tableName, updateExpression, updateArgumentValue, options);
        }

        IUpdateManyCommandBuilder IClass1DataSource.UpdateSet(string tableName, object newValues, UpdateOptions options)
        {
            return UpdateSet(tableName, newValues, options);
        }

        IScalarDbCommandBuilder IClass2DataSource.ScalarFunction(string scalarFunctionName)
        {
            return ScalarFunction(scalarFunctionName);
        }

        IScalarDbCommandBuilder IClass2DataSource.ScalarFunction(string scalarFunctionName, object functionArgumentValue)
        {
            return ScalarFunction(scalarFunctionName, functionArgumentValue);
        }
    }
}

#endif