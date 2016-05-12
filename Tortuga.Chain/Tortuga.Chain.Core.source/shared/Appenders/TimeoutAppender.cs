using System;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Class TimeoutAppender.
    /// </summary>
    internal sealed class TimeoutAppender : Appender
    {
        readonly TimeSpan m_Timeout;


        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="timeout">The command timeout.</param>
        public TimeoutAppender(ILink previousLink, TimeSpan timeout)
            : base(previousLink)
        {
            m_Timeout = timeout;
        }

        /// <summary>
        /// Override this if you want to examine or modify the DBCommand before it is executed.
        /// </summary>
        /// <param name="e">The <see cref="CommandBuiltEventArgs" /> instance containing the event data.</param>
        protected override void OnCommandBuilt(CommandBuiltEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");
            e.Command.CommandTimeout = (int)m_Timeout.TotalSeconds;
        }
    }
}
