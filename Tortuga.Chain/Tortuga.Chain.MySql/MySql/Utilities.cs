using MySqlConnector;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql;

internal static class Utilities
{
	/// <summary>
	/// Gets the parameters from a SQL Builder.
	/// </summary>
	/// <param name="sqlBuilder">The SQL builder.</param>
	/// <returns></returns>
	public static List<MySqlParameter> GetParameters(this SqlBuilder<MySqlDbType> sqlBuilder)
	{
		return sqlBuilder.GetParameters(ParameterBuilderCallback);
	}

	public static MySqlParameter CreateParameter(ISqlBuilderEntryDetails<MySqlDbType> details, string parameterName, object? value)
	{
		var result = new MySqlParameter();
		result.ParameterName = parameterName;
		result.Value = value switch
		{
			null => DBNull.Value,
			_ => value
		};
		if (details.DbType.HasValue)
			result.MySqlDbType = details.DbType.Value;
		return result;
	}

	public static MySqlParameter ParameterBuilderCallback(SqlBuilderEntry<MySqlDbType> entry)
	{
		return CreateParameter(entry.Details, entry.Details.SqlVariableName, entry.ParameterValue);
	}

	public static bool PrimaryKeyIsIdentity(this SqlBuilder<MySqlDbType> sqlBuilder, out List<MySqlParameter> keyParameters)
	{
		return sqlBuilder.PrimaryKeyIsIdentity((MySqlDbType? type) =>
		{
			var result = new MySqlParameter();
			if (type.HasValue)
				result.MySqlDbType = type.Value;
			return result;
		}, out keyParameters);
	}

	public static bool RequiresSorting(this MySqlLimitOption limitOption)
	{
		return limitOption switch
		{
			MySqlLimitOption.None => false,
			MySqlLimitOption.Rows => true,
			MySqlLimitOption.RandomSampleRows => false,
			_ => throw new ArgumentOutOfRangeException(nameof(limitOption), limitOption, "Unknown limit option")
		};
	}
}