#if !ORDINAL_STRINGS

using System;
using System.Globalization;

namespace Tortuga.Chain
{
	static class LegacyUtilities
	{
		public static int GetHashCode(this string source, StringComparison stringComparison)
		{
			switch (stringComparison)
			{
				case StringComparison.CurrentCultureIgnoreCase:
				case StringComparison.OrdinalIgnoreCase:
				case StringComparison.InvariantCultureIgnoreCase:
					return source.ToUpperInvariant().GetHashCode();

				default:
					return source.GetHashCode();
			}
		}

		public static string Replace(this string source, string oldValue, string newValue, StringComparison _)
		{
			return source.Replace(oldValue, newValue);
		}

		public static bool Contains(this string source, string value, StringComparison stringComparison)
		{
			return source.IndexOf(value, stringComparison) >= 0;
		}
	}
}

#endif
