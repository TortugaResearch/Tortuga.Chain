using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    internal class RefreshMaterializer<TCommand, TParameter, TArgument> : Materializer<TCommand, TParameter, TArgument>
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TArgument : class
    {
        private readonly ClassMetadata m_ObjectMetadata;
        private readonly ObjectDbCommandBuilder<TCommand, TParameter, TArgument> m_CommandBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshMaterializer{TCommand, TParameter, TArgument}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        public RefreshMaterializer(ObjectDbCommandBuilder<TCommand, TParameter, TArgument> commandBuilder)
            : base(commandBuilder)
        {
            m_ObjectMetadata = MetadataCache.GetMetadata(typeof(TArgument));
            m_CommandBuilder = commandBuilder;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TArgument Execute(object state = null)
        {
            Table table = null;
            var executionToken = Prepare();
            executionToken.Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, state);

            if (table.Rows.Count == 0)
                throw new DataException("No rows were returned");
            else if (table.Rows.Count > 1)
                throw new DataException("Expected 1 row but received " + table.Rows.Count + " rows");

            //update the ArgumentValue with any new keys, calculated fields, etc.
            Table.PopulateComplexObject(table.Rows[0], m_CommandBuilder.ArgumentValue, null);

            return m_CommandBuilder.ArgumentValue;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TArgument> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            Table table = null;

            var executionToken = Prepare();
            await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);


            if (table.Rows.Count == 0)
                throw new DataException("No rows were returned");
            else if (table.Rows.Count > 1)
                throw new DataException("Expected 1 row but received " + table.Rows.Count + " rows");

            //update the ArgumentValue with any new keys, calculated fields, etc.
            Table.PopulateComplexObject(table.Rows[0], m_CommandBuilder.ArgumentValue, null);

            return m_CommandBuilder.ArgumentValue;
        }

        /// <summary>
        /// Returns the list of columns the result materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return m_ObjectMetadata.ColumnsFor;
        }

    }
}
