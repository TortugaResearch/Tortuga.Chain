using Tortuga.Chain.Core;
using System;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Class StrictModeAppender.
    /// </summary>
    internal sealed class StrictModeAppender : Appender
    {
        readonly bool m_StrictMode;


        /// <summary>
        /// Initializes a new instance of the <see cref="Appender{TResult}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
        public StrictModeAppender(ILink previousLink, bool strictMode)
            : base(previousLink)
        {
            m_StrictMode = strictMode;
        }

        protected override void OnExecutionTokenPreparing(ExecutionTokenPreparingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");
            e.CommandBuilder.StrictMode = m_StrictMode;
        }

    }
}
