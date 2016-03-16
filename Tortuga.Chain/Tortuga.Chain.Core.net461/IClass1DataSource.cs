using System;
using System.Runtime.Caching;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain
{
    /// <summary>
    /// A class 1 data source supports basic CRUD operations. This is the bare minimum needed to implement the repostiory pattern.
    /// </summary>
    public interface IClass1DataSource
    {
        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's MemoryCache.
        /// </summary>
        /// <remarks>This is used by the WithCaching materializer.</remarks>
        ObjectCache Cache { get; set; }

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
        /// Invalidates a cache key.
        /// </summary>
        /// <param name="regionName">Name of the region. WARNING: some cache providers, including .NET's MemoryCache, don't support regions.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <exception cref="ArgumentException">cacheKey is null or empty.;cacheKey</exception>
        void InvalidateCache(string cacheKey, string regionName);

        /// <summary>
        /// Try to read from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="regionName">Name of the region. WARNING: some cache providers, including .NET's MemoryCache, don't support regions.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="result">The cached result.</param>
        /// <returns><c>true</c> if the key was found in the cache, <c>false</c> otherwise.</returns>
        bool TryReadFromCache<T>(string cacheKey, string regionName, out T result);

        /// <summary>
        /// Updates an object in the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The update options.</param>
        /// <exception cref="ArgumentException">tableName is empty.;tableName</exception>
        ISingleRowDbCommandBuilder Update(string tableName, object argumentValue, UpdateOptions options = UpdateOptions.None);

        /// <summary>
        /// Writes to cache, replacing any previous value.
        /// </summary>
        /// <param name="item">The cache item.</param>
        /// <param name="policy">Optional cache invalidation policy.</param>
        /// <exception cref="ArgumentNullException">item;item is null.</exception>
        void WriteToCache(CacheItem item, CacheItemPolicy policy);
    }
}
