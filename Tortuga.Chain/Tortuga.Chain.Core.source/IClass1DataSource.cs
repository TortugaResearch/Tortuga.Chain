using System;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

#if !WINDOWS_UWP
//using System.Runtime.Caching;
#endif

namespace Tortuga.Chain
{

    /// <summary>
    /// A class 1 data source supports basic CRUD operations. This is the bare minimum needed to implement the repostiory pattern.
    /// </summary>
    /// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.</remarks>
    public interface IClass1DataSource : IClass0DataSource
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
        ITableDbCommandBuilder From(string tableOrViewName);

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        ITableDbCommandBuilder From(string tableOrViewName, string whereClause);

        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause. Do not prefix this clause with "WHERE".</param>
        /// <param name="argumentValue">Optional argument value. Every property in the argument value must have a matching parameter in the WHERE clause</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        ITableDbCommandBuilder From(string tableOrViewName, string whereClause, object argumentValue);


        /// <summary>
        /// This is used to directly query a table or view.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <exception cref="ArgumentException">tableOrViewName is empty.;tableOrViewName</exception>
        ITableDbCommandBuilder From(string tableOrViewName, object filterValue);

        /// <summary>
        /// Inserts an object into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options for how the isnert occurs.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        ISingleRowDbCommandBuilder Insert(string tableName, object argumentValue, InsertOptions options = InsertOptions.None);


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
        /// Gets a record by its primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        ISingleRowDbCommandBuilder GetByKey<T>(string tableName, T key);

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        IMultipleRowDbCommandBuilder GetByKey<T>(string tableName, IEnumerable<T> keys);

        /// <summary>
        /// Gets a set of records by their primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        /// <remarks>This only works on tables that have a scalar primary key.</remarks>
        IMultipleRowDbCommandBuilder GetByKey<T>(string tableName, params T[] keys);

    }
}
