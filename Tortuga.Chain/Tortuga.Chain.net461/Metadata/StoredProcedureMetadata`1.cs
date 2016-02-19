using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    public class StoredProcedureMetadata<TName>
    {
        private readonly TName m_Name;
        private readonly ReadOnlyCollection<ParameterMetadata> m_Parameters;

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        public StoredProcedureMetadata(TName name, IList<ParameterMetadata> parameters)
        {
            m_Name = name;
            m_Parameters = new ReadOnlyCollection<ParameterMetadata>(parameters);
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
