using System.Data.SQLite;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SQLite
{
	internal static class Utilities
	{
		public static SQLiteParameter CreateParameter(ISqlBuilderEntryDetails<DbType> details, string parameterName, object? value)
		{
			var result = new SQLiteParameter();
			result.ParameterName = parameterName;

			result.Value = value switch
			{
				DateOnly dateOnly => dateOnly.ToDateTime(default),
				TimeOnly timeOnly => default(DateTime) + timeOnly.ToTimeSpan(),
				TimeSpan timeSpan => default(DateTime) + timeSpan,
				null => DBNull.Value,
				_ => value
			};

			if (details.DbType.HasValue)
				result.DbType = details.DbType.Value;

			result.Direction = details.Direction;

			return result;
		}

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
			return CreateParameter(entry.Details, entry.Details.SqlVariableName, entry.ParameterValue);
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
