using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{

    /// <summary>
    /// This is the base class from which all other command builders are created.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class DbCommandBuilder<TCommandType, TParameterType> : IDbCommandBuilder
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        private readonly DataSource<TCommandType, TParameterType> m_DataSource;

        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected DbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource<TCommandType, TParameterType> DataSource
        {
            get { return m_DataSource; }
        }

        /// <summary>
        /// Indicates this operation has no result set.
        /// </summary>
        /// <returns></returns>
        public IMaterializer AsNonQuery() { return new NonQueryMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public void Execute(object state = null)
        {
            AsNonQuery().Execute(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync(object state = null)
        {
            return AsNonQuery().ExecuteAsync(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            return AsNonQuery().ExecuteAsync(cancellationToken, state);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public abstract ExecutionToken<TCommandType, TParameterType> Prepare(Materializer<TCommandType, TParameterType> materializer);
    }
}