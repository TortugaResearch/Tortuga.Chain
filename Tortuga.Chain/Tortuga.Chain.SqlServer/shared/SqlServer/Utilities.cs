using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

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

	public static SqlParameter CreateParameter(ISqlBuilderEntryDetails<SqlDbType> details, string parameterName, object? value)
	{
		var result = new SqlParameter();
		result.ParameterName = parameterName;

		result.Value = value switch
		{
#if NET6_0_OR_GREATER
			DateOnly dateOnly => dateOnly.ToDateTime(default),
			TimeOnly timeOnly => timeOnly.ToTimeSpan(),
#endif
			null => DBNull.Value,
			_ => value ?? DBNull.Value
		};

		if (details.DbType.HasValue)
		{
			result.SqlDbType = details.DbType.Value;

			if (details.MaxLength.HasValue)
			{
				switch (result.SqlDbType)
				{
					case SqlDbType.Char:
					case SqlDbType.VarChar:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
						result.Size = details.MaxLength.Value;
						break;
				}
			}
		}

		if (value is DbDataReader)
			result.SqlDbType = SqlDbType.Structured;

		result.Direction = details.Direction;

		return result;
	}

	public static SqlParameter ParameterBuilderCallback(SqlBuilderEntry<SqlDbType> entry)
	{
		return CreateParameter(entry.Details, entry.Details.SqlVariableName, entry.ParameterValue);
	}

	public static bool RequiresSorting(this SqlServerLimitOption limitOption)
	{
		return limitOption switch
		{
			SqlServerLimitOption.None => false,
			SqlServerLimitOption.Rows => true,
			SqlServerLimitOption.Percentage => true,
			SqlServerLimitOption.RowsWithTies => true,
			SqlServerLimitOption.PercentageWithTies => true,
			SqlServerLimitOption.TableSampleSystemRows => false,
			SqlServerLimitOption.TableSampleSystemPercentage => false,
			_ => throw new ArgumentOutOfRangeException(nameof(limitOption), limitOption, "Unknown limit option")
		};
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