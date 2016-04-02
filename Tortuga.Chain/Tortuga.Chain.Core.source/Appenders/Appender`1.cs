using System;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// An appender modifies the execution chain of an operation, usually by performing an action just before or after the database call.
    /// </summary>
    /// <typeparam name="TResult">The operation's result type.</typeparam>
    public abstract class Appender<TResult> : ILink<TResult>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResult}"/> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        protected Appender(ILink<TResult> previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            PreviousLink = previousLink;
            PreviousLink.ExecutionTokenPrepared += PreviousLink_ExecutionTokenPrepared;
        }

        private void PreviousLink_ExecutionTokenPrepared(object sender, ExecutionTokenPreparedEventArgs e)
        {
            OnExecutionTokenPrepared(e); //left first
            ExecutionTokenPrepared?.Invoke(this, e); //then right
            e.ExecutionToken.CommandBuilt += ExecutionToken_CommandBuilt;
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
        public ILink<TResult> PreviousLink { get; }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <remarks>If you don't override this method, it will call execute on the previous link.</remarks>
        public virtual TResult Execute(object state = null)
        {
            return PreviousLink.Execute(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task<TResult> ExecuteAsync(object state = null)
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
        public virtual Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            return PreviousLink.ExecuteAsync(cancellationToken, state);
        }


        /// <summary>
        /// Occurs when an execution token has been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        public event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;

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
