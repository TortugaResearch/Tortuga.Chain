using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Class ForeignKeyConstraintCollection.
	/// </summary>
	/// <typeparam name="TName">The type of the name.</typeparam>
	/// <typeparam name="TDbType">The type of the database type.</typeparam>
	public class ForeignKeyConstraintCollection<TName, TDbType> : ReadOnlyCollection<ForeignKeyConstraint<TName, TDbType>>
		where TName : struct
		where TDbType : struct
	{
		/// <summary>Initializes a new instance of the class that is a read-only wrapper around the specified list.</summary>
		/// <param name="list">The list to wrap.</param>
		public ForeignKeyConstraintCollection(IList<ForeignKeyConstraint<TName, TDbType>> list) : base(list)
		{
			GenericCollection = new ForeignKeyConstraintCollection(this);
		}

		/// <summary>
		/// Gets the generic version of this collection.
		/// </summary>
		public ForeignKeyConstraintCollection GenericCollection { get; }
	}
}
