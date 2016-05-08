using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a list of DateTimeOffset.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DateTimeOffsetSetMaterializer<TCommand, TParameter> : SingleColumnMaterializer<TCommand, TParameter, HashSet<DateTimeOffset>> where TCommand : DbCommand
        where TParameter : DbParameter

    {

        readonly ListOptions m_ListOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeOffsetListMaterializer{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="listOptions">The list options.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public DateTimeOffsetSetMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string columnName = null, ListOptions listOptions = ListOptions.None)
            : base(commandBuilder, columnName)
        {
            m_ListOptions = listOptions;

        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override HashSet<DateTimeOffset> Execute(object state = null)
        {
            var result = new HashSet<DateTimeOffset>();

            Prepare().Execute(cmd =>
            {

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                    {
                        throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");
                    }

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        rowCount++;
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (!reader.IsDBNull(i))
                                result.Add((DateTimeOffset)reader.GetValue(i));
                            else if (!discardNulls)
                                throw new MissingDataException("Unexpected null value");
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
        public override async Task<HashSet<DateTimeOffset>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new HashSet<DateTimeOffset>();

            await Prepare().ExecuteAsync(async cmd =>
            {

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                    {
                        throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");
                    }

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

                    var rowCount = 0;
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        rowCount++;
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (!reader.IsDBNull(i))
                                result.Add((DateTimeOffset)reader.GetValue(i));
                            else if (!discardNulls)
                                throw new MissingDataException("Unexpected null value");
                        }
                    }
                    return rowCount;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            return result;
        }

    }
}