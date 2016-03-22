using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a database table value function.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class TableFunctionMetadata<TName, TDbType>
        where TDbType : struct
    {

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        public TableFunctionMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters, IList<ColumnMetadata<TDbType>> columns)
        {
            Name = name;
            Columns = new ReadOnlyCollection<ColumnMetadata<TDbType>>(columns);
            Parameters = new ReadOnlyCollection<ParameterMetadata<TDbType>>(parameters);

        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata<TDbType>> Columns { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public ReadOnlyCollection<ParameterMetadata<TDbType>> Parameters { get; }

    }
}
