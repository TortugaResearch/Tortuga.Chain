using System.Data.Common;
using System.Globalization;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Helper functions for building SQL statements.
/// </summary>
public static class SqlBuilder
{
	/// <summary>
	/// Checks to see of the same property appears in both objects. If it does, an InvalidOperationException is thrown with the provided error message.
	/// </summary>
	/// <param name="firstObject">The first object.</param>
	/// <param name="secondObject">The second object.</param>
	/// <param name="errorFormat">The error format. Slot 0 is the matching property.</param>
	/// <returns>System.String.</returns>
	/// <remarks>If either object is null, this check is skipped.</remarks>
	public static void CheckForOverlaps(object? firstObject, object? secondObject, string errorFormat)
	{
		if (firstObject == null)
			return;
		if (secondObject == null)
			return;

		var leftList = MetadataCache.GetMetadata(firstObject.GetType()).Properties;
		var rightList = MetadataCache.GetMetadata(secondObject.GetType()).Properties;
		foreach (var property1 in leftList)
			foreach (var property2 in rightList)
				if (property1.Name.Equals(property2.Name, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, errorFormat, property1.Name));
	}

	/// <summary>
	/// Gets parameters from an argument value.
	/// </summary>
	/// <typeparam name="TParameter">The type of the parameter.</typeparam>
	/// <param name="argumentValue">The argument value. This could be a DbParameter, collection of DbParameters, dictionary, or object.</param>	/// <returns></returns>
	public static List<TParameter> GetParameters<TParameter>(object? argumentValue)
	where TParameter : DbParameter, new()
	{
		return GetParameters(argumentValue, (n, v) => new TParameter() { ParameterName = n, Value = v ?? DBNull.Value });
	}

	/// <summary>
	/// Gets parameters from an argument value.
	/// </summary>
	/// <typeparam name="TParameter">The type of the parameter.</typeparam>
	/// <param name="argumentValue">The argument value. This could be a DbParameter, collection of DbParameters, dictionary, or object.</param>
	/// <param name="parameterBuilder">The parameter builder. This should set the parameter's database specific DbType property.</param>
	/// <returns></returns>
	public static List<TParameter> GetParameters<TParameter>(object? argumentValue, Func<string, object?, TParameter> parameterBuilder)
		where TParameter : DbParameter
	{
		if (parameterBuilder == null)
			throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} is null.");

		var result = new List<TParameter>();

		if (argumentValue is TParameter parameter)
			result.Add(parameter);
		else if (argumentValue is IEnumerable<TParameter> parameters)
			foreach (var param in parameters)
				result.Add(param);
		else if (argumentValue is IReadOnlyDictionary<string, object> readOnlyDictionary)
			foreach (var item in readOnlyDictionary)
			{
				var name = item.Key.StartsWith("@", StringComparison.OrdinalIgnoreCase) ? item.Key : "@" + item.Key;
				var param = parameterBuilder(name, item.Value);
				result.Add(param);
			}
		else if (argumentValue != null)
			foreach (var property in MetadataCache.GetMetadata(argumentValue.GetType()).Properties.Where(x => x.MappedColumnName != null))
			{
				var name = (property.MappedColumnName!.StartsWith("@", StringComparison.OrdinalIgnoreCase))
					? property.MappedColumnName
					: $"@{property.MappedColumnName}";
				var value = property.InvokeGet(argumentValue);
				var param = parameterBuilder(name, value);
				result.Add(param);
			}

		return result;
	}
}
