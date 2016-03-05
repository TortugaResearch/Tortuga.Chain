using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteUpdateObject.
    /// </summary>
    /// <seealso cref="Tortuga.Chain.SQLite.SQLite.CommandBuilders.SQLiteObjectCommand" />
    public class SQLiteUpdateObject : SQLiteObjectCommand
    {
        private readonly UpdateOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteUpdateObject"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public SQLiteUpdateObject(SQLiteDataSourceBase dataSource, string tableName, object argumentValue, UpdateOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the specified materializer.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;SQLiteCommand, SQLiteParameter&gt;.</returns>
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            var set = SetClause(parameters);
            var where = WhereClause(parameters, m_Options.HasFlag(UpdateOptions.UseKeyAttribute));
            var output = OutputClause(materializer, where);
            string sql;

            if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
                sql = $"{output}; UPDATE {TableName} {set} WHERE {where};";
            else
                sql = $"UPDATE {TableName} {set} WHERE {where}; {output};";

            return new SQLiteExecutionToken(DataSource, "Update " + TableName, sql, parameters, lockType: LockType.Write);
        }

        private string SetClause(List<SQLiteParameter> parameters)
        {
            var filter = GetPropertiesFilter.ThrowOnNoMatch | GetPropertiesFilter.UpdatableOnly;

            if (m_Options.HasFlag(UpdateOptions.UseKeyAttribute))
                filter |= GetPropertiesFilter.ObjectDefinedNonKey;
            else
                filter |= GetPropertiesFilter.NonPrimaryKey;

            if (DataSource.StrictMode)
                filter |= GetPropertiesFilter.ThrowOnMissingColumns;

            var availableColumns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

            var set = "SET " + string.Join(", ", availableColumns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));
            LoadParameters(availableColumns, parameters);
            return set;
        }

        private string OutputClause(Materializer<SQLiteCommand, SQLiteParameter> materializer, string whereClause)
        {
            if (materializer is NonQueryMaterializer<SQLiteCommand, SQLiteParameter>)
                return null;

            var desiredColumns = materializer.DesiredColumns().ToLookup(c => c);
            if (desiredColumns.Count > 0)
            {
                var availableColumns = Metadata.Columns.Where(c => desiredColumns.Contains(c.ClrName)).ToList();
                if (availableColumns.Count == 0)
                    throw new MappingException($"None of the requested columns [{string.Join(", ", desiredColumns)}] were found on table {TableName}.");
                return $"SELECT {string.Join(", ", availableColumns.Select(c => c.QuotedSqlName))} FROM {TableName} {whereClause}";
            }
            else if (Metadata.Columns.Any(c => c.IsPrimaryKey))
            {
                var availableColumns = Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => c.QuotedSqlName).ToList();
                return $"SELECT {string.Join(", ", availableColumns)} FROM {TableName} {whereClause}";
            }
            else
            {
                return "SELECT ROWID FROM {TableName} {whereClause}";
            }
        }
    }
}
