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
        private readonly ReadOnlyCollection<ColumnMetadata<TDbType>> m_Columns;
        private readonly TName m_Name;
        private readonly ReadOnlyCollection<ParameterMetadata<TDbType>> m_Parameters;

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        public TableFunctionMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters, IList<ColumnMetadata<TDbType>> columns)
        {
            m_Name = name;
            m_Columns = new ReadOnlyCollection<ColumnMetadata<TDbType>>(columns);
            m_Parameters = new ReadOnlyCollection<ParameterMetadata<TDbType>>(parameters);

        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata<TDbType>> Columns
        {
            get { return m_Columns; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name
        {
            get { return m_Name; }
        }
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public ReadOnlyCollection<ParameterMetadata<TDbType>> Parameters
        {
            get { return m_Parameters; }
        }

    }
}
