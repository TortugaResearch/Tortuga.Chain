using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata;

/// <summary>Class IndexMetadataCollection.
///</summary>
/// <typeparam name="TObjectName">The type of the name.</typeparam>
/// <typeparam name="TDbType">The database column type.</typeparam>
public class IndexMetadataCollection<TObjectName, TDbType> : ReadOnlyCollection<IndexMetadata<TObjectName, TDbType>>
	where TObjectName : struct
	where TDbType : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="IndexMetadataCollection{TObjectName,TDbType}"/> class.
	/// </summary>
	/// <param name="list">The list to wrap.</param>
	public IndexMetadataCollection(IList<IndexMetadata<TObjectName, TDbType>> list) : base(list)
	{
	}
}
