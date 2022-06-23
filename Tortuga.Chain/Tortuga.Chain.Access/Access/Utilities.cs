using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

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

	public static OleDbParameter ParameterBuilderCallback(SqlBuilderEntry<OleDbType> entry)
	{
		var result = new OleDbParameter();
		result.ParameterName = entry.Details.SqlVariableName;

#if NET6_0_OR_GREATER
		result.Value = entry.ParameterValue switch
		{
			DateOnly dateOnly => dateOnly.ToDateTime(default),
			TimeOnly timeOnly => default(DateTime) + timeOnly.ToTimeSpan(),
			_ => entry.ParameterValue
		};
#else
		result.Value = entry.ParameterValue;
#endif

		if (entry.Details.DbType.HasValue)
			result.OleDbType = entry.Details.DbType.Value;

		return result;
	}
}