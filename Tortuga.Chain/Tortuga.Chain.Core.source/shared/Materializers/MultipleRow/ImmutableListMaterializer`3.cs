using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as an immutable list of the indicated type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    /// <typeparam name="TObject">The type of the object returned.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TCollection}" />
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    internal sealed class ImmutableListMaterializer<TCommand, TParameter, TObject> : ConstructibleMaterializer<TCommand, TParameter, ImmutableList<TObject>, TObject>
        where TCommand : DbCommand
        where TObject : class
        where TParameter : DbParameter
    {

        readonly CollectionOptions m_CollectionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMaterializer{TCommand, TParameter, TObject, TCollection}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        /// <param name="collectionOptions">The collection options.</param>
        public ImmutableListMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, CollectionOptions collectionOptions)
            : base(commandBuilder)
        {
            m_CollectionOptions = collectionOptions;


            if (m_CollectionOptions.HasFlag(CollectionOptions.InferConstructor))
            {
                var constructors = ObjectMetadata.Constructors.Where(x => x.Signature.Length > 0).ToList();
                if (constructors.Count == 0)
                    throw new MappingException($"Type {typeof(TObject).Name} has does not have any non-default constructors.");
                if (constructors.Count > 1)
                    throw new MappingException($"Type {typeof(TObject).Name} has more than one non-default constructor. Please use the WithConstructor method to specify which one to use.");
                ConstructorSignature = constructors[0].Signature;
            }
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<string> DesiredColumns()
        {
            if (ConstructorSignature == null)
                return ObjectMetadata.ColumnsFor;

            var desiredType = typeof(TObject);
            var constructor = ObjectMetadata.Constructors.Find(ConstructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", ConstructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            return constructor.ParameterNames;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override ImmutableList<TObject> Execute(object state = null)
        {
            ImmutableList<TObject> result = null;
            Prepare().Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader().AsObjectConstructor<TObject>(ConstructorSignature, CommandBuilder.TryGetNonNullableColumns()))
                {
                    result = reader.ToObjects().ToImmutableList();
                    return result.Count;
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
        public override async Task<ImmutableList<TObject>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            ImmutableList<TObject> result = null;
            await Prepare().ExecuteAsync(async cmd =>
            {
                using (var reader = (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).AsObjectConstructor<TObject>(ConstructorSignature, CommandBuilder.TryGetNonNullableColumns()))
                {
                    result = (await reader.ToListAsync()).ToImmutableList();
                    return result.Count;
                }
            }, cancellationToken, state).ConfigureAwait(false);

            return result;
        }
    }
}
