using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Chain.Metadata
{
        


    /// <summary>
    /// Class ParameterMetadataCollection.
    /// </summary>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    public class ParameterMetadataCollection<TDbType> : ReadOnlyCollection<ParameterMetadata<TDbType>>
        where TDbType : struct
    {
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadataCollection{TDbType}" /> class.
        /// </summary>
        /// <param name="name">The name of the parent object.</param>
        /// <param name="list">The list to wrap.</param>
        public ParameterMetadataCollection(string name, IList<ParameterMetadata<TDbType>> list) : base(list)
        {
            m_Name = name;
            GenericCollection = new ParameterMetadataCollection(list);
        }

        /// <summary>
        /// Gets the generic version of this collection.
        /// </summary>
        /// <value>The generic collection.</value>
        /// <remarks>This is used in generic repository scenarios</remarks>
        public ParameterMetadataCollection GenericCollection { get; }

        /// <summary>
        /// Gets the <see cref="ParameterMetadata{TDbType}" /> with the specified name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>ParameterMetadata&lt;TDbType&gt;.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public ParameterMetadata<TDbType> this[string parameterName]
        {
            get
            {
                foreach (var item in this)
                    if (item.SqlParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                        return item;

                throw new KeyNotFoundException($"Could not find parameter named {parameterName} in object {m_Name}");
            }
        }





        /// <summary>
        /// Returns the parameter associated with the parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <remarks>If the parameter name was not found, this will return null</remarks>
        public ParameterMetadata<TDbType> TryGetParameter(string parameterName)
        {
            foreach (var item in this)
                if (item.SqlParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    return item;

            return null;
        }


    }
}
