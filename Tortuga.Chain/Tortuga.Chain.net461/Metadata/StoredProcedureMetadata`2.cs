using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class StoredProcedureMetadata.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class StoredProcedureMetadata<TName, TDbType>
        where TDbType : struct
    {
        private readonly TName m_Name;
        private readonly ReadOnlyCollection<ParameterMetadata<TDbType>> m_Parameters;

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        public StoredProcedureMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters)
        {
            m_Name = name;
            m_Parameters = new ReadOnlyCollection<ParameterMetadata<TDbType>>(parameters);
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
