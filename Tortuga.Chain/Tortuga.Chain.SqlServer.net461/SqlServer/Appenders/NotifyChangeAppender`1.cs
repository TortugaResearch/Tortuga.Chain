using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Appenders;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer.Core;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.SqlServer.Appenders
{
    internal class NotifyChangeAppender<TResultType> : Appender<TResultType>
    {
        readonly OnChangeEventHandler m_EventHandler;
        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResultType}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="eventHandler">The event handler to fire when then associated SQL Dependency is fired..</param>
        public NotifyChangeAppender(ILink<TResultType> previousLink, OnChangeEventHandler eventHandler)
            : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler", "eventHandler is null.");

            ExecutionTokenPrepared += NotifyChangeAppender_ExecutionTokenPrepared;

            m_EventHandler = eventHandler;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public override TResultType Execute(object state = null)
        {
            return PreviousLink.Execute(state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override Task<TResultType> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            return PreviousLink.ExecuteAsync(cancellationToken, state);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NotifyChangeAppender")]
        private void NotifyChangeAppender_ExecutionTokenPrepared(object sender, ExecutionTokenPreparedEventArgs e)
        {
            var token = e.ExecutionToken as SqlServerExecutionToken;
            if (token == null)
                throw new NotSupportedException($"This type of command builder does not support SQL Dependency, which is required for the {nameof(NotifyChangeAppender<TResultType>)}.");
            token.AddChangeListener(m_EventHandler);
        }
    }
}