using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// This represents an appender that includes a caching capability.
    /// </summary>
    public interface ICacheLink<TResult> : ILink<TResult>
    {
        /// <summary>
        /// Instructs the appender to invalidate any cache keys that it created or updated.
        /// </summary>
        void Invalidate();
    }
}