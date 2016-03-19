using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Tortuga.Chain
{
    /// <summary>
    /// Collection of row objects
    /// </summary>
    public class RowCollection : ReadOnlyCollection<Row>
    {
        public RowCollection(IList<Row> list)
            : base(list)
        {

        }

    }
}
