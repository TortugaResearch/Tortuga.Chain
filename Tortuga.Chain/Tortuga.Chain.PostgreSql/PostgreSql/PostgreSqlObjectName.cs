namespace Tortuga.Chain.PostgreSql
{
	/// <summary>
	/// Represents an object in PostgreSql (e.g. table, view, procedure)
	/// </summary>
	public struct PostgreSqlObjectName : IEquatable<PostgreSqlObjectName>
	{
		/// <summary>
		/// An empty schema/name pair
		/// </summary>
		public static readonly PostgreSqlObjectName Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <param name="name">The name.</param>
		public PostgreSqlObjectName(string? schema, string name)
		{
			Database = null;
			Schema = schema;
			Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="name">The name.</param>
		[Obsolete("While technically allowed, PostgreSQL does not support cross-database calls.")]
		public PostgreSqlObjectName(string database, string schema, string name)
		{
			Database = database;
			Schema = schema;
			Name = name;
		}

		static readonly char[] s_DotSeparator = ['.'];

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
		/// </summary>
		/// <param name="qualifiedName">Name of the qualified.</param>
		/// <exception cref="ArgumentException">
		/// Fully qualified name is null or empty.;qualifiedName
		/// or
		/// Four-part identifiers are not supported.
		/// </exception>
		public PostgreSqlObjectName(string qualifiedName)
		{
			if (string.IsNullOrEmpty(qualifiedName))
				throw new ArgumentException("Fully qualified name is null or empty.", nameof(qualifiedName));

			var parts = qualifiedName.Split(s_DotSeparator, 3);
			if (parts.Length == 1)
			{
				Database = null;
				Schema = null;
				Name = parts[0];
			}
			else if (parts.Length == 2)
			{
				Database = null;
				Schema = parts[0];
				Name = parts[1];
			}
			else if (parts.Length == 3)
			{
				Database = parts[0];
				Schema = parts[1];
				Name = parts[2];
			}
			else
			{
				throw new ArgumentException("Four-part identifiers are not supported.");
			}
		}

		/// <summary>
		/// Gets the database.
		/// </summary>
		/// <value>
		/// The database.
		/// </value>
		public string? Database { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; }

		/// <summary>
		/// Gets the schema.
		/// </summary>
		/// <value>
		/// The schema.
		/// </value>
		public string? Schema { get; }

#pragma warning disable CA2225 // Operator overloads have named alternates

		/// <summary>
		/// Perform an implicit conversion from <see cref="string"/> to <see cref="PostgreSqlObjectName"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator PostgreSqlObjectName(string value) => new PostgreSqlObjectName(value);

#pragma warning restore CA2225 // Operator overloads have named alternates

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(PostgreSqlObjectName left, PostgreSqlObjectName right) => !(left == right);

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(PostgreSqlObjectName left, PostgreSqlObjectName right)
		{
			return string.Equals(left.Database, right.Database, StringComparison.OrdinalIgnoreCase)
				&& string.Equals(left.Schema, right.Schema, StringComparison.OrdinalIgnoreCase)
				&& string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object? obj)
		{
			var other = obj as AbstractObjectName?;
			if (other == null)
				return false;
			return this == other;
		}

		/// <summary>
		/// Equalses the specified other.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <returns></returns>
		public bool Equals(AbstractObjectName other) => this == other;

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// To the quoted string.
		/// </summary>
		/// <returns>System.String.</returns>
		public string ToQuotedString()
		{
			if (Schema == null)
				return $"\"{Name}\"";
			else if (Database == null)
				return $"\"{Schema}\".\"{Name}\"";
			else
				return $"\"{Database}\".\"{Schema}\".\"{Name}\"";
		}

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (Schema == null)
				return $"{Name}";
			else if (Database == null)
				return $"{Schema}.{Name}";
			else
				return $"{Database}.{Schema}.{Name}";
		}
	}
}
