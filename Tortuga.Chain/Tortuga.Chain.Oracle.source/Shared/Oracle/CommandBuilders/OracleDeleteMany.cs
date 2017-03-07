using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Class OracleDeleteMany.
    /// </summary>
    internal sealed class OracleDeleteMany : MultipleRowDbCommandBuilder<OracleCommand, OracleParameter>
    {
        //readonly DeleteOptions m_Options;
        readonly IEnumerable<OracleParameter> m_Parameters;
        readonly TableOrViewMetadata<OracleObjectName, OracleDbType> m_Table;
        readonly string m_WhereClause;
        readonly object m_ArgumentValue;
        readonly object m_FilterValue;
        readonly FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDeleteMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        public OracleDeleteMany(OracleDataSourceBase dataSource, OracleObjectName tableName, string whereClause, IEnumerable<OracleParameter> parameters, DeleteOptions options) : base(dataSource)
        {
            if (options.HasFlag(DeleteOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            //m_Options = options;
            m_Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDeleteMany"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        public OracleDeleteMany(OracleDataSourceBase dataSource, OracleObjectName tableName, string whereClause, object argumentValue) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDeleteMany"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The options.</param>
        public OracleDeleteMany(OracleDataSourceBase dataSource, OracleObjectName tableName, object filterValue, FilterOptions filterOptions) : base(dataSource)
        {
            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
        }

        public override CommandExecutionToken<OracleCommand, OracleParameter> Prepare(Materializer<OracleCommand, OracleParameter> materializer)
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

