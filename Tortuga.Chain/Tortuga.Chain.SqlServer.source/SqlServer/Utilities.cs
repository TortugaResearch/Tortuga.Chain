using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the parameters from a SQL Builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <returns></returns>
        public static List<SqlParameter> GetParameters(this SqlBuilder<SqlDbType> sqlBuilder)
        {
            return sqlBuilder.GetParameters((SqlDbType? type) =>
            {
                var result = new SqlParameter();
                if (type.HasValue)
                    result.SqlDbType = type.Value;
                return result;
            });
        }
    }
}
