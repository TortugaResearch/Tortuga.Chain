using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlIndexMetadataCollection.
    /// </summary>
    public class PostgreSqlIndexMetadataCollection : ReadOnlyCollection<PostgreSqlIndexMetadata>
    {
        /// <summary>Initializes a new instance of the PostgreSqlIndexMetadataCollection class that is a read-only wrapper around the specified list.</summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.</exception>
        public PostgreSqlIndexMetadataCollection(IList<PostgreSqlIndexMetadata> list) : base(list)
        {
        }
    }
}
