﻿using System;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Represents an object in MySql (e.g. table, view, procedure)
    /// </summary>
    public struct MySqlObjectName
    {
        /// <summary>
        /// An empty schema/name pair
        /// </summary>
        public static readonly MySqlObjectName Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlObjectName" /> struct.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public MySqlObjectName(string schema, string name)
        {
            Schema = Normalize(schema);
            Name = Normalize(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlObjectName"/> struct.
        /// </summary>
        /// <param name="schemaAndName">Name of the schema and.</param>
        public MySqlObjectName(string schemaAndName)
        {
            if (string.IsNullOrEmpty(schemaAndName))
                throw new ArgumentException("schemaAndName is null or empty.", "schemaAndName");

            var parts = schemaAndName.Split(new[] { '.' }, 2);
            if (parts.Length == 1)
            {
                Schema = null;
                Name = Normalize(parts[0]);
            }
            else if (parts.Length == 2)
            {
                Schema = Normalize(parts[0]);
                Name = Normalize(parts[1]);
            }
            else
            {
                throw new ArgumentException("Three-part identifiers are not supported.");
            }
        }

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
        public string Schema { get; }

        /// <summary>
        /// Perform an implicit conversion from <see cref="string"/> to <see cref="MySqlObjectName"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator MySqlObjectName(string value)
        {
            return new MySqlObjectName(value);
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
        public static bool operator !=(MySqlObjectName left, MySqlObjectName right)
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
        public static bool operator ==(MySqlObjectName left, MySqlObjectName right)
        {
            return string.Equals(left.Schema, right.Schema, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>This is a case-insensitive comparison.</remarks>
        public override bool Equals(object obj)
        {
            var other = obj as MySqlObjectName?;
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
        public bool Equals(MySqlObjectName other)
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
            return Name.ToUpperInvariant().GetHashCode();
        }

        /// <summary>
        /// To the quoted string.
        /// </summary>
        /// <returns></returns>
        public string ToQuotedString()
        {
            if (Schema == null)
                return $"`{Name}`";
            else
                return $"`{Schema}`.`{Name}`";
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
            return $"{Schema}.{Name}";
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("`", "");
        }
    }
}