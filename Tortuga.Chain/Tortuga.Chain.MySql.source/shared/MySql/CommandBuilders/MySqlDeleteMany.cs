using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Class MySqlDeleteMany.
    /// </summary>
    internal sealed class MySqlDeleteMany : MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<MySqlParameter> m_Parameters;
        readonly TableOrViewMetadata<MySqlObjectName, MySqlDbType> m_Table;
        readonly string m_WhereClause;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public MySqlDeleteMany(MySqlDataSourceBase dataSource, MySqlObjectName tableName, string whereClause, IEnumerable<MySqlParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return m_Table.Columns.TryGetColumn(columnName);
        }

    }
}

