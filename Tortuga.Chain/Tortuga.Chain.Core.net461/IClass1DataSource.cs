using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

#if !WINDOWS_UWP
using System;
//using System.Runtime.Caching;
#endif

namespace Tortuga.Chain
{
    /// <summary>
    /// A class 1 data source supports basic CRUD operations. This is the bare minimum needed to implement the repostiory pattern.
    /// </summary>
    public interface IClass1DataSource
    {

        //#if !WINDOWS_UWP
        //        /// <summary>
        //        /// Gets or sets the cache to be used by this data source. The default is .NET's MemoryCache.
        //        /// </summary>
        //        /// <remarks>This is used by the WithCaching materializer.</remarks>
        //        ObjectCache Cache { get; set; }
        //#endif

        /// <summary>
        /// Returns an abstract metadata cache.
        /// </summary>
        IDatabaseMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Deletes an object model from the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The delete options.</param>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        IDbCommandBuilder Delete(string tableName, object argumentValue, DeleteOptions options = DeleteOptions.None);

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <exception cref="ArgumentException">
        /// tableName is empty.;tableName
        /// or
        /// Table or view named + tableName +  could not be found. Check to see if the user has permissions to execute this procedure.
        /// </exception>
        IMultipleRowDbCommandBuilder From(string tableOrViewName);

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        IMultipleRowDbCommandBuilder From(string tableOrViewName, string whereClause);

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        IMultipleRowDbCommandBuilder From(string tableOrViewName, string whereClause, object argumentValue);


        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        IMultipleRowDbCommandBuilder From(string tableOrViewName, object filterValue);

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        ISingleRowDbCommandBuilder Insert(string tableName, object argumentValue);


        /// <summary>
        /// Performs an insert or update operation as appropriate.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the insert/update occurs.</param>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        ISingleRowDbCommandBuilder Upsert(string tableName, object argumentValue, UpsertOptions options = UpsertOptions.None);

        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        ISingleRowDbCommandBuilder Update(string tableName, object argumentValue, UpdateOptions options = UpdateOptions.None);

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        IMultipleTableDbCommandBuilder Sql(string sqlStatement, object argumentValue);
    }
}
