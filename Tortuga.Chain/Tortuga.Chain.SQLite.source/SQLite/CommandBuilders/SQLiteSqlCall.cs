using System;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Core;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif


namespace Tortuga.Chain.SQLite.CommandBuilders
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
            return new SQLiteExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, SqlBuilder.GetParameters<SQLiteParameter>(m_ArgumentValue), lockType: m_LockType);
        }
    }
}
