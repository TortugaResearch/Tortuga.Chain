using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Tortuga.Chain.CommandBuilders;

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
            return sqlBuilder.GetParameters((SqlBuilderEntry<SqlDbType> entry) =>
            {
                var result = new SqlParameter();
                result.ParameterName = entry.Details.SqlVariableName;
                result.Value = entry.ParameterValue;

                if (entry.Details.DbType.HasValue)
                    result.SqlDbType = entry.Details.DbType.Value;

                if (entry.ParameterValue is DbDataReader)
                    result.SqlDbType = SqlDbType.Structured;

                return result;
            });
        }

    }
}
