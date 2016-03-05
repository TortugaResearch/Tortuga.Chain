using System;
using System.Data.SqlClient;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerDataSourceBase.
    /// </summary>
    public abstract class SqlServerDataSourceBase : DataSource<SqlCommand, SqlParameter>, IClass1DataSource
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
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public SqlServerProcedureCall Procedure(SqlServerObjectName procedureName)
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
        public SqlServerProcedureCall Procedure(SqlServerObjectName procedureName, object argumentValue)
        {
            return new SqlServerProcedureCall(this, procedureName, argumentValue);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns></returns>
        public SqlServerSqlCall Sql(string sqlStatement)
        {
            return new SqlServerSqlCall(this, sqlStatement, null);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerSqlCall.</returns>
        public SqlServerSqlCall Sql(string sqlStatement, object argumentValue)
        {
            return new SqlServerSqlCall(this, sqlStatement, argumentValue);
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
        public SqlServerTableOrView From(SqlServerObjectName tableOrViewName)
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
        public SqlServerTableOrView From(SqlServerObjectName tableOrViewName, string whereClause)
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
        public SqlServerTableOrView From(SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
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
        public SqlServerTableOrView From(SqlServerObjectName tableOrViewName, object filterValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// This is used to query a table value function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table or view.</param>
        /// <param name="whereClause">An optional where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">An optional argument value.</param>
        /// <returns></returns>
        public SqlServerTableFunction FromFunction(SqlServerObjectName tableFunctionName, string whereClause, object argumentValue)
        {
            return new SqlServerTableFunction(this, tableFunctionName, whereClause, argumentValue);
        }

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerInsertObject Insert(SqlServerObjectName tableName, object argumentValue)
        {
            return new SqlServerInsertObject(this, tableName, argumentValue);
        }

        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerUpdateObject Update(SqlServerObjectName tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
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
        public SqlServerInsertOrUpdateObject InsertOrUpdate(SqlServerObjectName tableName, object argumentValue, InsertOrUpdateOptions options = InsertOrUpdateOptions.None)
        {
            return new SqlServerInsertOrUpdateObject(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerDeleteObject Delete(SqlServerObjectName tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            return new SqlServerDeleteObject(this, tableName, argumentValue, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue)
        {
            return Insert(tableName, argumentValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Update(string tableName, object argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
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

        ISingleRowDbCommandBuilder IClass1DataSource.InsertOrUpdate(string tableName, object argumentValue, InsertOrUpdateOptions options)
        {
            return InsertOrUpdate(tableName, argumentValue, options);
        }


    }
}
