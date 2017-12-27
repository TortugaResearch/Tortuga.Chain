using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Csv
{

    /// <summary>
    /// Materializes the result set as a CSV string.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class CsvStringMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, string> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        readonly CsvSerializer m_CsvSerializer;
        readonly bool m_IncludeHeaders;
        readonly IReadOnlyList<string> m_DesiredColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvStringMaterializer{TCommand, TParameter}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        /// <param name="csvSerializer">The CSV serializer.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <param name="desiredColumns">The desired columns.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public CsvStringMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CsvSerializer csvSerializer, bool includeHeaders, IReadOnlyList<string> desiredColumns)
                : base(commandBuilder)
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
            if (csvSerializer == null)
                throw new ArgumentNullException(nameof(csvSerializer), $"{nameof(csvSerializer)} is null.");

            m_CsvSerializer = csvSerializer;
            m_IncludeHeaders = includeHeaders;
            m_DesiredColumns = desiredColumns ?? AllColumns;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override string Execute(object state)
        {
            var result = new StringWriter();

            Prepare().Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    return m_CsvSerializer.Serialize(reader, result, m_IncludeHeaders);
                }
            }, state);

            return result.ToString();
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<string> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new StringWriter();

            await Prepare().ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    return await m_CsvSerializer.SerializeAsync(reader, result, m_IncludeHeaders, cancellationToken).ConfigureAwait(false);
                }
            }, cancellationToken, state).ConfigureAwait(false);

            return result.ToString();
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns>
        /// IReadOnlyList&lt;System.String&gt;.
        /// </returns>
        /// <remarks>
        /// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
        /// </remarks>
        public override IReadOnlyList<string> DesiredColumns() => m_DesiredColumns;

    }
}