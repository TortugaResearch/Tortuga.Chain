using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
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

        ///// <summary>
        ///// Triggers need special handling for OUTPUT clauses.
        ///// </summary>
        //public static void UseTableVariable<TDbType>(this SqlBuilder<TDbType> sqlBuilder, SqlServerTableOrViewMetadata<TDbType> Table, out string header, out string intoClause, out string footer) where TDbType : struct
        //{
        //    if (sqlBuilder.HasReadFields && Table.HasTriggers)
        //    {
        //        header = "DECLARE @ResultTable TABLE( " + string.Join(", ", sqlBuilder.GetSelectColumnDetails().Select(c => c.QuotedSqlName + " " + c.FullTypeName + " NULL")) + ");" + Environment.NewLine;
        //        intoClause = " INTO @ResultTable ";
        //        footer = Environment.NewLine + "SELECT * FROM @ResultTable";
        //    }
        //    else
        //    {
        //        header = null;
        //        intoClause = null;
        //        footer = null;
        //    }
        //}
    }
}