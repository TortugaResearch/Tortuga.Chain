using System.Collections.Generic;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerTableOrViewMetadata.
    /// </summary>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    /// <seealso cref="TableOrViewMetadata{SqlServerObjectName, TDbType}" />
    public class SqlServerTableOrViewMetadata<TDbType> : TableOrViewMetadata<SqlServerObjectName, TDbType> where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableOrViewMetadata{TDbType}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> is a table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="hasTriggers">if set to <c>true</c> has triggers.</param>
        public SqlServerTableOrViewMetadata(SqlServerObjectName name, bool isTable, IList<ColumnMetadata<TDbType>> columns, bool hasTriggers) : base(name, isTable, columns)
        {
            HasTriggers = hasTriggers;
        }

        /// <summary>
        /// Gets a value indicating whether the associated table has triggers.
        /// </summary>
        /// <value><c>true</c> if this instance has triggers; otherwise, <c>false</c>.</value>
        /// <remarks>This affects SQL generation.</remarks>
        public bool HasTriggers { get; }
    }
}
