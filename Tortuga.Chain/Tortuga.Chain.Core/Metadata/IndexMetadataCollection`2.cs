using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
	/// <summary>Class IndexMetadataCollection.
	///</summary>
	/// <typeparam name="TName">The type of the name.</typeparam>
	/// <typeparam name="TDbType">The database column type.</typeparam>
	public class IndexMetadataCollection<TName, TDbType> : ReadOnlyCollection<IndexMetadata<TName, TDbType>>
		where TName : struct
		where TDbType : struct
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IndexMetadataCollection{TName,TDbType}"/> class.
		/// </summary>
		/// <param name="list">The list to wrap.</param>
		public IndexMetadataCollection(IList<IndexMetadata<TName, TDbType>> list) : base(list)
		{
		}
	}
}
