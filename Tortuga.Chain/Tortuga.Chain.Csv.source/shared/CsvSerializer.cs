using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Csv
{

    /// <summary>
    /// A lightweight CSV Serializer/Deserializer based on TextFieldParser.
    /// </summary>
    public class CsvSerializer
    {
        static readonly ConcurrentDictionary<Type, ICsvValueConverter> s_GlobalConverters = new ConcurrentDictionary<Type, ICsvValueConverter>();
        readonly Dictionary<Type, ICsvValueConverter> m_Converters = new Dictionary<Type, ICsvValueConverter>();

        /// <summary>
        /// Initializes the default converters
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static CsvSerializer()
        {
#pragma warning disable IDE0001 // Simplify Names

            AddGlobalConverter<Missing>(v => "", s => null); //used for nulls when the type isn't otherwise known

            AddGlobalConverter<bool>(v => v ? "true" : "false", s => bool.Parse(s));
            AddGlobalConverter<bool?>(v => v.HasValue ? (v.Value ? "true" : "false") : "", s => string.IsNullOrEmpty(s) ? (bool?)null : bool.Parse(s));

            AddGlobalConverter<byte>(v => v.ToString(), s => byte.Parse(s));
            AddGlobalConverter<byte?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (byte?)null : byte.Parse(s));

            AddGlobalConverter<short>(v => v.ToString(), s => short.Parse(s));
            AddGlobalConverter<short?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (short?)null : short.Parse(s));

            AddGlobalConverter<int>(v => v.ToString(), s => int.Parse(s));
            AddGlobalConverter<int?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (int?)null : int.Parse(s));

            AddGlobalConverter<long>(v => v.ToString(), s => long.Parse(s));
            AddGlobalConverter<long?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (long?)null : long.Parse(s));

            AddGlobalConverter<float>(v => v.ToString(), s => float.Parse(s));
            AddGlobalConverter<float?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (float?)null : float.Parse(s));

            AddGlobalConverter<double>(v => v.ToString(), s => double.Parse(s));
            AddGlobalConverter<double?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (double?)null : double.Parse(s));

            AddGlobalConverter<decimal>(v => v.ToString(), s => decimal.Parse(s));
            AddGlobalConverter<decimal?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (decimal?)null : decimal.Parse(s));

            AddGlobalConverter<ushort>(v => v.ToString(), s => ushort.Parse(s));
            AddGlobalConverter<ushort?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (ushort?)null : ushort.Parse(s));

            AddGlobalConverter<uint>(v => v.ToString(), s => uint.Parse(s));
            AddGlobalConverter<uint?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (uint?)null : uint.Parse(s));

            AddGlobalConverter<ulong>(v => v.ToString(), s => ulong.Parse(s));
            AddGlobalConverter<ulong?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (ulong?)null : ulong.Parse(s));

            AddGlobalConverter<sbyte>(v => v.ToString(), s => sbyte.Parse(s));
            AddGlobalConverter<sbyte?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (sbyte?)null : sbyte.Parse(s));

            AddGlobalConverter<DateTime>(v => v.ToString("o"), s => DateTime.Parse(s));
            AddGlobalConverter<DateTime?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (DateTime?)null : DateTime.Parse(s));

            AddGlobalConverter<TimeSpan>(v => v.ToString(), s => TimeSpan.Parse(s));
            AddGlobalConverter<TimeSpan?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (TimeSpan?)null : TimeSpan.Parse(s));

            AddGlobalConverter<Guid>(v => v.ToString(), s => Guid.Parse(s));
            AddGlobalConverter<Guid?>(v => v.HasValue ? v.ToString() : "", s => string.IsNullOrEmpty(s) ? (Guid?)null : Guid.Parse(s));


            AddGlobalConverter<string>(v => v == null ? "" : "\"" + v.Replace("\"", "\"\"") + "\"", s => s);

            AddGlobalConverter<object>(v => v == null ? "" : "\"" + v.ToString().Replace("\"", "\"\"") + "\"", s => { throw new NotSupportedException(); });
#pragma warning restore IDE0001 // Simplify Names
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CsvSerializer"/> class.
        /// </summary>
        public CsvSerializer()
        {
            Locale = CultureInfo.InvariantCulture;
            m_Converters = new Dictionary<Type, ICsvValueConverter>(s_GlobalConverters);
        }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        /// <remarks>This defaults to CultureInfo.InvariantCulture</remarks>
        public CultureInfo Locale { get; set; }

        /// <summary>
        /// Adds the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toString">To string.</param>
        /// <param name="fromString">From string.</param>
        public void AddConverter<T>(Func<T, string> toString, Func<string, T> fromString)
        {
            m_Converters[typeof(T)] = new CsvValueConverter<T>(toString, fromString);
        }

        /// <summary>
        /// Adds the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toString">To string.</param>
        /// <param name="fromString">From string.</param>
        public void AddConverter<T>(Func<T, CultureInfo, string> toString, Func<string, CultureInfo, T> fromString)
        {
            m_Converters[typeof(T)] = new CsvValueConverter<T>(toString, fromString);
        }

        /// <summary>
        /// Deserializes a CSV file into a DataReader.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>IDataReader.</returns>
        public IDataReader DeserializeToDataReader(TextReader inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            return new CsvDataReader(inputStream, columnTypeMap, m_Converters, Locale);
        }

        /// <summary>
        /// Deserializes a CSV file into a DataReader.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>IDataReader.</returns>
        public IDataReader DeserializeToDataReader(Stream inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            return new CsvDataReader(inputStream, columnTypeMap, m_Converters, Locale);
        }

        /// <summary>
        /// Deserializes a CSV file into a data table.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>DataTable.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DataTable DeserializeToDataTable(Stream inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
            {
                var result = new DataTable() { Locale = CultureInfo.InvariantCulture };
                result.Load(reader);
                return result;
            }
        }

        /// <summary>
        /// Deserializes a CSV file into a data table.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>DataTable.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DataTable DeserializeToDataTable(TextReader inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
            {
                var result = new DataTable() { Locale = CultureInfo.InvariantCulture };
                result.Load(reader);
                return result;
            }
        }

        /// <summary>
        /// Deserializes a CSV file into objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> DeserializeToObjects<T>(TextReader inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
            where T : new()
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
                return new Table(reader).ToObjects<T>();
        }

        /// <summary>
        /// Deserializes a CSV file into objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> DeserializeToObjects<T>(Stream inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
            where T : new()
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
                return new Table(reader).ToObjects<T>();
        }

        /// <summary>
        /// Deserializes a CSV file into a table.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>Table.</returns>
        public Table DeserializeToTable(TextReader inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
                return new Table(reader);
        }

        /// <summary>
        /// Deserializes a CSV file into a table.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="columnTypeMap">The column type map.</param>
        /// <returns>Table.</returns>
        public Table DeserializeToTable(Stream inputStream, IReadOnlyDictionary<string, Type> columnTypeMap)
        {
            using (var reader = DeserializeToDataReader(inputStream, columnTypeMap))
                return new Table(reader);
        }

        /// <summary>
        /// Serialize into a CSV file asynchronously.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <returns>Task.</returns>
        public async Task SerializeAsync(Table source, StreamWriter outputStream, bool includeHeaders)
        {
            var converters = new List<Tuple<string, ICsvValueConverter>>(source.ColumnNames.Count);

            foreach (var columnName in source.ColumnNames)
            {
                var type = source.ColumnTypeMap[columnName];
                var converter = m_Converters[typeof(object)];
                m_Converters.TryGetValue(type, out converter);
                converters.Add(Tuple.Create(columnName, converter));
            }

            if (includeHeaders)
            {
                var converter = (CsvValueConverter<string>)m_Converters[typeof(string)];
                await outputStream.WriteLineAsync(string.Join(",", source.ColumnNames.Select(c => converter.ConvertToString(c, Locale))));
            }

            foreach (var row in source.Rows)
                await outputStream.WriteLineAsync(string.Join(",", converters.Select(c => c.Item2.ConvertToString(row[c.Item1], Locale))));
        }

        /// <summary>
        /// Serialize into a CSV file asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <returns>Task.</returns>
        public async Task SerializeAsync<T>(IEnumerable<T> items, StreamWriter outputStream, bool includeHeaders)
        {
            var properties = MetadataCache.GetMetadata(typeof(T)).Properties.Where(p => p.CanRead).ToList();
            var converters = new List<Tuple<PropertyMetadata, ICsvValueConverter>>(properties.Count);

            foreach (var property in properties)
            {
                var type = property.PropertyType;
                var converter = m_Converters[typeof(object)];
                m_Converters.TryGetValue(type, out converter);
                converters.Add(Tuple.Create(property, converter));
            }

            if (includeHeaders)
            {
                var headerConverter = (CsvValueConverter<string>)m_Converters[typeof(string)];
                await outputStream.WriteLineAsync(string.Join(",", properties.Select(p => headerConverter.ConvertToString(p.MappedColumnName, Locale))));
            }

            foreach (var row in items)
                await outputStream.WriteLineAsync(string.Join(",", converters.Select(c => c.Item2.ConvertToString(c.Item1.InvokeGet(row), Locale))));
        }

        /// <summary>
        /// Adds the indicated conversion function to the list of global converters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toString">To string function.</param>
        /// <param name="fromString">From string function.</param>
        /// <remarks>This only applies to converters created after this function is called.</remarks>
        public static void AddGlobalConverter<T>(Func<T, string> toString, Func<string, T> fromString)
        {
            s_GlobalConverters[typeof(T)] = new CsvValueConverter<T>(toString, fromString);
        }

        /// <summary>
        /// Adds the indicated conversion function to the list of global converters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toString">To string function.</param>
        /// <param name="fromString">From string function.</param>
        /// <remarks>This only applies to converters created after this function is called.</remarks>
        public static void AddGlobalConverter<T>(Func<T, CultureInfo, string> toString, Func<string, CultureInfo, T> fromString)
        {
            s_GlobalConverters[typeof(T)] = new CsvValueConverter<T>(toString, fromString);
        }
    }




}
