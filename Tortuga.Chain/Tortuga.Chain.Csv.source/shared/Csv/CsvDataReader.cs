using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Tortuga.Chain.Csv
{

    sealed class CsvDataReader : IDataReader
    {
        readonly Dictionary<string, int> m_ColumnIndexMap = new Dictionary<string, int>();
        readonly Dictionary<string, Type> m_ColumnTypeMap = new Dictionary<string, Type>();
        readonly IReadOnlyDictionary<Type, ICsvValueConverter> m_Converters;
        readonly List<ICsvValueConverter> m_IndexConverterMap = new List<ICsvValueConverter>();
        readonly TextFieldParser m_Parser;
        readonly CultureInfo m_Locale;
        string[] m_ColumnNames;
        string[] m_CurrentRow;
        bool m_IsDisposed;

        internal CsvDataReader(TextReader inputStream, IReadOnlyDictionary<string, Type> columnTypeMap, IReadOnlyDictionary<Type, ICsvValueConverter> converters, CultureInfo locale)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream", "inputStream is null.");
            if (converters == null)
                throw new ArgumentNullException("converters", "converters is null.");

            m_Locale = locale;
            m_Parser = new TextFieldParser(inputStream);

            if (columnTypeMap != null)
                foreach (var item in columnTypeMap)
                    m_ColumnTypeMap.Add(item.Key, item.Value);

            m_Converters = converters;

            ParseHeader();
        }

        internal CsvDataReader(Stream inputStream, IReadOnlyDictionary<string, Type> columnTypeMap, IReadOnlyDictionary<Type, ICsvValueConverter> converters, CultureInfo locale)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream", "inputStream is null.");
            if (columnTypeMap == null)
                columnTypeMap = new Dictionary<string, Type>();
            if (converters == null)
                throw new ArgumentNullException("converters", "converters is null.");

            m_Locale = locale;
            m_Parser = new TextFieldParser(inputStream);

            if (columnTypeMap != null)
                foreach (var item in columnTypeMap)
                    m_ColumnTypeMap.Add(item.Key, item.Value);

            m_Converters = converters;

            ParseHeader();
        }

        public int FieldCount
        {
            get { return m_ColumnNames.Length; }
        }

        int IDataReader.Depth
        {
            get { return 0; }
        }

        bool IDataReader.IsClosed
        {
            get { return m_IsDisposed; }
        }

        int IDataReader.RecordsAffected
        {
            get { return -1; }
        }

        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }

        public void Dispose()
        {
            if (m_IsDisposed)
                return;

            m_Parser.Dispose();
            m_IsDisposed = true;
        }

        public bool GetBoolean(int i)
        {
            return (bool)m_Converters[typeof(bool)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public byte GetByte(int i)
        {
            return (byte)m_Converters[typeof(byte)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public char GetChar(int i)
        {
            return (char)m_Converters[typeof(char)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public string GetDataTypeName(int i)
        {
            return m_ColumnTypeMap[GetName(i)].FullName;
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)m_Converters[typeof(DateTime)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)m_Converters[typeof(decimal)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public double GetDouble(int i)
        {
            return (double)m_Converters[typeof(double)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public Type GetFieldType(int i)
        {
            return m_ColumnTypeMap[GetName(i)];
        }

        public float GetFloat(int i)
        {
            return (float)m_Converters[typeof(float)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public Guid GetGuid(int i)
        {
            return (Guid)m_Converters[typeof(Guid)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public short GetInt16(int i)
        {
            return (short)m_Converters[typeof(short)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public int GetInt32(int i)
        {
            return (int)m_Converters[typeof(int)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public long GetInt64(int i)
        {
            return (long)m_Converters[typeof(long)].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public string GetName(int i)
        {
            return m_ColumnNames[i];
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Required by the interface.")]
        public int GetOrdinal(string name)
        {
            int index;
            if (m_ColumnIndexMap.TryGetValue(name, out index))
                return index;

            //the interface calls for a case-insensitive search as a fall back
            foreach (var item in m_ColumnIndexMap)
                if (string.Compare(item.Key, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return item.Value;

            throw new IndexOutOfRangeException("Cannot find key " + name);
        }

        public object GetValue(int i)
        {
            return m_IndexConverterMap[i].ConvertFromString(m_CurrentRow[i], m_Locale);
        }

        public int GetValues(object[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("values is null or empty.", "values");

            var length = Math.Min(m_ColumnNames.Length, values.Length);

            for (var i = 0; i < length; i++)
                values[i] = GetValue(i);

            return length;
        }

        void IDataReader.Close()
        {
            Dispose();
        }

        DataTable IDataReader.GetSchemaTable()
        {
            return null; //We'll need to support this if we want to use Table-Valued Parameters in SQL Serever
        }

        bool IDataReader.NextResult()
        {
            return false;
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotSupportedException();
        }

        string IDataRecord.GetString(int i)
        {
            return m_CurrentRow[i];
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == null;
        }

        public bool Read()
        {
            if (m_Parser.EndOfData)
                return false;

            m_CurrentRow = m_Parser.ReadFields();
            return true;
        }

        void ParseHeader()
        {
            m_Parser.TextFieldType = FieldType.Delimited;
            m_Parser.SetDelimiters(",");
            m_Parser.HasFieldsEnclosedInQuotes = true;

            var index = 0;
            m_ColumnNames = m_Parser.ReadFields();
            foreach (var column in m_ColumnNames)
            {
                if (!m_ColumnTypeMap.ContainsKey(column))
                    m_ColumnTypeMap[column] = typeof(string); //default unmapped columns to string

                m_ColumnIndexMap[column] = index;
                index++;

                ICsvValueConverter converter;

                if (!m_Converters.TryGetValue(m_ColumnTypeMap[column], out converter))
                    converter = m_Converters[typeof(string)];

                m_IndexConverterMap.Add(converter);
            }
        }
    }
}
