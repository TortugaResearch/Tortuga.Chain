using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
namespace Tortuga.Chain.CommandBuilders
{

    /// <summary>
    /// This is the base class from which all other command builders are created.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command used.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class DbCommandBuilder<TCommand, TParameter> : IDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        private readonly DataSource<TCommand, TParameter> m_DataSource;

        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected DbCommandBuilder(DataSource<TCommand, TParameter> dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource<TCommand, TParameter> DataSource
        {
            get { return m_DataSource; }
        }

        /// <summary>
        /// Indicates this operation has no result set.
        /// </summary>
        /// <returns></returns>
        public ILink AsNonQuery() { return new NonQueryMaterializer<TCommand, TParameter>(this); }

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
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public abstract ExecutionToken<TCommand, TParameter> Prepare(Materializer<TCommand, TParameter> materializer);
    }
}

