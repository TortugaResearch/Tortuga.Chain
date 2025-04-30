using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// MaterializerTypeConverterDictionary is used when a type conversion is needed while materializing an object.
/// </summary>
public class MaterializerTypeConverter
{
	private readonly ConcurrentDictionary<(Type input, Type output), Func<object, object>> m_ConverterDictionary = new();
	bool m_HasCustomerConverters;

	/// <summary>
	/// Initializes an instance of the <see cref="MaterializerTypeConverter"/> class with the default set of converters.
	/// </summary>
	public MaterializerTypeConverter()
	{
	}

	/// <summary>
	/// Adds the converter to by used by materializers.
	/// </summary>
	/// <typeparam name="TIn">The original type of the value</typeparam>
	/// <typeparam name="TOut">The desired type of the value.</typeparam>
	/// <param name="converter">The converter function.</param>
	/// <exception cref="ArgumentNullException">converter</exception>
	public void AddConverter<TIn, TOut>(Func<TIn, TOut> converter)
	{
		if (converter == null)
			throw new ArgumentNullException(nameof(converter), $"{nameof(converter)} is null.");

		var typeKey = (typeof(TIn), typeof(TOut));
		m_ConverterDictionary[typeKey] = (input) => converter((TIn)input)!;

		m_HasCustomerConverters = true;
		//Future versions may capture the Func<TIn, TOut> version of the converter as well.
		//For now we're just using this pattern to ensure that TIn/TOut match the real effect of the Func<object, object> converter function.
	}

	/// <summary>
	/// Tries the convert the value into the indicated type.
	/// </summary>
	/// <param name="targetType">Desired type</param>
	/// <param name="value">The value to be converted.</param>
	/// <param name="conversionException">If an exception occurs when converting, it will be returned here.</param>
	/// <returns><c>true</c> if the conversion was successful, <c>false</c> otherwise.</returns>
	public bool TryConvertType(Type targetType, ref object? value, out Exception? conversionException)
	{
		if (targetType == null)
			throw new ArgumentNullException(nameof(targetType), $"{nameof(targetType)} is null.");

		conversionException = null;

		if (value == null)
			return true;

		if (value is DBNull)
		{
			value = null;
			return true;
		}

		var targetTypeInfo = targetType.GetTypeInfo();

		//For Nullable<T>, we only care about the type parameter
		if (targetType.Name == "Nullable`1" && targetTypeInfo.IsGenericType)
		{
			//isNullable = true;
			targetType = targetType.GenericTypeArguments[0];
			targetTypeInfo = targetType.GetTypeInfo();
		}

		try
		{
			//Check the converter map
			if (m_HasCustomerConverters && m_ConverterDictionary.TryGetValue((value.GetType(), targetType), out var converter))
			{
				value = converter(value);
				return true;
			}

			//some database return strings when we want strong types
			if (value is string stringValue)
			{
				if (targetType == typeof(char))
				{
					value = stringValue.Length >= 1 ? stringValue[0] : default;
					return true;
				}
				else if (targetType == typeof(XElement))
				{
					value = XElement.Parse(stringValue);
					return true;
				}
				else if (targetType == typeof(XDocument))
				{
					value = XDocument.Parse(stringValue);
					return true;
				}
				else if (targetTypeInfo.IsEnum)
				{
					value = Enum.Parse(targetType, stringValue);
					return true;
				}
				else if (targetType == typeof(bool))
				{
					value = bool.Parse(stringValue);
					return true;
				}
				else if (targetType == typeof(short))
				{
					value = short.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(int))
				{
					value = int.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(long))
				{
					value = long.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(float))
				{
					value = float.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(double))
				{
					value = double.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(decimal))
				{
					value = decimal.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(DateTime))
				{
					value = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(DateTimeOffset))
				{
					value = DateTimeOffset.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(TimeSpan))
				{
					value = TimeSpan.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}
				else if (targetType == typeof(TimeOnly))
				{
					value = TimeOnly.Parse(stringValue, CultureInfo.InvariantCulture);
					return true;
				}

				return false;
			}

			if (targetTypeInfo.IsEnum)
				value = Enum.ToObject(targetType, value);

			if (value is DateTime dt)
			{
				if (targetType == typeof(DateOnly))
				{
					value = DateOnly.FromDateTime(dt);
					return true;
				}
				if (targetType == typeof(TimeOnly))
				{
					value = TimeOnly.FromDateTime(dt);
					return true;
				}
				if (targetType == typeof(TimeSpan))
				{
					value = dt.TimeOfDay;
					return true;
				}
			}

			if (value is TimeSpan ts)
			{
				if (targetType == typeof(TimeOnly))
				{
					value = TimeOnly.FromTimeSpan(ts);
					return true;
				}
			}

			//this will handle numeric conversions
			value = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
			return true;
		}
		catch (Exception ex)
		{
			conversionException = ex;
			return false;
		}
	}
}
