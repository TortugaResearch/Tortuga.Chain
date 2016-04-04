using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Base class that describes a SQLite database command.
    /// </summary>
    internal abstract class SQLiteObjectCommand : SingleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        private readonly string m_TableName;
        private readonly object m_ArgumentValue;
        private readonly IReadOnlyDictionary<string, object> m_ArgumentDictionary;
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
            m_ArgumentDictionary = ArgumentValue as IReadOnlyDictionary<string, object>;
            m_Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(m_TableName);
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        protected string TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        protected object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<string, DbType> Metadata
        {
            get { return m_Metadata; }
        }

    }
}
