using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
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
        public ISingleRowDbCommandBuilder Delete(SqlServerObjectName tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            return new SqlServerDeleteObject(this, tableName, argumentValue, options);
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
        public IMultipleRowDbCommandBuilder From(SqlServerObjectName tableOrViewName)
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
        public IMultipleRowDbCommandBuilder From(SqlServerObjectName tableOrViewName, string whereClause)
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
        public IMultipleRowDbCommandBuilder From(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
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
        public IMultipleRowDbCommandBuilder From(SqlServerObjectName tableOrViewName, object filterValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue);
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

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue)
        {
            return Insert(tableName, argumentValue);
        }

        IMultipleTableDbCommandBuilder IClass1DataSource.Sql(string sqlStatement, object argumentValue)
        {
            return Sql(sqlStatement, argumentValue);
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
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public ISingleRowDbCommandBuilder Insert(SqlServerObjectName tableName, object argumentValue)
        {
            return new SqlServerInsertObject(this, tableName, argumentValue);
        }

        /// <summary>
        /// Loads the parameters from the ArgumentDictionary.
        /// </summary>
        /// <param name="argumentDictionary">The argument dictionary.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [Obsolete("This method will be removed in a future version.")]
        internal void LoadDictionaryParameters(IReadOnlyDictionary<string, object> argumentDictionary, List<SqlParameter> parameters, IEnumerable<ColumnMetadata<SqlDbType>> columns)
        {
            if (argumentDictionary == null)
                throw new ArgumentNullException("argumentDictionary", "argumentDictionary is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");
            if (columns == null)
                throw new ArgumentNullException("columns", "columns is null.");

            foreach (var item in columns)
            {
                var value = argumentDictionary[item.ClrName] ?? DBNull.Value;
                var parameter = new SqlParameter(item.SqlVariableName, value);
                if (item.DbType.HasValue)
                    parameter.SqlDbType = item.DbType.Value;
                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Loads the parameters from the ArgumentValue.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [Obsolete("This method will be removed in a future version.")]
        internal void LoadParameters(object argumentValue, List<SqlParameter> parameters, IEnumerable<ColumnPropertyMap<SqlDbType>> columns)
        {
            if (argumentValue == null)
                throw new ArgumentNullException("argumentValue", "argumentValue is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");
            if (columns == null)
                throw new ArgumentNullException("columns", "columns is null.");

            foreach (var item in columns)
            {
                var value = item.Property.InvokeGet(argumentValue) ?? DBNull.Value;
                var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                if (item.Column.DbType.HasValue)
                    parameter.SqlDbType = item.Column.DbType.Value;
                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public IMultipleTableDbCommandBuilder Procedure(SqlServerObjectName procedureName)
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
        public IMultipleTableDbCommandBuilder Procedure(SqlServerObjectName procedureName, object argumentValue)
        {
            return new SqlServerProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns></returns>
        public IMultipleTableDbCommandBuilder Sql(string sqlStatement)
        {
            return new SqlServerSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerSqlCall.</returns>
        public IMultipleTableDbCommandBuilder Sql(string sqlStatement, object argumentValue)
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
        public ISingleRowDbCommandBuilder Update(SqlServerObjectName tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
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
        public ISingleRowDbCommandBuilder Upsert(SqlServerObjectName tableName, object argumentValue, UpsertOptions options = UpsertOptions.None)
        {
            return new SqlServerInsertOrUpdateObject(this, tableName, argumentValue, options);
        }
    }
}

