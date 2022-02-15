using System.Collections.ObjectModel;

namespace Tortuga.Chain
{
	/// <summary>
	/// Collection of row objects
	/// </summary>
	public sealed class RowCollection : ReadOnlyCollection<Row>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RowCollection"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		public RowCollection(IList<Row> list)
			: base(list)
		{
		}
	}
}
