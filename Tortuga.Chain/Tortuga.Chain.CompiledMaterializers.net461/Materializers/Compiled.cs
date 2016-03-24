using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Extension for using compiled materializers with Tortuga Chain
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct Compiled<TCommand, TParameter>
            where TCommand : DbCommand
            where TParameter : DbParameter
    {

        private readonly DbCommandBuilder<TCommand, TParameter> m_CommandBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Compiled{TCommand, TParameter}"/> struct.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        public Compiled(DbCommandBuilder<TCommand, TParameter> commandBuilder)
        {
            m_CommandBuilder = commandBuilder;
        }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new CompiledObjectMaterializer<TCommand, TParameter, TObject>(m_CommandBuilder, rowOptions);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        public ILink<List<TObject>> ToCollection<TObject>()
            where TObject : class, new()
        {
            return new CompiledCollectionMaterializer<TCommand, TParameter, TObject, List<TObject>>(m_CommandBuilder);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ILink<TCollection> ToCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
        {
            return new CompiledCollectionMaterializer<TCommand, TParameter, TObject, TCollection>(m_CommandBuilder);
        }
    }
}
