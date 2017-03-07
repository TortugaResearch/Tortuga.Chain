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
    /// Class OracleUpdateMany.
    /// </summary>
    internal sealed class OracleUpdateMany : UpdateManyCommandBuilder<OracleCommand, OracleParameter>
    {
        readonly int? m_ExpectedRowCount;
        readonly IEnumerable<OracleParameter> m_Parameters;
        readonly TableOrViewMetadata<OracleObjectName, OracleDbType> m_Table;

        readonly object m_NewValues;
        readonly string m_UpdateExpression;
        readonly object m_UpdateArgumentValue;
        readonly UpdateOptions m_Options;

        string m_WhereClause;
        object m_WhereArgumentValue;
        object m_FilterValue;
        FilterOptions m_FilterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedRowCount">The expected row count.</param>
        /// <param name="options">The options.</param>
        public OracleUpdateMany(OracleDataSourceBase dataSource, OracleObjectName tableName, object newValues, string whereClause, IEnumerable<OracleParameter> parameters, int? expectedRowCount, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_NewValues = newValues;
            m_WhereClause = whereClause;
            m_ExpectedRowCount = expectedRowCount;
            m_Options = options;
            m_Parameters = parameters;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OracleUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="newValues">The new values.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.NotSupportedException">Cannot use Key attributes with this operation.</exception>
        public OracleUpdateMany(OracleDataSourceBase dataSource, OracleObjectName tableName, object newValues, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_NewValues = newValues;
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleUpdateMany" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateExpression">The update expression.</param>
        /// <param name="updateArgumentValue">The update argument value.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.NotSupportedException">Cannot use Key attributes with this operation.</exception>
        public OracleUpdateMany(OracleDataSourceBase dataSource, OracleObjectName tableName, string updateExpression, object updateArgumentValue, UpdateOptions options) : base(dataSource)
        {
            if (options.HasFlag(UpdateOptions.UseKeyAttribute))
                throw new NotSupportedException("Cannot use Key attributes with this operation.");

            m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
            m_UpdateExpression = updateExpression;
            m_Options = options;
            m_UpdateArgumentValue = updateArgumentValue;
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

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOptions">The filter options.</param>
        public override UpdateManyCommandBuilder<OracleCommand, OracleParameter> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
        {
            m_WhereClause = null;
            m_WhereArgumentValue = null;
            m_FilterValue = filterValue;
            m_FilterOptions = filterOptions;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OracleCommand, OracleParameter> WithFilter(string whereClause)
        {
            m_WhereClause = whereClause;
            m_WhereArgumentValue = null;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OracleCommand, OracleParameter> WithFilter(string whereClause, object argumentValue)
        {
            m_WhereClause = whereClause;
            m_WhereArgumentValue = argumentValue;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }

        /// <summary>
        /// Applies this command to all rows.
        /// </summary>
        /// <returns></returns>
        public override UpdateManyCommandBuilder<OracleCommand, OracleParameter> All()
        {
            m_WhereClause = null;
            m_WhereArgumentValue = null;
            m_FilterValue = null;
            m_FilterOptions = FilterOptions.None;
            return this;
        }


    }
}


