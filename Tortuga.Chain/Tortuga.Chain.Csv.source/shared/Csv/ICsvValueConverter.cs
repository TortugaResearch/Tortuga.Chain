using System.Globalization;

namespace Tortuga.Chain.Csv
{
    /// <summary>
    /// Interface ICsvValueConverter is used for converting values to and from strings
    /// </summary>
    interface ICsvValueConverter
    {
        /// <summary>
        /// Converts from string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>System.Object.</returns>
        object ConvertFromString(string value, CultureInfo locale);

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        string ConvertToString(object value, CultureInfo locale);
    }
}
