using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Access;

internal static class Utilities
{
	public static OleDbParameter Clone(this OleDbParameter original)
	{
		return new OleDbParameter()
		{
			DbType = original.DbType,
			Direction = original.Direction,
			IsNullable = original.IsNullable,
			OleDbType = original.OleDbType,
			ParameterName = original.ParameterName,
			Precision = original.Precision,
			Scale = original.Scale,
			Size = original.Size,
			SourceColumn = original.SourceColumn,
			SourceColumnNullMapping = original.SourceColumnNullMapping,
			SourceVersion = original.SourceVersion,
			Value = original.Value
		};
	}

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

	public static bool RequiresSorting(this AccessLimitOption limitOption)
	{
		return limitOption switch
		{
			AccessLimitOption.None => false,
			AccessLimitOption.RowsWithTies => true,
			_ => throw new ArgumentOutOfRangeException(nameof(limitOption), limitOption, "Unknown limit option")
		};
	}
}