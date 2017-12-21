using System;
using System.Collections.Generic;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Class MySqlTableOrViewMetadata.
    /// </summary>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    /// <seealso cref="TableOrViewMetadata{MySqlObjectName, TDbType}" />
    public class MySqlTableOrViewMetadata<TDbType> : TableOrViewMetadata<MySqlObjectName, TDbType> where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlTableOrViewMetadata{TDbType}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> is a table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="hasTriggers">if set to <c>true</c> has triggers.</param>
        public MySqlTableOrViewMetadata(MySqlObjectName name, bool isTable, IList<ColumnMetadata<TDbType>> columns) : base(name, isTable, columns)
        {

        }


    }
}
