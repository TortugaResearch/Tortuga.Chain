using MySqlConnector;
using System.Collections.Immutable;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders
{
	/// <summary>
	/// Class MySqlProcedureCall.
	/// </summary>
	internal sealed class MySqlProcedureCall : ProcedureDbCommandBuilder<MySqlCommand, MySqlParameter>
	{
		private readonly object? m_ArgumentValue;
		private readonly StoredProcedureMetadata<MySqlObjectName, MySqlDbType> m_Procedure;

		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlProcedureCall"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <param name="argumentValue">The argument value.</param>
		internal MySqlProcedureCall(MySqlDataSourceBase dataSource, MySqlObjectName procedureName, object? argumentValue = null) : base(dataSource)
		{
			if (procedureName == MySqlObjectName.Empty)
				throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

			m_ArgumentValue = argumentValue;
			m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public new MySqlDataSourceBase DataSource
		{
			get { return (MySqlDataSourceBase)base.DataSource; }
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer">The materializer.</param>
		/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
		public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			List<MySqlParameter> parameters;

			if (m_ArgumentValue is IEnumerable<MySqlParameter>)
			{
				parameters = ((IEnumerable<MySqlParameter>)m_ArgumentValue).ToList();
			}
			else
			{
				var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
				sqlBuilder.ApplyArgumentValue(DataSource, OperationTypes.None, m_ArgumentValue);
				parameters = sqlBuilder.GetParameters();
			}

			return new MySqlCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure);
		}

		/// <summary>
		/// Returns the column associated with the column name.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns></returns>
		/// <remarks>
		/// If the column name was not found, this will return null
		/// </remarks>
		public override ColumnMetadata? TryGetColumn(string columnName)
		{
			return null;
		}

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
