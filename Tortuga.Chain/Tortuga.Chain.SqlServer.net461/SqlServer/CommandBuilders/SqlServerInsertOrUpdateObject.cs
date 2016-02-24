using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerInsertOrUpdateObject.
    /// </summary>
    public class SqlServerInsertOrUpdateObject : SqlServerObjectCommand
    {
        private readonly InsertOrUpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerInsertOrUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, InsertOrUpdateOptions options) : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Formatter<SqlCommand, SqlParameter> formatter)
        {
            var parameters = new List<SqlParameter>();

            string on = OnClause(m_Options.HasFlag(InsertOrUpdateOptions.UseKeyAttribute));
            string set = UpdateClauses();
            string insertColumns;
            string insertValues;
            string source = SourceClause(parameters);
            InsertClauses(out insertColumns, out insertValues);

            var sql = $"MERGE INTO {TableName.ToQuotedString()} target USING {source} ON {on} WHEN MATCHED THEN UPDATE SET {set} WHEN NOT MATCHED THEN INSERT ( {insertColumns} ) VALUES ({insertValues}) ;";

            return new ExecutionToken<SqlCommand, SqlParameter>(DataSource, "Insert or update " + Metadata.Name, sql, parameters);
        }


        private string SourceClause(List<SqlParameter> parameters)
        {
            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.None).Where(c => !c.Column.IsIdentity);


            foreach (var item in availableColumns)
            {
                var value = item.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                if (item.Column.SqlDbType.HasValue)
                    parameter.SqlDbType = item.Column.SqlDbType.Value;
                parameters.Add(parameter);
            }


            return "(VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")) AS source (" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";

        }


        private string UpdateClauses()
        {
            var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly;

            if (m_Options.HasFlag(InsertOrUpdateOptions.UseKeyAttribute))
                filter = filter | GetPropertiesFilter.ObjectDefinedNonKey;
            else
                filter = filter | GetPropertiesFilter.NonPrimaryKey;

            if (DataSource.StrictMode)
                filter = filter | GetPropertiesFilter.ThrowOnMissingColumns;

            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

            return string.Join(", ", availableColumns.Select(c => $"target.{c.Column.QuotedSqlName} = source.{c.Column.QuotedSqlName}"));
        }

        private void InsertClauses(out string insertColumns, out string insertValues)
        {
            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.UpdatableOnly);

            insertColumns = string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName}"));
            insertValues = string.Join(", ", availableColumns.Select(c => $"source.{c.Column.QuotedSqlName}"));
        }

        private string OnClause(bool useKeyAttribute)
        {
            GetPropertiesFilter filter;
            if (useKeyAttribute)
                filter = (GetPropertiesFilter.ObjectDefinedKey | GetPropertiesFilter.ThrowOnMissingColumns);
            else
                filter = (GetPropertiesFilter.PrimaryKey | GetPropertiesFilter.ThrowOnMissingProperties);

            var columns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter).Where(c => !c.Column.IsIdentity);

            return string.Join(" AND ", columns.Select(c => $"target.{c.Column.QuotedSqlName} = source.{c.Column.QuotedSqlName}"));
        }
    }
}
