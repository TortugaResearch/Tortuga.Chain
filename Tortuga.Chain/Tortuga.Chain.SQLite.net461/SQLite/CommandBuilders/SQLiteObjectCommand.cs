using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class SQLiteObjectCommand.
    /// </summary>
    public abstract class SQLiteObjectCommand : SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        private readonly string m_TableName;
        private readonly object m_ArgumentValue;
        private readonly TableOrViewMetadata<string, DbType> m_Metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteObjectCommand" /> class
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected SQLiteObjectCommand(SQLiteDataSourceBase dataSource, string tableName, object argumentValue)
            : base(dataSource)
        {
            m_TableName = tableName;
            m_ArgumentValue = argumentValue;
            m_Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(m_TableName);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        protected string TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <value>The argument value.</value>
        protected object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public TableOrViewMetadata<string, DbType> Metadata
        {
            get { return m_Metadata; }
        }

        /// <summary>
        /// Wheres the clause.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="useKeyAttribute">if set to <c>true</c> [use key attribute].</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Loads the parameters.
        /// </summary>
        /// <param name="columnProps">The column props.</param>
        /// <param name="parameters">The parameters.</param>
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
    }
}
