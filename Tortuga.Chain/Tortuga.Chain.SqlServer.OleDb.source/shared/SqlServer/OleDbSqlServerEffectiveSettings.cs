using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using System.Data.OleDb;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// This contains the connection options that are currently in effect.
    /// </summary>
    internal class OleDbSqlServerEffectiveSettings : SqlServerEffectiveSettings
    {
        internal OleDbSqlServerEffectiveSettings()
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal void Reload(OleDbConnection connection, OleDbTransaction transaction)
        {
            using (var cmd = new OleDbCommand("SELECT @@Options") { Connection = connection, Transaction = transaction })
                m_Options = (int)cmd.ExecuteScalar();
        }

        internal async Task ReloadAsync(OleDbConnection connection, OleDbTransaction transaction)
        {
            using (var cmd = new OleDbCommand("SELECT @@Options") { Connection = connection, Transaction = transaction })
                m_Options = (int)(await cmd.ExecuteScalarAsync());
        }
    }
}