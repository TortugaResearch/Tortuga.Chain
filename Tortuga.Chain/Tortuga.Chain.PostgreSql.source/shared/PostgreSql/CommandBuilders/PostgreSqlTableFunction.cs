using Npgsql;
using System;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlTableFunction.
    /// </summary>
    public class PostgreSqlTableFunction : TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTableFunction" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="functionArgumentValue">The function argument value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public PostgreSqlTableFunction(ICommandDataSource<NpgsqlCommand, NpgsqlParameter> dataSource, PostgreSqlObjectName tableFunctionName, object functionArgumentValue) : base(dataSource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the row count using a <c>SELECT Count(*)</c> style query.
        /// </summary>
        /// <returns>ILink&lt;System.Int64&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ILink<long> AsCount()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the row count for a given column. <c>SELECT Count(columnName)</c>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
        /// <returns>ILink&lt;System.Int64&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ILink<long> AsCount(string columnName, bool distinct = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>ColumnMetadata.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>If the column name was not found, this will return null</remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(string whereClause)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(object filterValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (or replaces) the filter on this command builder.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithFilter(string whereClause, object argumentValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds sorting to the command builder.
        /// </summary>
        /// <param name="sortExpressions">The sort expressions.</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> WithSorting(IEnumerable<SortExpression> sortExpressions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, LimitOptions limitOptions, int? seed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds limits to the command builder.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">Number of rows to take.</param>
        /// <param name="limitOptions">The limit options.</param>
        /// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
        /// <returns>TableDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption> OnWithLimits(int? skip, int? take, PostgreSqlLimitOption limitOptions, int? seed)
        {
            throw new NotImplementedException();
        }
    }
}
