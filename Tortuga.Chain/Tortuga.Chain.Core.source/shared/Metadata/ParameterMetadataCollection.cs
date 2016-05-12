using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class ParameterMetadataCollection.
    /// </summary>
    public class ParameterMetadataCollection : ReadOnlyCollection<ParameterMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadataCollection"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ParameterMetadataCollection(IEnumerable<ParameterMetadata> source) : base(source.ToList())
        {

        }

        /// <summary>
        /// Returns the parameter associated with the parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <remarks>If the parameter name was not found, this will return null</remarks>
        public ParameterMetadata TryGetParameter(string parameterName)
        {
            foreach (var item in this)
                if (item.SqlParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    return item;

            return null;
        }
    }
}
