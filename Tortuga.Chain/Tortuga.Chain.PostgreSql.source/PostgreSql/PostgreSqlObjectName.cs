using System;
using System.Globalization;

namespace Tortuga.Chain.PostgreSql
{
    public struct PostgreSqlObjectName
    {
        public static readonly PostgreSqlObjectName Empty;

        private readonly string m_Database;
        private readonly string m_Schema;
        private readonly string m_Name;

        public PostgreSqlObjectName(string schema, string name)
            : this(null, schema, name)
        {
        }

        public PostgreSqlObjectName(string database, string schema, string name)
        {
            m_Database = database;
            m_Schema = schema;
            m_Name = name;
        }

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

        public string Database
        {
            get { return m_Database; }
        }

        public string Schema
        {
            get { return m_Schema; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public static implicit operator PostgreSqlObjectName(string value)
        {
            return new PostgreSqlObjectName(value);
        }

        public static bool operator !=(PostgreSqlObjectName left, PostgreSqlObjectName right)
        {
            return !(left == right);
        }

        public static bool operator ==(PostgreSqlObjectName left, PostgreSqlObjectName right)
        {
            return string.Equals(left.Database, right.Database, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Schema, right.Schema, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var other = obj as PostgreSqlObjectName?;
            if (other == null)
                return false;
            return this == other;
        }

        public bool Equals(PostgreSqlObjectName other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Name.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
        }


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
