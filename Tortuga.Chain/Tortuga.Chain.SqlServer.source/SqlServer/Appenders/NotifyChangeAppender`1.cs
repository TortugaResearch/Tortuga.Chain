using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Appenders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.SqlServer.Appenders
{
    internal class NotifyChangeAppender<TResult> : Appender<TResult>
    {
        readonly OnChangeEventHandler m_EventHandler;
        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="eventHandler">The event handler to fire when then associated SQL Dependency is fired..</param>
        public NotifyChangeAppender(ILink<TResult> previousLink, OnChangeEventHandler eventHandler)
            : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler", "eventHandler is null.");

            m_EventHandler = eventHandler;
        }


        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NotifyChangeAppender")]
        protected override void OnExecutionTokenPrepared(ExecutionTokenPreparedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");

            var token = e.ExecutionToken as SqlServerExecutionToken;
            if (token == null)
                throw new NotSupportedException($"This type of command builder does not support SQL Dependency, which is required for the {nameof(NotifyChangeAppender<TResult>)}.");
            token.AddChangeListener(m_EventHandler);
        }

    }
}