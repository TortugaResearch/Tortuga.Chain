using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TName">The type of the name.</typeparam>
    public class IndexMetadataCollection<TName> : ReadOnlyCollection<IndexMetadata<TName>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexMetadataCollection{TName}"/> class.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        public IndexMetadataCollection(IList<IndexMetadata<TName>> list) : base(list)
        {
        }
    }
}
