using System;
using System.Globalization;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Represents an object in PostgreSql (e.g. table, view, procedure)
    /// </summary>
    public struct PostgreSqlObjectName
    {
        /// <summary>
        /// An empty schema/name pair
        /// </summary>
        public static readonly PostgreSqlObjectName Empty;

        private readonly string m_Database;
        private readonly string m_Schema;
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public PostgreSqlObjectName(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public PostgreSqlObjectName(string database, string schema, string name)
        {
            m_Database = database;
            m_Schema = schema;
            m_Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlObjectName"/> struct.
        /// </summary>
        /// <param name="qualifiedName">Name of the qualified.</param>
        /// <exception cref="System.ArgumentException">
        /// Fully qualified name is null or empty.;qualifiedName
        /// or
        /// Four-part identifiers are not supported.
        /// </exception>
        public PostgreSqlObjectName(string qualifiedName)
        {
            if (string.IsNullOrEmpty(qualifiedName))
                throw new ArgumentException("Fully qualified name is null or empty.", "qualifiedName");

            var parts = qualifiedName.Split(new[] { '.' }, 3);
            if (parts.Length == 1)
            {
                m_Database = null;
                m_Schema = null;
                m_Name = parts[0];
            }
            else if (parts.Length == 2)
            {
                m_Database = null;
                m_Schema = parts[0];
                m_Name = parts[1];
            }
            else if (parts.Length == 3)
            {
                m_Database = parts[0];
                m_Schema = parts[1];
                m_Name = parts[2];
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
        public string Database
        {
            get { return m_Database; }
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>
        /// The schema.
        /// </value>
        public string Schema
        {
            get { return m_Schema; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="PostgreSqlObjectName"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator PostgreSqlObjectName(string value)
        {
            return new PostgreSqlObjectName(value);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(PostgreSqlObjectName left, PostgreSqlObjectName right)
        {
            return !(left == right);
        }

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
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as PostgreSqlObjectName?;
            if (other == null)
                return false;
            return this == other;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(PostgreSqlObjectName other)
        {
            return this == other;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Name.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
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
    }
}
