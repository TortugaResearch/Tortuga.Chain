using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    public abstract class SQLiteObjectCommand : SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        private readonly string m_TableName;
        private readonly object m_ArgumentValue;
        private readonly TableOrViewMetadata<string, DbType> m_Metadata;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteObjectCommand"/> class
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="tableName"></param>
        protected SQLiteObjectCommand(SQLiteDataSourceBase dataSource, string tableName, object argumentValue)
            : base(dataSource)
        {
            m_TableName = tableName;
            m_ArgumentValue = argumentValue;
            m_Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(m_TableName);
        }

        protected string TableName
        {
            get { return m_TableName; }
        }

        protected object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        public TableOrViewMetadata<string, DbType> Metadata
        {
            get { return m_Metadata; }
        }

        protected string WhereClause(List<SQLiteParameter> parameters, bool useKeyAttribute)
        {
            GetPropertiesFilter filter;
            if (useKeyAttribute)
                filter = (GetPropertiesFilter.ObjectDefinedKey | GetPropertiesFilter.ThrowOnMissingColumns);
            else
                filter = (GetPropertiesFilter.PrimaryKey | GetPropertiesFilter.ThrowOnMissingProperties);

            var columns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);
            var where = string.Join(" AND ", columns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));
            LoadParameters(columns, parameters);
            return where;
        }

        protected void LoadParameters(ImmutableList<ColumnPropertyMap<DbType>> columnProps, List<SQLiteParameter> parameters)
        {
            foreach(var columnProp in columnProps)
            {
                var value = columnProp.Property.InvokeGet(ArgumentValue) ?? DBNull.Value;
                var parameter = new SQLiteParameter(columnProp.Column.SqlVariableName, value);
                if (columnProp.Column.DbType.HasValue)
                    parameter.DbType = columnProp.Column.DbType.Value;
                parameters.Add(parameter);
            }
        }

        protected string OutputClause(Materializer<SQLiteCommand, SQLiteParameter> materializer, string whereClause)
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
                return $"SELECT ROWID FROM {TableName} {whereClause}";
            }
        }
    }
}
