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
    public struct CompiledSingleRow<TCommand, TParameter>
            where TCommand : DbCommand
            where TParameter : DbParameter
    {

        readonly SingleRowDbCommandBuilder<TCommand, TParameter> m_CommandBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledMultipleRow{TCommand, TParameter}"/> struct.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        public CompiledSingleRow(SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
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


    }
}
