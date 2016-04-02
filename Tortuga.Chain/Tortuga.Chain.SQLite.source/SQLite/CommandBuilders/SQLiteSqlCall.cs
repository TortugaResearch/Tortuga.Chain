using System;
using System.Collections.Generic;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Core;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class that represents an operation based on a raw SQL statement.
    /// </summary>
    internal sealed class SQLiteSqlCall : MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        readonly LockType m_LockType;
        private readonly object m_ArgumentValue;
        private readonly string m_SqlStatement;

        /// <summary>
        /// Creates a new instance of <see cref="SQLiteSqlCall" />
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <exception cref="ArgumentException">SQL statement is null or empty.;sqlStatement</exception>
        public SQLiteSqlCall(SQLiteDataSourceBase dataSource, string sqlStatement, object argumentValue, LockType lockType) :
            base(dataSource)
        {
            m_LockType = lockType;
            if (string.IsNullOrEmpty(sqlStatement))
                throw new ArgumentException("SQL statement is null or empty.", "sqlStatement");

            m_SqlStatement = sqlStatement;
            m_ArgumentValue = argumentValue;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns></returns>
        public override ExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
        {
            var parameters = new List<SQLiteParameter>();

            if (m_ArgumentValue is IEnumerable<SQLiteParameter>)
                foreach (var param in (IEnumerable<SQLiteParameter>)m_ArgumentValue)
                    parameters.Add(param);
            else if (m_ArgumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)m_ArgumentValue)
                    parameters.Add(new SQLiteParameter("@" + item.Key, item.Value ?? DBNull.Value));
            else if (m_ArgumentValue != null)
                foreach (var property in MetadataCache.GetMetadata(m_ArgumentValue.GetType()).Properties)
                    parameters.Add(new SQLiteParameter("@" + property.MappedColumnName, property.InvokeGet(m_ArgumentValue) ?? DBNull.Value));

            return new SQLiteExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, parameters, lockType: m_LockType);
        }
    }
}
