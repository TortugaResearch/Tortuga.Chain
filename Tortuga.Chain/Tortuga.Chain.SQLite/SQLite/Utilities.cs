using System.Data.SQLite;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
	internal static class Utilities
	{
		public static List<AbstractParameter> GetParameters(this SqlBuilder<AbstractDbType> sqlBuilder)
		{
			return sqlBuilder.GetParameters(ParameterBuilderCallback);
		}

		/// <summary>
		/// Callback for parameter builder.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <returns>SqlDbType.</returns>
		public static SQLiteParameter ParameterBuilderCallback(SqlBuilderEntry<DbType> entry)
		{
			var result = new SQLiteParameter();
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
				result.DbType = entry.Details.DbType.Value;

			result.Direction = entry.Details.Direction;

			return result;
		}
	}
}