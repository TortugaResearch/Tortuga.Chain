using System.Data.Common;
using System.Threading.Tasks;

namespace Tortuga.Chain.DataSources
{

    /// <summary>
    /// This is a data source from which other data sources can be created.
    /// </summary>
    /// <remarks>
    /// This interface is primarily for testing purposes.
    /// </remarks>
    public interface IRootDataSource
    {
        /// <summary>
        /// Creates and opens a connection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
        DbConnection CreateConnection();

        /// <summary>
        /// Creates and opens a connection asynchronously.
        /// </summary>
        /// <returns></returns>
        /// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
        Task<DbConnection> CreateConnectionAsync();


        /// <summary>
        /// Creates an open data source.
        /// </summary>
        /// <param name="connection">The connection to wrap.</param>
        /// <param name="transaction">The transaction to wrap.</param>
        /// <returns></returns>
        IOpenDataSource CreateOpenDataSource(DbConnection connection, DbTransaction transaction);

        /// <summary>
        /// Begin a transaction using the default settings
        /// </summary>
        ITransactionalDataSource BeginTransaction();

        /// <summary>
        /// Begin a transaction using the default settings
        /// </summary>
        Task<ITransactionalDataSource> BeginTransactionAsync();
    }
}
