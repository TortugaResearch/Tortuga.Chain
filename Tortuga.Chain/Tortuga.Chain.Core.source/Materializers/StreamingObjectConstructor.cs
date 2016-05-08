using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Materializers
{
    internal class StreamingObjectConstructor<T> : IDisposable
        where T : class
    {
        static readonly Type[] s_DefaultConstructor = new Type[0];

        readonly ConstructorMetadata m_Constructor;
        readonly StreamingObjectConstructorDictionary m_Dictionary;
        readonly Dictionary<string, int> m_Ordinals;
        readonly bool m_PopulateComplexObject;
        private T m_Current;
        private bool m_Disposed;
        private DbDataReader m_Source;
        private int m_RowsRead;

        public StreamingObjectConstructor(DbDataReader source, IReadOnlyList<Type> constructorSignature)
        {
            m_Source = source;
            m_Ordinals = new Dictionary<string, int>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < source.FieldCount; i++)
                m_Ordinals.Add(source.GetName(i), i);

            constructorSignature = constructorSignature ?? s_DefaultConstructor;

            var desiredType = typeof(T);
            m_Constructor = MetadataCache.GetMetadata(desiredType).Constructors.Find(constructorSignature);

            if (m_Constructor == null)
            {
                var types = string.Join(", ", constructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            var constructorParameters = m_Constructor.ParameterNames;
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                if (!m_Ordinals.ContainsKey(constructorParameters[i]))
                    throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
            }

            m_PopulateComplexObject = constructorSignature.Count == 0;
            m_Dictionary = new StreamingObjectConstructorDictionary(this);
        }

        public T Current
        {
            get { return m_Current; }
        }

        public IReadOnlyDictionary<string, object> CurrentDictionary
        {
            get { return m_Dictionary; }
        }

        public int RowsRead
        {
            get { return m_RowsRead; }
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Source.Dispose();
            m_Source = null;
            m_Disposed = true;
        }

        public bool Read()
        {
            var result = m_Source.Read();
            if (result)
            {
                m_Current = ConstructObject();
                m_RowsRead += 1;
            }
            else
                m_Current = null;
            return result;
        }

        internal IEnumerable<T> ToObjects()
        {
            while (Read())
                yield return Current;
        }

        public async Task<bool> ReadAsync()
        {
            var result = await m_Source.ReadAsync();
            if (result)
            {
                m_Current = ConstructObject();
                m_RowsRead += 1;
            }
            else
                m_Current = null;
            return result;
        }

        public List<T> ToList()
        {
            var result = new List<T>();
            while (Read())
                result.Add(Current);
            return result;
        }

        public async Task<List<T>> ToListAsync()
        {
            var result = new List<T>();
            while (await ReadAsync())
                result.Add(Current);
            return result;
        }

        private T ConstructObject()
        {
            var constructorParameters = m_Constructor.ParameterNames;
            var parameters = new object[constructorParameters.Length];

            for (var i = 0; i < constructorParameters.Length; i++)
                parameters[i] = m_Dictionary[constructorParameters[i]];

            var result = (T)m_Constructor.ConstructorInfo.Invoke(parameters);

            if (m_PopulateComplexObject)
                MaterializerUtilities.PopulateComplexObject(m_Dictionary, result, null);

            //Change tracking objects shouldn't be materialized as unchanged.
            (result as IChangeTracking)?.AcceptChanges();

            return result;
        }

        private class StreamingObjectConstructorDictionary : IReadOnlyDictionary<string, object>
        {
            readonly StreamingObjectConstructor<T> m_Parent;

            public StreamingObjectConstructorDictionary(StreamingObjectConstructor<T> parent)
            {
                m_Parent = parent;
            }

            int IReadOnlyCollection<KeyValuePair<string, object>>.Count
            {
                get { return m_Parent.m_Source.FieldCount; }
            }

            IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
            {
                get { return m_Parent.m_Ordinals.Keys; }
            }

            IEnumerable<object> IReadOnlyDictionary<string, object>.Values
            {
                get
                {
                    var result = new object[m_Parent.m_Source.FieldCount];
                    m_Parent.m_Source.GetValues(result);
                    return result;
                }
            }

            public object this[string key]
            {
                get
                {
                    var ordinal = m_Parent.m_Ordinals[key];
                    if (m_Parent.m_Source.IsDBNull(ordinal))
                        return null;
                    return m_Parent.m_Source.GetValue(ordinal);
                }
            }
            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (var i = 0; i < m_Parent.m_Source.FieldCount; i++)
                    yield return new KeyValuePair<string, object>(m_Parent.m_Source.GetName(i), m_Parent.m_Source.IsDBNull(i) ? null : m_Parent.m_Source.GetValue(i));
            }

            bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
            {
                return m_Parent.m_Ordinals.ContainsKey(key);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value)
            {
                if (m_Parent.m_Ordinals.ContainsKey(key))
                {
                    value = this[key];
                    return true;
                }
                value = null;
                return false;
            }
        }

    }
}
