using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerIndexMetadataCollection.
    /// </summary>
    public class SqlServerIndexMetadataCollection : ReadOnlyCollection<SqlServerIndexMetadata>
    {
        /// <summary>Initializes a new instance of the SqlServerIndexMetadataCollection class that is a read-only wrapper around the specified list.</summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.</exception>
        public SqlServerIndexMetadataCollection(IList<SqlServerIndexMetadata> list) : base(list)
        {
        }
    }
}
