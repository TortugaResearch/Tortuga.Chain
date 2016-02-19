using System.Data.SqlClient;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerDataSourceBase.
    /// </summary>
    public abstract class SqlServerDataSourceBase : DataSource<SqlCommand, SqlParameter>
    {

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract SqlServerMetadataCache DatabaseMetadata { get; }
        /// <summary>
        /// Loads a procedure definition
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public SqlServerProcedureCall Procedure(SqlServerObjectName procedureName)
        {
            return new SqlServerProcedureCall(this, procedureName);
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
        /// <exception cref="System.ArgumentException">
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
        /// <exception cref="System.ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
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
        /// <exception cref="System.ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
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
        /// <exception cref="System.ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        public SqlServerTableOrView From(SqlServerObjectName tableOrViewName, object filterValue)
        {
            return new SqlServerTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// This is used to query a table value function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// tableName is empty.;tableName
        /// or
        /// Table or view named + tableName +  could not be found. Check to see if the user has permissions to execute this procedure.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">argumentValue;argumentValue is null.</exception>
        public SqlServerTableFunction FromTableFunction(SqlServerObjectName tableFunctionName, string whereClause, object argumentValue)
        {
            return new SqlServerTableFunction(this, tableFunctionName, whereClause, argumentValue);
        }

        /// <summary>
        /// Inserts an object model into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="System.ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerInsertModel Insert(SqlServerObjectName tableName, object argumentValue)
        {
            return new SqlServerInsertModel(this, tableName, argumentValue);
        }

        /// <summary>
        /// Updates an object model in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="System.ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerUpdateModel Update(SqlServerObjectName tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SqlServerUpdateModel(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <returns>SqlServerUpdate.</returns>
        /// <exception cref="System.ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerInsertOrUpdateModel InsertOrUpdate(SqlServerObjectName tableName, object argumentValue, InsertOrUpdateOptions options = InsertOrUpdateOptions.None)
        {
            return new SqlServerInsertOrUpdateModel(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Inserts an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <returns>SqlServerInsert.</returns>
        /// <exception cref="System.ArgumentException">tableName is empty.;tableName</exception>
        public SqlServerDeleteModel Delete(SqlServerObjectName tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            return new SqlServerDeleteModel(this, tableName, argumentValue, options);
        }

    }
}
