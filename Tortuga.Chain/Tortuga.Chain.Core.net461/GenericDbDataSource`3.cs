using System;
using System.Data.Common;

namespace Tortuga.Chain
{

    /// <summary>
    /// The GenericDbDataSource is the most simplistic of all of the data sources. The command builder only supports raw SQL, but you still have access to all of the materializers.
    /// </summary>
    /// <typeparam name="TConnection">The type of the t connection.</typeparam>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
    public class GenericDbDataSource<TConnection, TCommand, TParameter> : GenericDbDataSource
        where TConnection : DbConnection, new()
        where TCommand : DbCommand, new()
        where TParameter : DbParameter, new()
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="name">Name of the data source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public GenericDbDataSource(string name, string connectionString) : base(name, connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public GenericDbDataSource(string connectionString)
            : this(null, connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="name">Optional name of the data source.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public GenericDbDataSource(string name, DbConnectionStringBuilder connectionStringBuilder) : base(name, connectionStringBuilder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        public GenericDbDataSource(DbConnectionStringBuilder connectionStringBuilder)
            : this(null, connectionStringBuilder)
        {
        }

        internal override DbCommand CreateCommand()
        {
            return new TCommand();
        }

        internal override DbParameter CreateParameter()
        {
            return new TParameter();
        }

        internal override DbConnection OnCreateConnection()
        {
            return new TConnection();
        }

        ///// <summary>
        ///// Creates and opens a SQL connection.
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>The caller of this method is responsible for closing the connection.</remarks>
        //public new TConnection CreateConnection()
        //{
        //    return (TConnection)base.CreateConnection();
        //}

        ///// <summary>
        ///// Creates and opens a SQL connection.
        ///// </summary>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns></returns>
        ///// <remarks>
        ///// The caller of this method is responsible for closing the connection.
        ///// </remarks>
        //public new async Task<TConnection> CreateConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return (TConnection)(await base.CreateConnectionAsync(cancellationToken));
        //}

    }
}
