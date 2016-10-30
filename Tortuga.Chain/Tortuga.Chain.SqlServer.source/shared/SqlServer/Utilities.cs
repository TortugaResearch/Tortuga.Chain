using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Tortuga.Chain.CommandBuilders;

#if !OleDb_Missing
using System.Data.OleDb;
#endif

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

#if !OleDb_Missing
        /// <summary>
        /// Gets the parameters from a SQL Builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <returns></returns>
        public static List<OleDbParameter> GetParameters(this SqlBuilder<OleDbType> sqlBuilder)
        {
            return sqlBuilder.GetParameters((SqlBuilderEntry<OleDbType> entry) =>
            {
                var result = new OleDbParameter();
                result.ParameterName = entry.Details.SqlVariableName;
                result.Value = entry.ParameterValue;

                if (entry.Details.DbType.HasValue)
                {
                    result.OleDbType = entry.Details.DbType.Value;

                    if (entry.Details.TypeName == "datetime2" && entry.Details.Scale.HasValue)
                        result.Scale = (byte)entry.Details.Scale.Value;
                }
                return result;
            });
        }
#endif 


    }
}
