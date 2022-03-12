using Npgsql;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
	/// <summary>
	/// Class PostgreSqlDataSourceBase.
	/// </summary>
	public abstract partial class PostgreSqlDataSourceBase : DataSource<NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand, NpgsqlParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlDataSourceBase"/> class.
		/// </summary>
		/// <param name="settings">Optional settings object.</param>
		protected PostgreSqlDataSourceBase(DataSourceSettings? settings) : base(settings)
		{
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		public abstract new PostgreSqlMetadataCache DatabaseMetadata { get; }

		/// <summary>
		/// Dereferences cursors returned by a stored procedure.
		/// </summary>
		/// <param name="cmd">The command.</param>
		/// <param name="implementation">The implementation.</param>
		/// <returns>System.String.</returns>
		protected static int? DereferenceCursors(NpgsqlCommand cmd, CommandImplementation<NpgsqlCommand> implementation)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd), $"{nameof(cmd)} is null.");
			if (cmd.Connection == null)
				throw new ArgumentNullException($"{nameof(cmd)}.{nameof(cmd.Connection)}", $"{nameof(cmd)}.{nameof(cmd.Connection)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var closeTransaction = false;
			try
			{
				if (cmd.Transaction == null)
				{
					cmd.Transaction = cmd.Connection.BeginTransaction();
					closeTransaction = true;
				}

				var sql = new StringBuilder();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						sql.AppendLine($"FETCH ALL IN \"{ reader.GetString(0) }\";");

				using (var cmd2 = new NpgsqlCommand())
				{
					cmd2.Connection = cmd.Connection;
					cmd2.Transaction = cmd.Transaction;
					cmd2.CommandTimeout = cmd.CommandTimeout;
					cmd2.CommandText = sql.ToString();
					cmd2.CommandType = CommandType.Text;
					return implementation(cmd2);
				}
			}
			finally
			{
				if (closeTransaction)
					cmd.Transaction!.Commit();
			}
		}

		/// <summary>
		/// Dereferences cursors returned by a stored procedure.
		/// </summary>
		/// <param name="cmd">The command.</param>
		/// <param name="implementation">The implementation.</param>
		/// <returns>System.String.</returns>
		protected static async Task<int?> DereferenceCursorsAsync(NpgsqlCommand cmd, CommandImplementationAsync<NpgsqlCommand> implementation)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd), $"{nameof(cmd)} is null.");
			if (cmd.Connection == null)
				throw new ArgumentNullException($"{nameof(cmd)}.{nameof(cmd.Connection)}", $"{nameof(cmd)}.{nameof(cmd.Connection)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var closeTransaction = false;
			try
			{
				if (cmd.Transaction == null)
				{
					cmd.Transaction = cmd.Connection.BeginTransaction();
					closeTransaction = true;
				}

				var sql = new StringBuilder();
				using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
					while (await reader.ReadAsync().ConfigureAwait(false))
						sql.AppendLine($"FETCH ALL IN \"{ reader.GetString(0) }\";");

				using (var cmd2 = new NpgsqlCommand())
				{
					cmd2.Connection = cmd.Connection;
					cmd2.Transaction = cmd.Transaction;
					cmd2.CommandTimeout = cmd.CommandTimeout;
					cmd2.CommandText = sql.ToString();
					cmd2.CommandType = CommandType.Text;
					return await implementation(cmd2).ConfigureAwait(false);
				}
			}
			finally
			{
				if (closeTransaction)
					await cmd.Transaction!.CommitAsync().ConfigureAwait(false);
			}
		}


	}
}
