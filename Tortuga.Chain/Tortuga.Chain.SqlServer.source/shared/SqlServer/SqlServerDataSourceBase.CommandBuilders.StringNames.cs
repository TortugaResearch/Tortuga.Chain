using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
    //Methods in this file should always delegate the same method using SqlServerObjectName instead of a string for the table name.

    partial class SqlServerDataSourceBase
    {
        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(string tableName, DataTable dataTable, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch(new SqlServerObjectName(tableName), dataTable, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(string tableName, DbDataReader dataReader, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch(new SqlServerObjectName(tableName), dataReader, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation..
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(string tableName, IEnumerable<TObject> objects, string tableTypeName, InsertOptions options = InsertOptions.None)
        {
            return InsertBatch<TObject>(new SqlServerObjectName(tableName), objects, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records as one operation..
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="tableTypeName">Name of the table type.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.
        /// </returns>
        public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(IEnumerable<TObject> objects, string tableTypeName, InsertOptions options = InsertOptions.None) where TObject : class
        {
            return InsertBatch<TObject>(objects, new SqlServerObjectName(tableTypeName), options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(string tableName, DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return InsertBulk(new SqlServerObjectName(tableName), dataTable, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        public SqlServerInsertBulk InsertBulk(string tableName, IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            return InsertBulk(new SqlServerObjectName(tableName), dataReader, options);
        }

        /// <summary>
        /// Inserts the batch of records using bulk insert.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="options">The options.</param>
        /// <returns>SqlServerInsertBulk.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public SqlServerInsertBulk InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
        {
            return InsertBulk<TObject>(new SqlServerObjectName(tableName), objects, options);
        }
    }
}