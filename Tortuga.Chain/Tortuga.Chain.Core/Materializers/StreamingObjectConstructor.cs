using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;
using static Tortuga.Chain.Materializers.MaterializerUtilities;

namespace Tortuga.Chain.Materializers
{
	internal sealed class StreamingObjectConstructor<T> : IDisposable
		where T : class
	{
		static readonly ImmutableArray<MappedProperty<T>> s_AllMappedProperties;
		static readonly ImmutableArray<MappedProperty<T>> s_DecomposedProperties;

		static readonly Type[] s_DefaultConstructor = Array.Empty<Type>();

		readonly ConstructorMetadata m_Constructor;

		readonly StreamingObjectConstructorDictionary m_Dictionary;

		readonly List<OrdinalMappedProperty<T>>? m_MappedProperties;
		readonly Dictionary<int, bool> m_NullableColumns;
		readonly Dictionary<string, int> m_Ordinals;
		DbDataReader? m_Source;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static StreamingObjectConstructor()
		{
			var methodType = typeof(StreamingObjectConstructor<T>).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m => m.Name == "CreateMappedProperty_Helper");

			var mappedProperties = new List<MappedProperty<T>>();
			var decomposedProperties = new List<MappedProperty<T>>();

			foreach (var property in MetadataCache.GetMetadata(typeof(T)).Properties)
			{
				if (property.MappedColumnName != null)
				{
					if (property.CanWrite)
					{
						var genericMethod = methodType.MakeGenericMethod(property.PropertyType);
						var mapper = (MappedProperty<T>)genericMethod.Invoke(null, new object?[] { property.MappedColumnName, property })!;

						mappedProperties.Add(mapper);
					}

					if (property.Decompose)
					{
						decomposedProperties.Add(new MappedProperty<T>(property.MappedColumnName, property));
					}
				}
			}

			s_AllMappedProperties = mappedProperties.ToImmutableArray();
			s_DecomposedProperties = decomposedProperties.ToImmutableArray();
		}

		public StreamingObjectConstructor(DbDataReader source, ConstructorMetadata? constructor, IReadOnlyList<ColumnMetadata> nonNullableColumns, bool? populateComplexObject)
		{
			m_Source = source;
			m_Ordinals = new Dictionary<string, int>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			m_NullableColumns = new Dictionary<int, bool>(source.FieldCount);
			for (var i = 0; i < source.FieldCount; i++)
			{
				var columnName = source.GetName(i);
				m_Ordinals.Add(columnName, i);
				m_NullableColumns.Add(i, !nonNullableColumns.Any(c => c.SqlName == columnName)); //assume nullable unless proven otherwise
			}

			if (constructor == null)
				constructor = MetadataCache.GetMetadata(typeof(T)).Constructors.Find(s_DefaultConstructor);
			if (constructor == null)
				throw new MappingException($"Cannot find a default constructor for {typeof(T).Name}");

			var desiredType = typeof(T);

			m_Constructor = constructor;

			var constructorParameters = m_Constructor.ParameterNames;
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				if (!m_Ordinals.ContainsKey(constructorParameters[i]))
					throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
			}

			m_Dictionary = new StreamingObjectConstructorDictionary(this);

			if (constructor.Signature.Length == 0 || populateComplexObject == true)
			{
				m_MappedProperties = new List<OrdinalMappedProperty<T>>();

				foreach (var mapper in s_AllMappedProperties)
				{
					if (m_Dictionary.ContainsKey(mapper.MappedColumnName))
						m_MappedProperties.Add(new OrdinalMappedProperty<T>(mapper, m_Ordinals[mapper.MappedColumnName]));
				}
			}
		}

		public StreamingObjectConstructor(DbDataReader source, IReadOnlyList<Type>? constructorSignature, IReadOnlyList<ColumnMetadata> nonNullableColumns, bool? populateComplexObject)
		{
			m_Source = source;
			m_Ordinals = new Dictionary<string, int>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			m_NullableColumns = new Dictionary<int, bool>(source.FieldCount);
			for (var i = 0; i < source.FieldCount; i++)
			{
				var columnName = source.GetName(i);
				m_Ordinals.Add(columnName, i);
				m_NullableColumns.Add(i, !nonNullableColumns.Any(c => c.SqlName == columnName)); //assume nullable unless proven otherwise
			}
			constructorSignature ??= s_DefaultConstructor;

			var desiredType = typeof(T);
			var constructor = MetadataCache.GetMetadata(desiredType).Constructors.Find(constructorSignature);

			if (constructor == null)
			{
				var types = string.Join(", ", constructorSignature.Select(t => t.Name));
				throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
			}
			m_Constructor = constructor;

			var constructorParameters = m_Constructor.ParameterNames;
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				if (!m_Ordinals.ContainsKey(constructorParameters[i]))
					throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
			}

			m_Dictionary = new StreamingObjectConstructorDictionary(this);

			if (constructorSignature.Count == 0 || populateComplexObject == true)
			{
				m_MappedProperties = new List<OrdinalMappedProperty<T>>();

				foreach (var mapper in s_AllMappedProperties)
				{
					if (m_Dictionary.ContainsKey(mapper.MappedColumnName))
						m_MappedProperties.Add(new OrdinalMappedProperty<T>(mapper, m_Ordinals[mapper.MappedColumnName]));
				}
			}
		}

		public T? Current { get; private set; }

		public IReadOnlyDictionary<string, object?> CurrentDictionary => m_Dictionary;

		public int RowsRead { get; private set; }

		private DbDataReader Source
		{
			get
			{
				if (m_Source == null)
					throw new ObjectDisposedException(nameof(StreamingObjectConstructor<T>));

				return m_Source;
			}
		}

		public void Dispose()
		{
			if (m_Source == null)
				return;

			m_Source.Dispose();
			m_Source = null;
		}

		public bool Read([NotNullWhen(true)] out T? value)
		{
			var result = Source.Read();
			if (result)
			{
				Current = ConstructObject();
				RowsRead += 1;
			}
			else
				Current = null;

			value = Current;
			return result;
		}

		public async Task<bool> ReadAsync()
		{
			var result = await Source.ReadAsync().ConfigureAwait(false);
			if (result)
			{
				Current = ConstructObject();
				RowsRead += 1;
			}
			else
				Current = null;
			return result;
		}

		public List<T> ToList()
		{
			var result = new List<T>();
			while (Read(out var value))
				result.Add(value);
			return result;
		}

		public async Task<List<T>> ToListAsync()
		{
			var result = new List<T>();
			while (await ReadAsync().ConfigureAwait(false))
				result.Add(Current!);
			return result;
		}

		internal static MappedProperty<T> CreateMappedProperty_Helper<T2>(string mappedColumnName, PropertyMetadata propertyMetadata)
		{
			return new MappedProperty<T, T2>(mappedColumnName, propertyMetadata);
		}

		internal IEnumerable<T> ToObjects()
		{
			while (Read(out var value))
				yield return value;
		}

		T ConstructObject()
		{
			var constructorParameters = m_Constructor.ParameterNames;
			var parameters = new object?[constructorParameters.Length];

			for (var i = 0; i < constructorParameters.Length; i++)
				parameters[i] = m_Dictionary[constructorParameters[i]];

			var result = (T)m_Constructor.ConstructorInfo.Invoke(parameters);

			if (m_MappedProperties != null)
				PopulateComplexObject(m_Dictionary, result, null, m_MappedProperties, s_DecomposedProperties);

			//Change tracking objects shouldn't be materialized as unchanged.
			(result as IChangeTracking)?.AcceptChanges();

			return result;
		}

		class StreamingObjectConstructorDictionary : IReadOnlyDictionary<string, object?>, IReadOnlyDictionary<int, object?>
		{
			readonly StreamingObjectConstructor<T> m_Parent;

			public StreamingObjectConstructorDictionary(StreamingObjectConstructor<T> parent)
			{
				m_Parent = parent;
			}

			public int Count => m_Parent.Source.FieldCount;

			IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => m_Parent.m_Ordinals.Keys;

			IEnumerable<int> IReadOnlyDictionary<int, object?>.Keys => m_Parent.m_Ordinals.Values;

			public IEnumerable<object> Values
			{
				get
				{
					var result = new object[m_Parent.Source.FieldCount];
					m_Parent.Source.GetValues(result);
					return result;
				}
			}

			public object? this[int key]
			{
				get
				{
					var result = m_Parent.Source.GetValue(key);
					return result == DBNull.Value ? null : result;
				}
			}

			public object? this[string key] => this[m_Parent.m_Ordinals[key]];

			public bool ContainsKey(string key) => m_Parent.m_Ordinals.ContainsKey(key);

			bool IReadOnlyDictionary<int, object?>.ContainsKey(int key) => key < m_Parent.m_Ordinals.Count;

			public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
			{
				for (var i = 0; i < m_Parent.Source.FieldCount; i++)
					yield return new KeyValuePair<string, object?>(m_Parent.Source.GetName(i), m_Parent.Source.IsDBNull(i) ? null : m_Parent.Source.GetValue(i));
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			IEnumerator<KeyValuePair<int, object?>> IEnumerable<KeyValuePair<int, object?>>.GetEnumerator()
			{
				for (var i = 0; i < m_Parent.Source.FieldCount; i++)
					yield return new KeyValuePair<int, object?>(i, m_Parent.Source.IsDBNull(i) ? null : m_Parent.Source.GetValue(i));
			}

			bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value)
			{
				if (m_Parent.m_Ordinals.ContainsKey(key))
				{
					value = this[key];
					return true;
				}
				value = null;
				return false;
			}

			bool IReadOnlyDictionary<int, object?>.TryGetValue(int key, out object? value)
			{
				if (key < m_Parent.m_Ordinals.Count)
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
