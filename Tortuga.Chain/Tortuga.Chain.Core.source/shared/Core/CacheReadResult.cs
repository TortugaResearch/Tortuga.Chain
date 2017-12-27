using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Core
{

    /// <summary>
    /// Lightweight alternative to a Tuple for reading from the cache asynchronously.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct CacheReadResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheReadResult{T}"/> struct.
        /// </summary>
        /// <param name="keyFound">if set to <c>true</c> [key found].</param>
        /// <param name="value">The value.</param>
        public CacheReadResult(bool keyFound, T value)
        {
            KeyFound = keyFound;
            Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the key was found.
        /// </summary>
        public bool KeyFound { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <remarks>Will be Default(T) is the key wasn't found.</remarks>
        public T Value { get; }
    }
}
