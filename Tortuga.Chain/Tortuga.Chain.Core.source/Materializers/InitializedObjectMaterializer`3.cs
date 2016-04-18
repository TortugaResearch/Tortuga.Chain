using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as an instance of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    /// <typeparam name="TObject">The type of the object returned.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TTObject}" />
    internal sealed class InitializedObjectMaterializer<TCommand, TParameter, TObject> : Materializer<TCommand, TParameter, TObject>
        where TCommand : DbCommand
        where TObject : class
        where TParameter : DbParameter
    {

        readonly RowOptions m_RowOptions;
        readonly Type[] m_ConstructorSignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializedObjectMaterializer{TCommand, TParameter, TObject}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        /// <param name="constructorSignature">The constructor signature.</param>
        public InitializedObjectMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Type[] constructorSignature, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
            m_ConstructorSignature = constructorSignature;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TObject Execute(object state = null)
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
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                {
                    var ex = new DataException("No rows were returned");
                    throw ex;
                }
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                var ex = new DataException("Expected 1 row but received " + table.Rows.Count + " rows");
                throw ex;
            }
            return table.ToObjects<TObject>(m_ConstructorSignature).First();
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TObject> ExecuteAsync(CancellationToken cancellationToken, object state = null)
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
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                {
                    throw new DataException("No rows were returned");
                }
            }
            else if (table.Rows.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new DataException("Expected 1 row but received " + table.Rows.Count + " rows");
            }
            return table.ToObjects<TObject>(m_ConstructorSignature).First();
        }

        /// <summary>
        /// Returns the list of columns the result materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            var desiredType = typeof(TObject);
            var constructor = desiredType.GetConstructor(m_ConstructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", m_ConstructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constuctor on {desiredType.Name} with the types [{types}]");
            }

            return ImmutableList.CreateRange(constructor.GetParameters().Select(p => p.Name));
        }

    }
}
