using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

internal static class Utilities
{
	/// <summary>
	/// Gets the parameters from a SQL Builder.
	/// </summary>
	/// <param name="sqlBuilder">The SQL builder.</param>
	/// <returns></returns>
	public static List<SqlParameter> GetParameters(this SqlBuilder<SqlDbType> sqlBuilder)
	{
		return sqlBuilder.GetParameters(ParameterBuilderCallback);
	}

	public static SqlParameter ParameterBuilderCallback(SqlBuilderEntry<SqlDbType> entry)
	{
		var result = new SqlParameter();
		result.ParameterName = entry.Details.SqlVariableName;

#if NET6_0_OR_GREATER
		result.Value = entry.ParameterValue switch
		{
			DateOnly dateOnly => dateOnly.ToDateTime(default),
			TimeOnly timeOnly => timeOnly.ToTimeSpan(),
			_ => entry.ParameterValue
		};
#else
		result.Value = entry.ParameterValue;
#endif

		if (entry.Details.DbType.HasValue)
		{
			result.SqlDbType = entry.Details.DbType.Value;

			if (entry.Details.MaxLength.HasValue)
			{
				switch (result.SqlDbType)
				{
					case SqlDbType.Char:
					case SqlDbType.VarChar:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
						result.Size = entry.Details.MaxLength.Value;
						break;
				}
			}
		}

		if (entry.ParameterValue is DbDataReader)
			result.SqlDbType = SqlDbType.Structured;

		result.Direction = entry.Details.Direction;

		return result;
	}

	/// <summary>
	/// Triggers need special handling for OUTPUT clauses.
	/// </summary>
	public static void UseTableVariable<TDbType>(this SqlBuilder<TDbType> sqlBuilder, SqlServerTableOrViewMetadata<TDbType> table, out string? header, out string? intoClause, out string? footer)
		where TDbType : struct
	{
		if (sqlBuilder.HasReadFields && table.HasTriggers)
		{
			header = "DECLARE @ResultTable TABLE( " + string.Join(", ", sqlBuilder.GetSelectColumnDetails().Select(c => c.QuotedSqlName + " " + c.FullTypeName + " NULL")) + ");" + Environment.NewLine;
			intoClause = " INTO @ResultTable ";
			footer = Environment.NewLine + "SELECT * FROM @ResultTable";
		}
		else
		{
			header = null;
			intoClause = null;
			footer = null;
		}
	}
}