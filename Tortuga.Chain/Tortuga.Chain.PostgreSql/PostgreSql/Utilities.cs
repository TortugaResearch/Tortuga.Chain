using Npgsql;
using NpgsqlTypes;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql;

internal static class Utilities
{
	/// <summary>
	/// Gets the parameters from a SQL Builder.
	/// </summary>
	/// <param name="sqlBuilder">The SQL builder.</param>
	/// <returns></returns>
	public static List<NpgsqlParameter> GetParameters(this SqlBuilder<NpgsqlDbType> sqlBuilder)
	{
		return sqlBuilder.GetParameters(ParameterBuilderCallback);
	}

	public static NpgsqlParameter CreateParameter(ISqlBuilderEntryDetails<NpgsqlDbType> details, string parameterName, object? value)
	{
		var result = new NpgsqlParameter();
		result.ParameterName = parameterName;
		result.Value = value switch
		{
			null => DBNull.Value,
			_ => value
		};
		if (details.DbType.HasValue)
			result.NpgsqlDbType = details.DbType.Value;
		return result;
	}

	public static NpgsqlParameter ParameterBuilderCallback(SqlBuilderEntry<NpgsqlDbType> entry)
	{
		return CreateParameter(entry.Details, entry.Details.SqlVariableName, entry.ParameterValue);
	}

	public static bool PrimaryKeyIsIdentity(this SqlBuilder<NpgsqlDbType> sqlBuilder, out List<NpgsqlParameter> keyParameters)
	{
		return sqlBuilder.PrimaryKeyIsIdentity((NpgsqlDbType? type) =>
		{
			var result = new NpgsqlParameter();
			if (type.HasValue)
				result.NpgsqlDbType = type.Value;
			return result;
		}, out keyParameters);
	}

	public static bool RequiresSorting(this PostgreSqlLimitOption limitOption)
	{
		return limitOption switch
		{
			PostgreSqlLimitOption.None => false,
			PostgreSqlLimitOption.Rows => true,
			PostgreSqlLimitOption.TableSampleSystemPercentage => false,
			PostgreSqlLimitOption.TableSampleBernoulliPercentage => false,
			_ => throw new ArgumentOutOfRangeException(nameof(limitOption), limitOption, "Unknown limit option")
		};
	}
}
