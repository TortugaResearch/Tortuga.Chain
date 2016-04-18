using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a collection of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    /// <typeparam name="TObject">The type of the object returned.</typeparam>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TCollection}" />
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    internal sealed class InitializedCollectionMaterializer<TCommand, TParameter, TObject, TCollection> : Materializer<TCommand, TParameter, TCollection>
        where TCommand : DbCommand
        where TObject : class
        where TCollection : ICollection<TObject>, new()
        where TParameter : DbParameter
    {
        private readonly Type[] m_ConstructorSignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializedCollectionMaterializer{TCommand, TParameter, TObject, TCollection}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        /// <param name="constructorSignature">The constructor signature.</param>
        public InitializedCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, Type[] constructorSignature)
            : base(commandBuilder)
        {
            m_ConstructorSignature = constructorSignature;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TCollection Execute(object state = null)
        {
            var result = new TCollection();
            Table table = null;
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, state);

            foreach (var item in table.ToObjects<TObject>(m_ConstructorSignature))
                result.Add(item);
            return result;
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TCollection> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new TCollection();

            Table table = null;
            await ExecuteCoreAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    table = new Table(reader);
                    return table.Rows.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);

            foreach (var item in table.ToObjects<TObject>(m_ConstructorSignature))
                result.Add(item);
            return result;
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            var desiredType = typeof(TObject);
            var constructor = desiredType.GetConstructor(m_ConstructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", m_ConstructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            return ImmutableList.CreateRange(constructor.GetParameters().Select(p => p.Name));
        }
    }
}
