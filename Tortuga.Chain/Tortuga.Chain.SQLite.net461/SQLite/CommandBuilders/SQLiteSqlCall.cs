using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.SQLite.CommandBuilders
{
    /// <summary>
    /// Class that represents an operation based on a raw SQL statement.
    /// </summary>
    public class SQLiteSqlCall : MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter>
    {
        readonly LockType m_LockType;
        private readonly object m_ArgumentValue;
        private readonly string m_SqlStatement;

        /// <summary>
        /// Creates a new instance of <see cref="SQLiteSqlCall"/>
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="sqlStatement"></param>
        /// <param name="argumentValue"></param>
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
