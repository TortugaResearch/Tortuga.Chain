using System;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// An appender modifies the execution chain of an operation, usually by performing an action just before or after the database call. This version can convert an ILink from one type to another.
    /// </summary>
    /// <typeparam name="TIn">The type of the input type.</typeparam>
    /// <typeparam name="TOut">The type of the output type.</typeparam>
    /// <seealso cref="ILink{TOut}" />
    public abstract class Appender<TIn, TOut> : ILink<TOut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <exception cref="ArgumentNullException">previousLink;previousLink is null.</exception>
        protected Appender(ILink<TIn> previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            PreviousLink = previousLink;
            PreviousLink.ExecutionTokenPrepared += PreviousLink_ExecutionTokenPrepared;
            PreviousLink.ExecutionTokenPreparing += PreviousLink_ExecutionTokenPreparing;
        }

        private void PreviousLink_ExecutionTokenPrepared(object sender, ExecutionTokenPreparedEventArgs e)
        {
            OnExecutionTokenPrepared(e); //left first
            ExecutionTokenPrepared?.Invoke(this, e); //then right
            e.ExecutionToken.CommandBuilt += ExecutionToken_CommandBuilt;
        }

        private void PreviousLink_ExecutionTokenPreparing(object sender, ExecutionTokenPreparingEventArgs e)
        {
            OnExecutionTokenPreparing(e); //left first
            ExecutionTokenPreparing?.Invoke(this, e); //then right
        }

        private void ExecutionToken_CommandBuilt(object sender, CommandBuiltEventArgs e)
        {
            OnCommandBuilt(e);
        }

        /// <summary>
        /// Override this if you want to examine or modify the DBCommand before it is executed. 
        /// </summary>
        /// <param name="e">The <see cref="CommandBuiltEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCommandBuilt(CommandBuiltEventArgs e)
        {

        }

        /// <summary>
        /// Override this if you want to examine or modify the execution token before the DBCommand object is built.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionTokenPreparedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExecutionTokenPrepared(ExecutionTokenPreparedEventArgs e)
        {

        }

        /// <summary>
        /// Override this if you want to examine or modify the command builder before the execution token is built.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionTokenPreparingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExecutionTokenPreparing(ExecutionTokenPreparingEventArgs e)
        {

        }
        /// <summary>
        /// Gets the data source that is associated with this materilizer or appender.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource
        {
            get { return PreviousLink.DataSource; }
        }

        /// <summary>
        /// Gets the previous link in the operation chain.
        /// </summary>
        public ILink<TIn> PreviousLink { get; }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <remarks>If you don't override this method, it will call execute on the previous link.</remarks>
        public abstract TOut Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task<TOut> ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        /// <remarks>If you don't override this method, it will call execute on the previous link.</remarks>
        public abstract Task<TOut> ExecuteAsync(CancellationToken cancellationToken, object state = null);

        /// <summary>
        /// Occurs when an execution token has been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        public event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;

        /// <summary>
        /// Occurs when an execution token is about to be prepared.
        /// </summary>
        /// <remarks>
        /// This is mostly used by appenders to override SQL generation.
        /// </remarks>
        public event EventHandler<ExecutionTokenPreparingEventArgs> ExecutionTokenPreparing;

        /// <summary>
        /// Returns the generated SQL statement of the previous link.
        /// </summary>
        /// <returns></returns>
        public string CommandText()
        {
            return PreviousLink.CommandText();
        }
    }
}
