using System.Collections;
using System.Collections.Generic;

namespace Tortuga.Chain
{


    /// <summary>
    /// A lightweigh row expressed as a dictionary.
    /// </summary>
    /// <seealso cref="IReadOnlyDictionary{String, Object}" />
    public class Row : IReadOnlyDictionary<string, object>
    {
        private Dictionary<string, object> m_Contents;

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="contents">The contents.</param>
        internal Row(Dictionary<string, object> contents)
        {
            m_Contents = contents;
        }

        /// <summary>
        /// Gets the <see cref="object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return m_Contents[key]; }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return m_Contents.Count; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        public IEnumerable<string> Keys
        {
            get { return m_Contents.Keys; }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        public IEnumerable<object> Values
        {
            get { return m_Contents.Values; }
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// true if the read-only dictionary contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return m_Contents.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return m_Contents.GetEnumerator();
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out object value)
        {
            return m_Contents.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Contents).GetEnumerator();
        }
    }
}
