using System;
using Npgsql;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Text;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Class PostgreSqlTableOrView
    /// </summary>
    public class PostgreSqlTableOrView : MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
    {
        private readonly object m_FilterValue;
        private readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Metadata;
        private readonly string m_WhereClause;
        private readonly object m_ArgumentValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public PostgreSqlTableOrView(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableOrViewName, object filterValue) : 
            base(dataSource)
        {
            if (tableOrViewName == PostgreSqlObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_FilterValue = filterValue;
            m_Metadata = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        public PostgreSqlTableOrView(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableOrViewName, string whereClause, object argumentValue)
            : base(dataSource)
        {
            if (tableOrViewName == PostgreSqlObjectName.Empty)
                throw new ArgumentException($"{nameof(tableOrViewName)} is empty", nameof(tableOrViewName));

            m_WhereClause = whereClause;
            m_ArgumentValue = argumentValue;
            m_Metadata = DataSource.DatabaseMetadata.GetTableOrView(tableOrViewName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>
        /// ExecutionToken&lt;TCommand&gt;.
        /// </returns>
        public override ExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            List<NpgsqlParameter> parameters;

            var sqlBuilder = m_Metadata.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

            var sql = new StringBuilder();
            sqlBuilder.BuildSelectClause(sql, "SELECT ", null, " FROM " + m_Metadata.Name);

            throw new NotImplementedException();
        }

        public new PostgreSqlDataSourceBase DataSource
        {
            get { return (PostgreSqlDataSourceBase)base.DataSource;  }
        }
    }
}
