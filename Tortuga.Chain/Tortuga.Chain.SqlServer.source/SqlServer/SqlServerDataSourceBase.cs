using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerDataSourceBase.
    /// </summary>
    public abstract class SqlServerDataSourceBase : DataSource<SqlCommand, SqlParameter>, IClass2DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings value.</param>
        protected SqlServerDataSourceBase(SqlServerDataSourceSettings settings) : base(settings)
        {

        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract SqlServerMetadataCache DatabaseMetadata { get; }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> Delete(SqlServerObjectName tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            var table = DatabaseMetadata.GetTableOrView(tableName);
            if (!AuditRules.UseSoftDelete(table))
                return new SqlServerDeleteObject(this, tableName, argumentValue, options);

            UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

            return new SqlServerUpdateObject(this, tableName, argumentValue, effectiveOptions);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// tableName is empty.;tableName
        /// or
        /// Table or view named + tableName +  could not be found. Check to see if the user has permissions to execute this procedure.
        /// </exception>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> From(SqlServerObjectName tableOrViewName)
        {
            return new SqlServerTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <returns>SqlServerTableOrView.</returns>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> From(SqlServerObjectName tableOrViewName, string whereClause)
        {
            return new SqlServerTableOrView(this, tableOrViewName, whereClause, null);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns>SqlServerTableOrView.</returns>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> From(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns>SqlServerTableOrView.</returns>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> From(SqlServerObjectName tableOrViewName, object filterValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<T>(SqlServerObjectName tableName, T key)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"GetByKey operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            var columnMetadata = primaryKeys.Single();
            var where = columnMetadata.SqlName + " = " + columnMetadata.SqlVariableName;

            var parameters = new List<SqlParameter>();

            var param = new SqlParameter(columnMetadata.SqlVariableName, key);
            if (columnMetadata.DbType.HasValue)
                param.SqlDbType = columnMetadata.DbType.Value;
            parameters.Add(param);


            return new SqlServerTableOrView(this, tableName, where, parameters);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<T>(SqlServerObjectName tableName, params T[] keys)
        {
            return GetByKey(tableName, (IEnumerable<T>)keys);
        }

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> GetByKey<T>(SqlServerObjectName tableName, IEnumerable<T> keys)
        {
            var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).Columns.Where(c => c.IsPrimaryKey).ToList();
            if (primaryKeys.Count != 1)
                throw new MappingException($"GetByKey operation isn't allowed on {tableName} because it doesn't have a single primary key. Use DataSource.From instead.");

            var keyList = keys.AsList();
            var columnMetadata = primaryKeys.Single();
            var where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";

            var parameters = new List<SqlParameter>();
            for (var i = 0; i < keyList.Count; i++)
            {
                var param = new SqlParameter("@Param" + i, keyList[i]);
                if (columnMetadata.DbType.HasValue)
                    param.SqlDbType = columnMetadata.DbType.Value;
                parameters.Add(param);
            }

            return new SqlServerTableOrView(this, tableName, where, parameters);
        }

        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return Sql(sqlStatement, argumentValue);
        }

        IDbCommandBuilder IClass1DataSource.Delete(string tableName, object argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            return From(tableOrViewName, filterValue);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, T key)
        {
            return GetByKey(tableName, key);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, IEnumerable<T> keys)
        {
            return GetByKey(tableName, keys);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, params T[] keys)
        {
            return GetByKey(tableName, (IEnumerable<T>)keys);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue, InsertOptions options)
        {
            return Insert(tableName, argumentValue, options);
        }
        ISingleRowDbCommandBuilder IClass1DataSource.Update(string tableName, object argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Upsert(string tableName, object argumentValue, UpsertOptions options)
        {
            return Upsert(tableName, argumentValue, options);
        }

        ILink IClass2DataSource.BulkInsert<T>(string tableName, IEnumerable<T> values)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        ILink IClass2DataSource.BulkInsert(string tableName, DataTable values)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        ILink IClass2DataSource.BulkInsert(string tableName, IDataReader values)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        IMultipleTableDbCommandBuilder IClass2DataSource.Procedure(string procedureName)
        {
            return Procedure(procedureName);
        }

        IMultipleTableDbCommandBuilder IClass2DataSource.Procedure(string procedureName, object argumentValue)
        {
            return Procedure(procedureName, argumentValue);
        }

        IMultipleRowDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        IMultipleRowDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue, object filterValue)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        IMultipleRowDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue, string whereClause)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        IMultipleRowDbCommandBuilder IClass2DataSource.TableFunction(string functionName, object functionArgumentValue, string whereClause, object whereClauseArgumentValue)
        {
            throw new NotImplementedException("This feature is planned for a future version");
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// SqlServerInsert.
        /// </returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> Insert(SqlServerObjectName tableName, object argumentValue, InsertOptions options = InsertOptions.None)
        {
            return new SqlServerInsertObject(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(SqlServerObjectName procedureName)
        {
            return new SqlServerProcedureCall(this, procedureName, null);
        }


        /// <summary>
        /// Loads a procedure definition and populates it using the parameter object.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        /// <remarks>
        /// The procedure's definition is loaded from the database and used to determine which properties on the parameter object to use.
        /// </remarks>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Procedure(SqlServerObjectName procedureName, object argumentValue)
        {
            return new SqlServerProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns></returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Sql(string sqlStatement)
        {
            return new SqlServerSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerSqlCall.</returns>
        public MultipleTableDbCommandBuilder<SqlCommand, SqlParameter> Sql(string sqlStatement, object argumentValue)
        {
            return new SqlServerSqlCall(this, sqlStatement, argumentValue);
        }
        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> Update(SqlServerObjectName tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SqlServerUpdateObject(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns>SqlServerUpdate.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SingleRowDbCommandBuilder<SqlCommand, SqlParameter> Upsert(SqlServerObjectName tableName, object argumentValue, UpsertOptions options = UpsertOptions.None)
        {
            return new SqlServerInsertOrUpdateObject(this, tableName, argumentValue, options);
        }
    }
}

