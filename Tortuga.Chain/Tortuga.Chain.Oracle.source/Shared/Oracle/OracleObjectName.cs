using System;
using System.Globalization;

namespace Tortuga.Chain.Oracle
{
    /// <summary>
    /// Represents an object in Oracle (e.g. table, view, procedure)
    /// </summary>
    public struct OracleObjectName
    {
        /// <summary>
        /// An empty schema/name pair
        /// </summary>
        public static readonly OracleObjectName Empty;

        readonly string m_Schema;
        readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleObjectName"/> struct.
        /// </summary>
        /// <param name="schemaAndName">The name.</param>
        public OracleObjectName(string schemaAndName)
        {
            if (string.IsNullOrEmpty(schemaAndName))
                throw new ArgumentException("schemaAndName is null or emtpy.", "schemaAndName");

            var parts = schemaAndName.Split(new[] { '.' });
            if(parts.Length == 1)
            {
                m_Schema = null;
                m_Name = Normalize(parts[0]);
            }
            else if (parts.Length == 2)
            {
                m_Schema = Normalize(parts[0]);
                m_Name = Normalize(parts[1]);
            }
            else
            {
                throw new ArgumentException("Three-part identifiers are not supported.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleObjectName" /> struct.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        public OracleObjectName(string schema, string name)
        {
            m_Schema = Normalize(schema);
            m_Name = Normalize(name);
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
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="OracleObjectName"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator OracleObjectName(string value)
        {
            return new OracleObjectName(value);
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
        public static bool operator !=(OracleObjectName left, OracleObjectName right)
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
        public static bool operator ==(OracleObjectName left, OracleObjectName right)
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
        public override bool Equals(object obj)
        {
            var other = obj as OracleObjectName?;
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
        public bool Equals(OracleObjectName other)
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
            return $"`{Name}`";
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

        /// <summary>
        /// Normalizes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("`", "").Replace("`", "");
        }
    }
}
