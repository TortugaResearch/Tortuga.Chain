using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Methods that will eventually be moved into Anchor
    /// </summary>
    static class AnchorMissing
    {
        /// <summary>
        /// Gets the keys as a readonly collection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        /// <remarks>This is just a cast. It accounts for an API bug in ConcurrentDictionary.</remarks>
        public static ReadOnlyCollection<TKey> GetKeys<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary)
        {
            return (ReadOnlyCollection<TKey>)dictionary.Keys;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        /// <remarks>This is just a cast. It accounts for an API bug in ConcurrentDictionary.</remarks>
        public static ReadOnlyCollection<TValue> GetValues<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary)
        {
            return (ReadOnlyCollection<TValue>)dictionary.Values;
        }
    }
}

