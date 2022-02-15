using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Tortuga.Anchor;

namespace Tortuga.Chain.SQLite
{
	/// <summary>
	/// Represents an object in SQLite (e.g. table, view, procedure)
	/// </summary>
	public struct SQLiteObjectName : IEquatable<SQLiteObjectName>
	{
		/// <summary>
		/// An empty schema/name pair
		/// </summary>
		public static readonly SQLiteObjectName Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="SQLiteObjectName" /> struct.
		/// </summary>
		/// <param name="name">The name.</param>
		public SQLiteObjectName(string name)
		{
			Name = Normalize(name);
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; }

		/// <summary>
		/// Perform an implicit conversion from <see cref="string"/> to <see cref="SQLiteObjectName"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
#pragma warning disable CA2225 // Operator overloads have named alternates

		public static implicit operator SQLiteObjectName(string value)
#pragma warning restore CA2225 // Operator overloads have named alternates
		{
			return new SQLiteObjectName(value);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public static bool operator !=(SQLiteObjectName left, SQLiteObjectName right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public static bool operator ==(SQLiteObjectName left, SQLiteObjectName right)
		{
			return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public override bool Equals(object? obj)
		{
			var other = obj as SQLiteObjectName?;
			if (other == null)
				return false;
			return this == other;
		}

		/// <summary>
		/// Returns true if the two objects are equal.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public bool Equals(SQLiteObjectName other)
		{
			return this == other;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public override int GetHashCode()
		{
			return Name.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
		}

		/// <summary>
		/// To the quoted string.
		/// </summary>
		/// <returns></returns>
		public string ToQuotedString()
		{
			return $"\"{Name}\"";
		}

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		[return: NotNullIfNotNull("value")]
		private static string? Normalize(string? value)
		{
			if (value.IsNullOrWhiteSpace())
				return null;

			//SQLite supports many escaping options
			if (value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
				return value.Substring(1, value.Length - 2);

			if (value.StartsWith("[", StringComparison.OrdinalIgnoreCase) && value.EndsWith("]", StringComparison.OrdinalIgnoreCase))
				return value.Substring(1, value.Length - 2);

			if (value.StartsWith("`", StringComparison.OrdinalIgnoreCase) && value.EndsWith("`", StringComparison.OrdinalIgnoreCase))
				return value.Substring(1, value.Length - 2);

			return value;
		}
	}
}
