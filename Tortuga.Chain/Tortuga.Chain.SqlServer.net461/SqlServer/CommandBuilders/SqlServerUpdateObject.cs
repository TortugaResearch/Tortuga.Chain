using System;
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
    /// Class SqlServerUpdateObject.
    /// </summary>
    public class SqlServerUpdateObject : SqlServerObjectCommand
    {
        private readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SqlServerUpdateObject(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue, UpdateOptions options) : base(dataSource, tableName, argumentValue)
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

            var set = SetClause(parameters);
            var output = OutputClause(materializer, m_Options.HasFlag(UpdateOptions.ReturnOldValues));
            var where = WhereClause(parameters, m_Options.HasFlag(UpdateOptions.UseKeyAttribute));

            var sql = $"UPDATE {TableName.ToQuotedString()} {set} {output} WHERE {where}";

            return new SqlServerExecutionToken(DataSource, "Update " + TableName, sql, parameters);
        }

        private string SetClause(List<SqlParameter> parameters)
        {
            if (ArgumentDictionary != null)
            {

                var filter = GetKeysFilter.ThrowOnNoMatch | GetKeysFilter.UpdatableOnly;

                filter = filter | GetKeysFilter.NonPrimaryKey;

                if (DataSource.StrictMode)
                    filter = filter | GetKeysFilter.ThrowOnMissingColumns;

                var availableColumns = Metadata.GetKeysFor(ArgumentDictionary, filter);

                var result = "SET " + string.Join(", ", availableColumns.Select(c => $"{c.QuotedSqlName} = {c.SqlVariableName}"));

                foreach (var item in availableColumns)
                {
                    var value = ArgumentDictionary[item.ClrName] ?? DBNull.Value;
                    var parameter = new SqlParameter(item.SqlVariableName, value);
                    if (item.DbType.HasValue)
                        parameter.SqlDbType = item.DbType.Value;
                    parameters.Add(parameter);
                }

                return result;
            }
            else
            {
                var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly;

                if (m_Options.HasFlag(UpdateOptions.UseKeyAttribute))
                    filter = filter | GetPropertiesFilter.ObjectDefinedNonKey;
                else
                    filter = filter | GetPropertiesFilter.NonPrimaryKey;

                if (DataSource.StrictMode)
                    filter = filter | GetPropertiesFilter.ThrowOnMissingColumns;

                var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

                var result = "SET " + string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));

                foreach (var item in availableColumns)
                {
                    var value = item.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                    var parameter = new SqlParameter(item.Column.SqlVariableName, value);
                    if (item.Column.DbType.HasValue)
                        parameter.SqlDbType = item.Column.DbType.Value;
                    parameters.Add(parameter);
                }

                return result;
            }
        }
    }
}

