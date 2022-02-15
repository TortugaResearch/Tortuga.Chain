using MySqlConnector;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.MySql
{
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

		public static MySqlParameter ParameterBuilderCallback(SqlBuilderEntry<MySqlDbType> entry)
		{
			var result = new MySqlParameter();
			result.ParameterName = entry.Details.SqlVariableName;
			result.Value = entry.ParameterValue;
			if (entry.Details.DbType.HasValue)
				result.MySqlDbType = entry.Details.DbType.Value;
			return result;
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
	}
}
