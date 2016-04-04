using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Metadata;

#if SDS
using System.Data.SQLite;
#else
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the parameters from a SQL Builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <returns></returns>
        public static List<SQLiteParameter> GetParameters(this SqlBuilder<DbType> sqlBuilder)
        {
            return sqlBuilder.GetParameters((DbType? type) =>
            {
                var result = new SQLiteParameter();
                if (type.HasValue)
                    result.DbType = type.Value;
                return result;
            });
        }
    }
}
