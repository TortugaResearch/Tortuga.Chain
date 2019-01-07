using System.Globalization;

namespace Tortuga.Chain.Metadata
{
    internal static class Utilities
    {
        static readonly char[] s_InvalidCharacters = new char[] { ' ', '.', ',', '$', '@' };

        public static string ToClrName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            string result = name;
            foreach (char c in s_InvalidCharacters)
            {
                result = result.Replace(c.ToString(CultureInfo.InvariantCulture), "");
            }

            if (!char.IsLetter(result[0]) && result[0] != '_')
                result = "_" + result;

            return result;
        }
    }
}