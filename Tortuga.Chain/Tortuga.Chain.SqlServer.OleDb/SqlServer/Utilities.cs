using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

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
				result.OleDbType = entry.Details.DbType.Value;

				if (entry.Details.TypeName == "datetime2" && entry.Details.Scale.HasValue)
					result.Scale = (byte)entry.Details.Scale.Value;
				else if (entry.Details.TypeName == "time" && entry.Details.Scale > 0)
				{
					//If we need fractions of a second, force a type change.
					//OleDbType.DBTime does not support milliseconds.
					//OleDbType.DBTimeStamp only accepts DateTime.

					result.OleDbType = OleDbType.DBTimeStamp;
					result.Scale = (byte)entry.Details.Scale.Value;

					if (result.Value is TimeSpan tv)
					{
						result.Value = default(DateTime) + tv;
					}
				}
			}
			return result;
		});
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