

using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.SQLite
{

    /// <summary>
    /// This class is used to modify settings that are not represented by the connection string. 
    /// </summary>
    /// <seealso cref="DataSourceSettings" />
    public class SQLiteDataSourceSettings : DataSourceSettings
    {
        /// <summary>
        /// Normally we use a reader/writer lock to avoid simutaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexcepted exceptions.
        /// </summary>
        public bool? DisableLocks { get; set; }
    }
}
