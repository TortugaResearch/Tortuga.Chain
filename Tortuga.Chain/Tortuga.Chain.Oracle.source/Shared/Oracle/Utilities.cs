using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Oracle
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the parameters from a SQL Builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <returns></returns>
        public static List<OracleParameter> GetParameters(this SqlBuilder<OracleDbType> sqlBuilder)
        {
            return sqlBuilder.GetParameters((SqlBuilderEntry<OracleDbType> entry) =>
            {
                var result = new OracleParameter();
                result.ParameterName = entry.Details.SqlVariableName;
                result.Value = entry.ParameterValue;
                if (entry.Details.DbType.HasValue)
                    result.OracleDbType = entry.Details.DbType.Value;
                return result;
            });
        }

        public static bool PrimaryKeyIsIdentity(this SqlBuilder<OracleDbType> sqlBuilder, out List<OracleParameter> keyParameters)
        {
            return sqlBuilder.PrimaryKeyIsIdentity((OracleDbType? type) =>
            {
                var result = new OracleParameter();
                if (type.HasValue)
                    result.OracleDbType = type.Value;
                return result;
            }, out keyParameters);
        }
    }
}
