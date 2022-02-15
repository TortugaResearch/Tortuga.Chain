using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;

namespace Tortuga.Chain.Access
{
	/// <summary>
	/// Represents an object in Access (e.g. table, view, procedure)
	/// </summary>
	public struct AccessObjectName : IEquatable<AccessObjectName>
	{
		/// <summary>
		/// An empty schema/name pair
		/// </summary>
		public static readonly AccessObjectName Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessObjectName" /> struct.
		/// </summary>
		/// <param name="name">The name.</param>
		public AccessObjectName(string name)
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

#pragma warning disable CA2225 // Operator overloads have named alternates

		/// <summary>
		/// Perform an implicit conversion from <see cref="string"/> to <see cref="AccessObjectName"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator AccessObjectName(string value) => new AccessObjectName(value);

#pragma warning restore CA2225 // Operator overloads have named alternates

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public static bool operator !=(AccessObjectName left, AccessObjectName right) => !(left == right);

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public static bool operator ==(AccessObjectName left, AccessObjectName right)
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
			var other = obj as AccessObjectName?;
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
		public bool Equals(AccessObjectName other) => this == other;

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		/// <remarks>This is a case-insensitive comparison.</remarks>
		public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// To the quoted string.
		/// </summary>
		/// <returns></returns>
		public string ToQuotedString() => $"[{Name}]";

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString() => Name;

		[return: NotNullIfNotNull("value")]
		private static string? Normalize(string? value)
		{
			if (value.IsNullOrWhiteSpace())
				return null;

			return value.Replace("[", "", StringComparison.OrdinalIgnoreCase).Replace("]", "", StringComparison.OrdinalIgnoreCase);
		}
	}
}
