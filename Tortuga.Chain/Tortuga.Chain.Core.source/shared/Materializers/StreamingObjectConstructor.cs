using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using static Tortuga.Chain.Materializers.MaterializerUtilities;

namespace Tortuga.Chain.Materializers
{
    internal class StreamingObjectConstructor<T> : IDisposable
        where T : class
    {

        static readonly ImmutableArray<MappedProperty<T>> s_AllMappedProperties;
        static readonly ImmutableArray<MappedProperty<T>> s_DecomposedProperties;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static StreamingObjectConstructor()
        {
            var methodType = typeof(StreamingObjectConstructor<T>).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(m => m.Name == "CreateMappedProperty_Helper").Single();

            var mappedProperties = new List<MappedProperty<T>>();
            var decomposedProperties = new List<MappedProperty<T>>();

            foreach (var property in MetadataCache.GetMetadata(typeof(T)).Properties)
            {
                if (property.CanWrite)
                {
                    var genericMethod = methodType.MakeGenericMethod(property.PropertyType);
                    var mapper = (MappedProperty<T>)genericMethod.Invoke(null, new object[] { property.MappedColumnName, property });

                    mappedProperties.Add(mapper);
                }

                if (property.Decompose)
                {
                    decomposedProperties.Add(new MappedProperty<T>(property.MappedColumnName, property));
                }
            }

            s_AllMappedProperties = mappedProperties.ToImmutableArray();
            s_DecomposedProperties = decomposedProperties.ToImmutableArray();
        }



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


            if (m_PopulateComplexObject)
            {
                m_MappedProperties = new List<OrdinalMappedProperty<T>>();

                foreach (var mapper in s_AllMappedProperties)
                {
                    if (m_Dictionary.ContainsKey(mapper.MappedColumnName))
                        m_MappedProperties.Add(new OrdinalMappedProperty<T>(mapper, m_Ordinals[mapper.MappedColumnName]));
                }
            }
        }

        internal static MappedProperty<T> CreateMappedProperty_Helper<T2>(string mappedColumnName, PropertyMetadata propertyMetadata)
        {
            return new MappedProperty<T, T2>(mappedColumnName, propertyMetadata);
        }


        List<OrdinalMappedProperty<T>> m_MappedProperties;

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
                MaterializerUtilities.PopulateComplexObject<T>(m_Dictionary, result, null, m_MappedProperties, s_DecomposedProperties);

            //Change tracking objects shouldn't be materialized as unchanged.
            (result as IChangeTracking)?.AcceptChanges();

            return result;
        }

        private class StreamingObjectConstructorDictionary : IReadOnlyDictionary<string, object>, IReadOnlyDictionary<int, object>
        {
            readonly StreamingObjectConstructor<T> m_Parent;

            public StreamingObjectConstructorDictionary(StreamingObjectConstructor<T> parent)
            {
                m_Parent = parent;
            }

            public int Count => m_Parent.m_Source.FieldCount;

            IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => m_Parent.m_Ordinals.Keys;

            public IEnumerable<object> Values
            {
                get
                {
                    var result = new object[m_Parent.m_Source.FieldCount];
                    m_Parent.m_Source.GetValues(result);
                    return result;
                }
            }

            IEnumerable<int> IReadOnlyDictionary<int, object>.Keys => m_Parent.m_Ordinals.Values;



            public object this[int key]
            {
                get
                {
                    if (m_Parent.m_Source.IsDBNull(key))
                        return null;
                    return m_Parent.m_Source.GetValue(key);
                }
            }

            public object this[string key] => this[m_Parent.m_Ordinals[key]];

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (var i = 0; i < m_Parent.m_Source.FieldCount; i++)
                    yield return new KeyValuePair<string, object>(m_Parent.m_Source.GetName(i), m_Parent.m_Source.IsDBNull(i) ? null : m_Parent.m_Source.GetValue(i));
            }

            public bool ContainsKey(string key)
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

            bool IReadOnlyDictionary<int, object>.ContainsKey(int key)
            {
                return key < m_Parent.m_Ordinals.Count;
            }

            bool IReadOnlyDictionary<int, object>.TryGetValue(int key, out object value)
            {
                if (key < m_Parent.m_Ordinals.Count)
                {
                    value = this[key];
                    return true;
                }
                value = null;
                return false;
            }

            IEnumerator<KeyValuePair<int, object>> IEnumerable<KeyValuePair<int, object>>.GetEnumerator()
            {
                for (var i = 0; i < m_Parent.m_Source.FieldCount; i++)
                    yield return new KeyValuePair<int, object>(i, m_Parent.m_Source.IsDBNull(i) ? null : m_Parent.m_Source.GetValue(i));
            }
        }

    }
}
