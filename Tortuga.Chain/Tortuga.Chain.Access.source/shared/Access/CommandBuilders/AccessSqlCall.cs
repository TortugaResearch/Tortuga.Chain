using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;


namespace Tortuga.Chain.Access.CommandBuilders
{
    /// <summary>
    /// Class that represents an operation based on a raw SQL statement.
    /// </summary>
    internal sealed class AccessSqlCall : MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter>
    {
        readonly object m_ArgumentValue;
        readonly string m_SqlStatement;

        /// <summary>
        /// Creates a new instance of <see cref="AccessSqlCall" />
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="System.ArgumentException">SQL statement is null or empty.;sqlStatement</exception>
        public AccessSqlCall(AccessDataSourceBase dataSource, string sqlStatement, object argumentValue) :
            base(dataSource)
        {
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
        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
        {
            return new AccessCommandExecutionToken(DataSource, "Raw SQL call", m_SqlStatement, SqlBuilder.GetParameters<OleDbParameter>(m_ArgumentValue));
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName) => null;

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => ImmutableList<ColumnMetadata>.Empty;
    }
}
