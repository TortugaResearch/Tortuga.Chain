﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    /// <summary>
    /// This is implemented by materializers and appenders that return a value.
    /// </summary>

    public interface ILink<TResult>
    {
        /// <summary>
        /// Gets the data source that is associated with this materilizer or appender.
        /// </summary>
        /// <value>The data source.</value>
        /// <remarks>This is only used for </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        DataSource DataSource { get; }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        TResult Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync(CancellationToken cancellationToken, object state = null);

        /// <summary>
        /// Occurs when an execution token has been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;

        /// <summary>
        /// Returns command text (usually SQL) without executing it.
        /// </summary>
        /// <returns></returns>
        string CommandText();
    }
}