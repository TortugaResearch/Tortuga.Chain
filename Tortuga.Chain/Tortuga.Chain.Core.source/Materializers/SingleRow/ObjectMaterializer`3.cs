using System.Collections.Generic;
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
    internal sealed class ObjectMaterializer<TCommand, TParameter, TObject> : ConstructibleMaterializer<TCommand, TParameter, TObject, TObject>
        where TCommand : DbCommand
        where TObject : class
        where TParameter : DbParameter
    {

        readonly RowOptions m_RowOptions;

        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="rowOptions">The row options.</param>
        public ObjectMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;

            if (m_RowOptions.HasFlag(RowOptions.InferConstructor))
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
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override TObject Execute(object state = null)
        {
            IReadOnlyDictionary<string, object> row = null;

            var executionToken = Prepare();
            var rowCount = executionToken.Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    row = reader.ReadDictionary();
                    return (row != null ? 1 : 0) + reader.RemainingRowCount();
                }
            }, state);

            return ConstructObject(row, rowCount);
        }


        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TObject> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            IReadOnlyDictionary<string, object> row = null;

            var executionToken = Prepare();
            var rowCount = await executionToken.ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    row = await reader.ReadDictionaryAsync();
                    return (row != null ? 1 : 0) + await reader.RemainingRowCountAsync();
                }
            }, cancellationToken, state).ConfigureAwait(false);

            return ConstructObject(row, rowCount);
        }

        private TObject ConstructObject(IReadOnlyDictionary<string, object> row, int? rowCount)
        {
            if (rowCount == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new MissingDataException("No rows were returned");
            }
            else if (rowCount > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException($"Expected 1 row but received {rowCount} rows");
            }
            return MaterializerUtilities.ConstructObject<TObject>(row, ConstructorSignature);
        }

        /// <summary>
        /// Returns the list of columns the result materializer would like to have.
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

    }
}
