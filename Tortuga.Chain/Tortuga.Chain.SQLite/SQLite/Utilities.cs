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

			result.Value = entry.ParameterValue switch
			{
#if NET6_0_OR_GREATER
			DateOnly dateOnly => dateOnly.ToDateTime(default),
			TimeOnly timeOnly => default(DateTime) + timeOnly.ToTimeSpan(),
#endif
				TimeSpan timeSpan => default(DateTime) + timeSpan,
				_ => entry.ParameterValue
			};

			if (entry.Details.DbType.HasValue)
				result.DbType = entry.Details.DbType.Value;

			result.Direction = entry.Details.Direction;

			return result;
		}

		public static bool RequiresSorting(this SQLiteLimitOption limitOption)
		{
			return limitOption switch
			{
				SQLiteLimitOption.None => false,
				SQLiteLimitOption.Rows => true,
				SQLiteLimitOption.RandomSampleRows => false,
				_ => throw new ArgumentOutOfRangeException(nameof(limitOption), limitOption, "Unknown limit option")
			};
		}
	}
}