﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a list of strings.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public class StringListMaterializer<TCommand, TParameter> : SingleColumnMaterializer<TCommand, TParameter, List<string>> where TCommand : DbCommand
        where TParameter : DbParameter
    {

        readonly ListOptions m_ListOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="listOptions">The list options.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public StringListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, ListOptions listOptions, string columnName = null)
            : base(commandBuilder, columnName)
        {
            m_ListOptions = listOptions;

        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override List<string> Execute(object state = null)
        {
            var result = new List<string>();

            ExecuteCore(cmd =>
            {

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                        throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);
                    while (reader.Read())
                    {
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (reader.IsDBNull(i))
                            {
                                if (!discardNulls)
                                    result.Add(null);
                            }
                            result.Add(reader.GetString(i));
                        }
                    }
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
        public override async Task<List<string>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new List<string>();

            await ExecuteCoreAsync(async cmd =>
            {

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                        throw new UnexpectedDataException($"Expected one column but found {reader.FieldCount} columns");

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (reader.IsDBNull(i))
                            {
                                if (!discardNulls)
                                    result.Add(null);
                            }
                            result.Add(reader.GetString(i));
                        }
                    }
                }
            }, cancellationToken, state).ConfigureAwait(false);


            return result;
        }

    }
}