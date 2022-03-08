﻿using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializes the result set as a list of Guids.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command type.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
	internal sealed class GuidOrNullListMaterializer<TCommand, TParameter> : SingleColumnMaterializer<TCommand, TParameter, List<Guid?>> where TCommand : DbCommand
		where TParameter : DbParameter
	{
		readonly ListOptions m_ListOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="GuidOrNullListMaterializer{TCommand, TParameter}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The command builder.</param>
		/// <param name="columnName">Name of the desired column.</param>
		/// <param name="listOptions">The list options.</param>
		public GuidOrNullListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string? columnName = null, ListOptions listOptions = ListOptions.None)
			: base(commandBuilder, columnName)
		{
			m_ListOptions = listOptions;
		}

		/// <summary>
		/// Execute the operation synchronously.
		/// </summary>
		/// <returns></returns>
		public override List<Guid?> Execute(object? state = null)
		{
			var result = new List<Guid?>();

			Prepare().Execute(cmd =>
			{
				using (var reader = cmd.ExecuteReader(CommandBehavior))
				{
					if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
						throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");

					var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
					var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);
					var rowCount = 0;
					while (reader.Read())
					{
						rowCount++;
						for (var i = 0; i < columnCount; i++)
						{
							if (!reader.IsDBNull(i))
								result.Add(reader.GetGuid(i));
							else if (!discardNulls)
								result.Add(null);
						}
					}
					return rowCount;
				}
			}, state);

			return result;
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User defined state, usually used for logging.</param>
		/// <returns></returns>
		public override async Task<List<Guid?>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
		{
			var result = new List<Guid?>();

			await Prepare().ExecuteAsync(async cmd =>
			{
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
				{
					if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
						throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");

					var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
					var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

					var rowCount = 0;
					while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						rowCount++;
						for (var i = 0; i < columnCount; i++)
						{
							if (!reader.IsDBNull(i))
								result.Add(reader.GetGuid(i));
							else if (!discardNulls)
								result.Add(null);
						}
					}
					return rowCount;
				}
			}, cancellationToken, state).ConfigureAwait(false);

			return result;
		}
	}
}
