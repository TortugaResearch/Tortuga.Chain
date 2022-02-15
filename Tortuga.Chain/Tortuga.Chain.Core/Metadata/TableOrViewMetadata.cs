using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class DatabaseObject.
	/// </summary>
	public abstract class DatabaseObject
	{
		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.DatabaseObject"/> class.</summary>
		protected DatabaseObject(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

			Name = name;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; }
	}

	/// <summary>
	/// Abstract version of TableOrViewMetadata.
	/// </summary>
	public abstract class TableOrViewMetadata : DatabaseObject
	{
		/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.Metadata.TableOrViewMetadata"/> class.</summary>
		protected TableOrViewMetadata(string name, bool isTable, ColumnMetadataCollection columns) : base(name)
		{
			IsTable = isTable;
			Columns = columns ?? throw new ArgumentNullException(nameof(columns), $"{nameof(columns)} is null.");
			NonNullableColumns = new ColumnMetadataCollection(name, columns.Where(c => c.IsNullable == false).ToList());
			PrimaryKeyColumns = new ColumnMetadataCollection(name, columns.Where(c => c.IsPrimaryKey).ToList());
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>
		/// The columns.
		/// </value>
		public ColumnMetadataCollection Columns { get; }

		/// <summary>
		/// Gets a value indicating whether this table or view has primary key.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance has a primary key; otherwise, <c>false</c>.
		/// </value>
		public bool HasPrimaryKey => Columns.Any(c => c.IsPrimaryKey);

		/// <summary>
		/// Gets a value indicating whether this instance is table or a view.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is a table; otherwise, <c>false</c>.
		/// </value>
		public bool IsTable { get; }

		/// <summary>
		/// Gets the columns known to be not nullable.
		/// </summary>
		/// <value>
		/// The nullable columns.
		/// </value>
		/// <remarks>This is used to improve the performance of materializers by avoiding is null checks.</remarks>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ColumnMetadataCollection NonNullableColumns { get; }

		/// <summary>
		/// Gets the columns that make up the primary key.
		/// </summary>
		/// <value>
		/// The columns.
		/// </value>
		public ColumnMetadataCollection PrimaryKeyColumns { get; }
	}
}
