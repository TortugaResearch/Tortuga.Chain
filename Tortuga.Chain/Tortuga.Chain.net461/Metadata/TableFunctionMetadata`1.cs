using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a database table value function.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    public class TableFunctionMetadata<TName>
    {
        private readonly ReadOnlyCollection<ColumnMetadata> m_Columns;
        private readonly TName m_Name;
        private readonly ReadOnlyCollection<ParameterMetadata> m_Parameters;

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        public TableFunctionMetadata(TName name, IList<ParameterMetadata> parameters, IList<ColumnMetadata> columns)
        {
            m_Name = name;
            m_Columns = new ReadOnlyCollection<ColumnMetadata>(columns);
            m_Parameters = new ReadOnlyCollection<ParameterMetadata>(parameters);

        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata> Columns
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
        public ReadOnlyCollection<ParameterMetadata> Parameters
        {
            get { return m_Parameters; }
        }

    }
}
