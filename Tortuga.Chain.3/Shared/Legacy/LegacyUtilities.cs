#if !ORDINAL_STRINGS

using System;

#endif

namespace Tortuga.Chain
{
#if !ORDINAL_STRINGS

    static class LegacyUtilities
    {
#if !ORDINAL_STRINGS

        public static int GetHashCode(this string source, StringComparison stringComparison)
        {
            if (stringComparison == StringComparison.OrdinalIgnoreCase)
                return source.ToUpperInvariant().GetHashCode();
            else
                return source.GetHashCode();
        }

        public static string Replace(this string source, string oldValue, string newValue, StringComparison _)
        {
            return source.Replace(oldValue, newValue);
        }

        public static bool Contains(this string source, string value, StringComparison _)
        {
            return source.Contains(value);
        }

#endif
    }

#endif
}
