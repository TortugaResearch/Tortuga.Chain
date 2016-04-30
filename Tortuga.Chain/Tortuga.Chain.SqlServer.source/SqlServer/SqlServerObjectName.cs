using System;
using System.Globalization;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Represents an object in SQL Server (e.g. table, view, procedure)
    /// </summary>
    public struct SqlServerObjectName
    {
        /// <summary>
        /// An empty schema/name pair
        /// </summary>
        public static readonly SqlServerObjectName Empty;

        readonly string m_Database;
        readonly string m_Name;
        readonly string m_Schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerObjectName"/> struct.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public SqlServerObjectName(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerObjectName" /> struct.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public SqlServerObjectName(string database, string schema, string name)
        {
            m_Database = Normalize(database);
            m_Schema = Normalize(schema);
            m_Name = Normalize(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerObjectName"/> struct.
        /// </summary>
        /// <param name="schemaAndName">Name of the schema and.</param>
        public SqlServerObjectName(string schemaAndName)
        {
            if (string.IsNullOrEmpty(schemaAndName))
                throw new ArgumentException("schemaAndName is null or empty.", "schemaAndName");

            var parts = schemaAndName.Split(new[] { '.' }, 2);
            if (parts.Length == 1)
            {
                m_Database = null;
                m_Schema = null;
                m_Name = Normalize(parts[0]);
            }
            else if (parts.Length == 2)
            {
                m_Database = null;
                m_Schema = Normalize(parts[0]);
                m_Name = Normalize(parts[1]);
            }
            else if (parts.Length == 3)
            {
                m_Database = Normalize(parts[1]);
                m_Schema = Normalize(parts[1]);
                m_Name = Normalize(parts[2]);
            }
            else
            {
                throw new ArgumentException("Four-part identifiers are not supported.");
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public string Database
        {
            get { return m_Database; }
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
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="SqlServerObjectName"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlServerObjectName(string value)
        {
            return new SqlServerObjectName(value);
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
        public static bool operator !=(SqlServerObjectName left, SqlServerObjectName right)
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
        public static bool operator ==(SqlServerObjectName left, SqlServerObjectName right)
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
        /// <remarks>This is a case-insensitive comparison.</remarks>
        public override bool Equals(object obj)
        {
            var other = obj as SqlServerObjectName?;
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
        public bool Equals(SqlServerObjectName other)
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
            if (Schema == null)
                return $"[{Name}]";
            else if (Database == null)
                return $"[{Schema}].[{Name}]";
            else
                return $"[{Database}].[{Schema}].[{Name}]";
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

        static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("[", "").Replace("]", "");
        }
    }
}