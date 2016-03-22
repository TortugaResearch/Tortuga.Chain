using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Tortuga.Chain
{
    /// <summary>
    /// Collection of row objects
    /// </summary>
    public class RowCollection : ReadOnlyCollection<Row>
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
