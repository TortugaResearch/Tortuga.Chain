using Npgsql;
using System;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlDataSourceBase.
    /// </summary>
    public abstract class PostgreSqlDataSourceBase : DataSource<NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand, NpgsqlParameter>, IClass1DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        public PostgreSqlDataSourceBase(DataSourceSettings settings) : base(settings)
        {
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        public abstract PostgreSqlMetadataCache DatabaseMetadata { get; }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Delete<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
        where TArgument : class
        {
            return new PostgreSqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
        }

        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName);
        }

        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, object filterValue)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, filterValue);
        }

        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, string whereClause)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, whereClause);
        }

        public TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> From(PostgreSqlObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new PostgreSqlTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey<T>(PostgreSqlObjectName tableName, IEnumerable<T> keys)
        {
            throw new NotImplementedException();
        }

        public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey<T>(PostgreSqlObjectName tableName, params T[] keys)
        {
            throw new NotImplementedException();
        }

        public SingleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> GetByKey<T>(PostgreSqlObjectName tableName, T key)
        {
            throw new NotImplementedException();
        }

        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Insert<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, InsertOptions options = InsertOptions.None)
        where TArgument : class
        {
            return new PostgreSqlInsertObject<TArgument>(this, tableName, argumentValue, options);
        }

        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Sql(string sqlStatement)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, null);
        }

        public MultipleTableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> Sql(string sqlStatement, object argumentValue)
        {
            return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
        }

        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Update<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpdateOptions options = UpdateOptions.None)
        where TArgument : class
        {
            return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        public ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> Upsert<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options = UpsertOptions.None)
        where TArgument : class
        {
            return new PostgreSqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            return From(tableOrViewName, filterValue);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKey(tableName, keys);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, params T[] keys)
        {
            return GetByKey(tableName, keys);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, T key)
        {
            return GetByKey(tableName, key);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Insert<TArgument>(string tableName, TArgument argumentValue, InsertOptions options)
        {
            return Insert(tableName, argumentValue, options);
        }

        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return Sql(sqlStatement, argumentValue);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Update<TArgument>(string tableName, TArgument argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
        }

        IObjectDbCommandBuilder<TArgument> IClass1DataSource.Upsert<TArgument>(string tableName, TArgument argumentValue, UpsertOptions options)
        {
            return Upsert(tableName, argumentValue, options);
        }



        //TODO: implement ClassXMetadata

    }
}
