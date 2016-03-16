using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.Core;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerInsertOrUpdateObject.
    /// </summary>
    public class SqlServerInsertOrUpdateObject : SqlServerObjectCommand
    {
        private readonly UpsertOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerInsertOrUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerInsertOrUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, UpsertOptions options) : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>

        public override ExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            var parameters = new List<SqlParameter>();

            string on = OnClause(m_Options.HasFlag(UpsertOptions.UseKeyAttribute));
            string set = UpdateClauses();
            string insertColumns;
            string insertValues;
            string source = SourceClause(parameters);
            string output = OutputClause(materializer, false);
            InsertClauses(out insertColumns, out insertValues);

            var sql = $"MERGE INTO {TableName.ToQuotedString()} target USING {source} ON {on} WHEN MATCHED THEN UPDATE SET {set} WHEN NOT MATCHED THEN INSERT ( {insertColumns} ) VALUES ({insertValues}) {output} ;";

            return new SqlServerExecutionToken(DataSource, "Insert or update " + Metadata.Name, sql, parameters);
        }


        private string SourceClause(List<SqlParameter> parameters)
        {
            if (ArgumentDictionary != null)
            {
                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, GetKeysFilter.None);

                DataSource.LoadDictionaryParameters(ArgumentDictionary, parameters, availableColumns);

                return "(VALUES (" + string.Join(", ", availableColumns.Select(c => c.SqlVariableName)) + ")) AS source (" + string.Join(", ", availableColumns.Select(c => c.QuotedSqlName)) + ")";

            }
            else
            {
                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.None);

                DataSource.LoadParameters(ArgumentValue, parameters, availableColumns);

                return "(VALUES (" + string.Join(", ", availableColumns.Select(c => c.Column.SqlVariableName)) + ")) AS source (" + string.Join(", ", availableColumns.Select(c => c.Column.QuotedSqlName)) + ")";
            }
        }


        private string UpdateClauses()
        {
            if (ArgumentDictionary != null)
            {
                var filter = GetKeysFilter.ThrowOnNoMatch | GetKeysFilter.MutableColumns | GetKeysFilter.NonPrimaryKey;

                if (DataSource.StrictMode)
                    filter = filter | GetKeysFilter.ThrowOnMissingColumns;

                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, filter);

                return string.Join(", ", availableColumns.Select(c => $"target.{c.QuotedSqlName} = source.{c.QuotedSqlName}"));

            }
            else
            {
                var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.MutableColumns | GetPropertiesFilter.ForUpdate;

                if (m_Options.HasFlag(UpsertOptions.UseKeyAttribute))
                    filter = filter | GetPropertiesFilter.ObjectDefinedNonKey;
                else
                    filter = filter | GetPropertiesFilter.NonPrimaryKey;

                if (DataSource.StrictMode)
                    filter = filter | GetPropertiesFilter.ThrowOnMissingColumns;

                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

                return string.Join(", ", availableColumns.Select(c => $"target.{c.Column.QuotedSqlName} = source.{c.Column.QuotedSqlName}"));
            }
        }

        private void InsertClauses(out string insertColumns, out string insertValues)
        {
            if (ArgumentDictionary != null)
            {
                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, GetKeysFilter.MutableColumns);

                insertColumns = string.Join(", ", availableColumns.Select(c => $"{c.QuotedSqlName}"));
                insertValues = string.Join(", ", availableColumns.Select(c => $"source.{c.QuotedSqlName}"));

            }
            else
            {
                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), GetPropertiesFilter.MutableColumns | GetPropertiesFilter.ForInsert);

                insertColumns = string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName}"));
                insertValues = string.Join(", ", availableColumns.Select(c => $"source.{c.Column.QuotedSqlName}"));
            }
        }

        private string OnClause(bool useKeyAttribute)
        {
            if (ArgumentDictionary != null)
            {
                GetKeysFilter filter = (GetKeysFilter.PrimaryKey | GetKeysFilter.ThrowOnMissingProperties);

                var columns = Metadata.GetKeysFor(ArgumentDictionary, filter);

                return string.Join(" AND ", columns.Select(c => $"target.{c.QuotedSqlName} = source.{c.QuotedSqlName}"));

            }
            else
            {
                GetPropertiesFilter filter;
                if (useKeyAttribute)
                    filter = (GetPropertiesFilter.ObjectDefinedKey | GetPropertiesFilter.ThrowOnMissingColumns);
                else
                    filter = (GetPropertiesFilter.PrimaryKey | GetPropertiesFilter.ThrowOnMissingProperties);

                var columns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter); //.Where(c => !c.Column.IsIdentity);

                return string.Join(" AND ", columns.Select(c => $"target.{c.Column.QuotedSqlName} = source.{c.Column.QuotedSqlName}"));
            }
        }
    }
}
