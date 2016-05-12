using System.Data.Common;

namespace Tortuga.Chain.CommandBuilders
{

    /// <summary>
    /// Callback for the parameter builder.
    /// </summary>
    /// <typeparam name="TParameter">The type of the desired DbParameter.</typeparam>
    /// <typeparam name="TDbType">The database specific DbType</typeparam>
    /// <param name="entry">Metadata about the parameter in question.</param>
    /// <returns>TParameter.</returns>
    /// <remarks>For internal use only.</remarks>
    public delegate TParameter ParameterBuilderCallback<TParameter, TDbType>(SqlBuilderEntry<TDbType> entry)
        where TDbType : struct
        where TParameter : DbParameter;
}
