using System.Data.Common;
using Tortuga.Chain.DataSources;

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
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="System.ArgumentException">connectionString is null or empty.;connectionString</exception>
        public GenericDbDataSource(string name, string connectionString, DataSourceSettings settings = null) : base(name, connectionString, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="System.ArgumentException">connectionString is null or empty.;connectionString</exception>
        public GenericDbDataSource(string connectionString, DataSourceSettings settings = null)
            : this(null, connectionString, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="name">Optional name of the data source.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="System.ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public GenericDbDataSource(string name, DbConnectionStringBuilder connectionStringBuilder, DataSourceSettings settings = null) : base(name, connectionStringBuilder, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <param name="settings">Optional settings object.</param>
        public GenericDbDataSource(DbConnectionStringBuilder connectionStringBuilder, DataSourceSettings settings = null)
            : this(null, connectionStringBuilder, settings)
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

    }
}
