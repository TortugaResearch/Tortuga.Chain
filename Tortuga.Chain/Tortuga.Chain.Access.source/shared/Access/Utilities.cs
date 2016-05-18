using System.Collections.Generic;
using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Access
{
    internal static class Utilities
    {
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
                    result.OleDbType = entry.Details.DbType.Value;

                return result;
            });
        }

        public static List<OleDbParameter> GetParametersKeysLast(this SqlBuilder<OleDbType> sqlBuilder)
        {
            return sqlBuilder.GetParametersKeysLast((SqlBuilderEntry<OleDbType> entry) =>
            {
                var result = new OleDbParameter();
                result.ParameterName = entry.Details.SqlVariableName;
                result.Value = entry.ParameterValue;

                if (entry.Details.DbType.HasValue)
                    result.OleDbType = entry.Details.DbType.Value;

                return result;
            });
        }
    }
}
