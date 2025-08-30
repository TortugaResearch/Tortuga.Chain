namespace Tortuga.Chain.Materializers;
static class Converters
{
	public static DateOnly ToDateOnly(object value)
	{
		return value switch
		{
			DateOnly dateOnly => dateOnly,
			DateTime dateTime => DateOnly.FromDateTime(dateTime),
			DateTimeOffset dateTimeOffset => DateOnly.FromDateTime(dateTimeOffset.DateTime),
			_ => (DateOnly)value,
		};
	}

	public static TimeOnly ToTimeOnly(object value)
	{
		return value switch
		{
			TimeOnly timeOnly => timeOnly,
			TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
			DateTime dateTime => TimeOnly.FromDateTime(dateTime),
			DateTimeOffset dateTimeOffset => TimeOnly.FromDateTime(dateTimeOffset.DateTime),
			_ => (TimeOnly)value,
		};
	}
}
