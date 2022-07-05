using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Enum IndexType
/// </summary>
public enum IndexType
{
	/// <summary>
	/// Unknown index type.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// B-Tree Index
	/// </summary>
	BTree = 1,

	/// <summary>
	/// Hash
	/// </summary>
	Hash = 2,

	/// <summary>
	/// Generalized Inverted Search Tree (GiST)
	/// </summary>
	Gist = 3,

	/// <summary>
	/// Generalized Inverted Index (GIN)
	/// </summary>
	Gin = 4,

	/// <summary>
	/// Space partitioned GiST (SP-GiST)
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spgist")]
	Spgist = 5,

	/// <summary>
	/// Block Range Indexes (BRIN)
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Brin")]
	Brin = 6,

	/// <summary>
	/// R-Tree Index
	/// </summary>
	RTree = 7,

	/// <summary>
	/// Full Text Index
	/// </summary>
	FullText = 8,

	/// <summary>
	/// Heap
	/// </summary>
	Heap = 9,

	/// <summary>
	/// Clustered
	/// </summary>
	Clustered = 10,

	/// <summary>
	/// Nonclustered
	/// </summary>
	Nonclustered = 11,

	/// <summary>
	/// XML
	/// </summary>
	Xml = 12,

	/// <summary>
	/// Spatial
	/// </summary>
	Spatial = 13,

	/// <summary>
	/// Clustered columnstore index
	/// </summary>
	ClusteredColumnstoreIndex = 14,

	/// <summary>
	/// Nonclustered columnstore index
	/// </summary>
	NonclusteredColumnstoreIndex = 15,

	/// <summary>
	/// Nonclustered hash index
	/// </summary>
	NonclusteredHashIndex = 16
}
