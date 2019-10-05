using System;
using System.Globalization;

namespace Tortuga.Chain.Metadata
{
    internal static class Utilities
    {
        static readonly char[] s_InvalidCharacters = new char[] { ' ', '.', ',', '$', '@' };

        public static string ToClrName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

            string result = name;
            foreach (char c in s_InvalidCharacters)
            {
                result = result.Replace(c.ToString(CultureInfo.InvariantCulture), "", StringComparison.Ordinal);
            }

            if (!char.IsLetter(result[0]) && result[0] != '_')
                result = "_" + result;

            return result;
        }

        public static string QuotedSqlNameSafe(this ISqlBuilderEntryDetails details)
        {
            var value = details.QuotedSqlName;
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"Using parameter {details.SqlVariableName} as a column is not allowed because QuotedSqlName is missing.");

            return value!;
        }

#if !OrdinalHashCodes

        public static int GetHashCode(this string source, StringComparison _)
        {
            return source.GetHashCode();
        }

        public static string Replace(this string source, string oldValue, string newValue, StringComparison _)
        {
            return source.Replace(oldValue, newValue);
        }

#endif
    }
}
