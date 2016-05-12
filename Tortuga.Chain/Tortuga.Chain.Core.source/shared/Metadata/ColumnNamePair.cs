using System;

namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// This is used to read out column names during SQL generation.
    /// </summary>
    public struct ColumnNamePair : IEquatable<ColumnNamePair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNamePair"/> struct.
        /// </summary>
        /// <param name="quotedSqlName">Name of the quoted SQL.</param>
        /// <param name="sqlVariableName">Name of the SQL variable.</param>
        public ColumnNamePair(string quotedSqlName, string sqlVariableName)
        {
            QuotedSqlName = quotedSqlName;
            SqlVariableName = sqlVariableName;
        }

        /// <summary>
        /// Gets or sets column name as quoted SQL.
        /// </summary>
        public string QuotedSqlName { get; }

        /// <summary>
        /// Gets or sets column name as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ColumnNamePair)
                return Equals((ColumnNamePair)obj);
            return base.Equals(obj);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ColumnNamePair first, ColumnNamePair second)
        {
            if ((object)first == null)
                return (object)second == null;
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ColumnNamePair first, ColumnNamePair second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ColumnNamePair other)
        {
            return QuotedSqlName == other.QuotedSqlName && SqlVariableName == other.SqlVariableName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return QuotedSqlName.GetHashCode() ^ SqlVariableName.GetHashCode();
            }
        }
    }
}
