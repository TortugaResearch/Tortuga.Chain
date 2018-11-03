using System;
using System.Globalization;

namespace Tortuga.Chain.Csv
{

    /// <summary>
    /// Default implementation of ICsvValueConverter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Tortuga.Chain.Csv.ICsvValueConverter" />
    public class CsvValueConverter<T> : ICsvValueConverter
    {
        readonly Func<string, CultureInfo, T> m_FromString;
        readonly Func<T, CultureInfo, string> m_ToString;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvValueConverter{T}"/> class.
        /// </summary>
        /// <param name="toString">To string.</param>
        /// <param name="fromString">From string.</param>
        /// <exception cref="System.ArgumentNullException">
        /// toString;toString is null.
        /// or
        /// fromString;fromString is null.
        /// </exception>
        public CsvValueConverter(Func<T, string> toString, Func<string, T> fromString)
        {
            if (toString == null)
                throw new ArgumentNullException("toString", "toString is null.");
            if (fromString == null)
                throw new ArgumentNullException("fromString", "fromString is null.");

            m_ToString = (v, l) => toString(v);
            m_FromString = (s, l) => fromString(s);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvValueConverter{T}"/> class.
        /// </summary>
        /// <param name="toString">To string.</param>
        /// <param name="fromString">From string.</param>
        /// <exception cref="System.ArgumentNullException">
        /// toString;toString is null.
        /// or
        /// fromString;fromString is null.
        /// </exception>
        public CsvValueConverter(Func<T, CultureInfo, string> toString, Func<string, CultureInfo, T> fromString)
        {
            if (toString == null)
                throw new ArgumentNullException("toString", "toString is null.");
            if (fromString == null)
                throw new ArgumentNullException("fromString", "fromString is null.");

            m_ToString = toString;
            m_FromString = fromString;
        }

        /// <summary>
        /// Converts from string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>T.</returns>
        public T ConvertFromString(string value, CultureInfo locale) => m_FromString(value, locale);

        /// <summary>
        /// Converts from string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>System.Object.</returns>
        object ICsvValueConverter.ConvertFromString(string value, CultureInfo locale) => m_FromString(value, locale);

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        public string ConvertToString(T value, CultureInfo locale) => m_ToString(value, locale);
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        string ICsvValueConverter.ConvertToString(object value, CultureInfo locale) => m_ToString((T)value, locale);
    }


}
