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
    /// Formats the result set as a list of TimeSpan.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public class TimeSpanListMaterializer<TCommandType, TParameterType> : SingleColumnFormatter<TCommandType, TParameterType, List<TimeSpan>> where TCommandType : DbCommand
        where TParameterType : DbParameter
    {

        readonly ListOptions m_ListOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="listOptions">The list options.</param>
        /// <param name="columnName">Name of the desired column.</param>
        public TimeSpanListMaterializer(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, ListOptions listOptions, string columnName = null)
            : base(commandBuilder, columnName)
        {
            m_ListOptions = listOptions;

        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override List<TimeSpan> Execute(object state = null)
        {
            var result = new List<TimeSpan>();

            ExecuteCore(cmd =>
            {

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                        throw new DataException(string.Format("Expected one column but found {0} columns", reader.FieldCount));

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);
                    while (reader.Read())
                    {
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (reader.IsDBNull(i) && !discardNulls)
                                throw new DataException("Unexpected null value");

                            result.Add((TimeSpan)reader.GetValue(i));
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
        public override async Task<List<TimeSpan>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new List<TimeSpan>();

            await ExecuteCoreAsync(async cmd =>
            {

                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    if (reader.FieldCount > 1 && !m_ListOptions.HasFlag(ListOptions.IgnoreExtraColumns))
                        throw new DataException(string.Format("Expected one column but found {0} columns", reader.FieldCount));

                    var columnCount = m_ListOptions.HasFlag(ListOptions.FlattenExtraColumns) ? reader.FieldCount : 1;
                    var discardNulls = m_ListOptions.HasFlag(ListOptions.DiscardNulls);

                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (reader.IsDBNull(i) && !discardNulls)
                                throw new DataException("Unexpected null value");

                            result.Add((TimeSpan)reader.GetValue(i));
                        }
                    }
                }
            }, cancellationToken, state).ConfigureAwait(false);


            return result;
        }

    }
}