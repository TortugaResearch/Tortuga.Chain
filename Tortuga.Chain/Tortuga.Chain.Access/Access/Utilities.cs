using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

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

	public static List<AbstractParameter> GetParameters(this SqlBuilder<AbstractDbType> sqlBuilder, IDataSource dataSource)
	{
		return sqlBuilder.GetParameters(ParameterBuilderCallback);
	}

	public static List<OleDbParameter> GetParametersKeysLast(this SqlBuilder<OleDbType> sqlBuilder)
	{
		return sqlBuilder.GetParametersKeysLast(ParameterBuilderCallback);
	}

	public static OleDbParameter CreateParameter(ISqlBuilderEntryDetails<OleDbType> details, string parameterName, object? value)
	{
		var result = new OleDbParameter();
		result.ParameterName = parameterName;

		result.Value = value switch
		{
#if NET6_0_OR_GREATER
			DateOnly dateOnly => dateOnly.ToDateTime(default),
			TimeOnly timeOnly => default(DateTime) + timeOnly.ToTimeSpan(),
#endif
			TimeSpan timeSpan => default(DateTime) + timeSpan,
			null => DBNull.Value,
			_ => value
		};

		if (details.DbType.HasValue)
			result.OleDbType = details.DbType.Value;

		return result;
	}

	public static OleDbParameter ParameterBuilderCallback(SqlBuilderEntry<OleDbType> entry)
	{
		return CreateParameter(entry.Details, entry.Details.SqlVariableName, entry.ParameterValue);
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