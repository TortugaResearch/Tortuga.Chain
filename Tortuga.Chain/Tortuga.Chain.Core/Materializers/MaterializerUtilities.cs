using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Core;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Materializer utilities are used for constructing materializers.
	/// </summary>
	/// <remarks>For internal use only.</remarks>
	public static class MaterializerUtilities
	{
		static readonly Type[] s_EmptyTypeList = Array.Empty<Type>();

		/// <summary>
		/// Checks the delete row count.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="deleteOptions">The delete options.</param>
		/// <param name="expectedRowCount">The expected row count.</param>
		/// <returns>The execution token with an attached event handler.</returns>
		public static T CheckDeleteRowCount<T>(this T executionToken, DeleteOptions deleteOptions, int? expectedRowCount) where T : ExecutionToken
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");

			if (expectedRowCount.HasValue && deleteOptions.HasFlag(DeleteOptions.CheckRowsAffected))
				executionToken.CommandExecuted += (s, e) => CheckDeleteRowCount(s, e, expectedRowCount.Value);
			return executionToken;
		}

		/// <summary>
		/// Checks the delete row count.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="deleteOptions">The delete options.</param>
		/// <returns>The execution token with an attached event handler.</returns>
		public static T CheckDeleteRowCount<T>(this T executionToken, DeleteOptions deleteOptions) where T : ExecutionToken
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");

			if (deleteOptions.HasFlag(DeleteOptions.CheckRowsAffected))
				executionToken.CommandExecuted += CheckDeleteRowCount;
			return executionToken;
		}

		/// <summary>
		/// Checks the update row count.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="updateOptions">The update options.</param>
		/// <param name="expectedRowCount">The expected row count.</param>
		/// <returns>The execution token with an attached event handler.</returns>
		public static T CheckUpdateRowCount<T>(this T executionToken, UpdateOptions updateOptions, int? expectedRowCount) where T : ExecutionToken
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");

			if (expectedRowCount.HasValue && !updateOptions.HasFlag(UpdateOptions.IgnoreRowsAffected))
				executionToken.CommandExecuted += (s, e) => CheckUpdateRowCount(s, e, expectedRowCount.Value);
			return executionToken;
		}

		/// <summary>
		/// Checks the update row count.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="updateOptions">The update options.</param>
		/// <returns>The execution token with an attached event handler.</returns>
		public static T CheckUpdateRowCount<T>(this T executionToken, UpdateOptions updateOptions) where T : ExecutionToken
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");

			if (!updateOptions.HasFlag(UpdateOptions.IgnoreRowsAffected))
				executionToken.CommandExecuted += CheckUpdateRowCount;
			return executionToken;
		}

		internal static StreamingObjectConstructor<T> AsObjectConstructor<T>(this DbDataReader reader, ConstructorMetadata? constructor, IReadOnlyList<ColumnMetadata> nonNullableColumns, MaterializerTypeConverter converter)
			where T : class
		{
			return new StreamingObjectConstructor<T>(reader, constructor, nonNullableColumns, converter);
		}

		internal static StreamingObjectConstructor<T> AsObjectConstructor<T>(this DbDataReader reader, IReadOnlyList<Type>? constructorSignature, IReadOnlyList<ColumnMetadata> nonNullableColumns, MaterializerTypeConverter converter)
	where T : class
		{
			return new StreamingObjectConstructor<T>(reader, constructorSignature, nonNullableColumns, converter);
		}

		static internal T ConstructObject<T>(IReadOnlyDictionary<string, object?> source, ConstructorMetadata? constructor, MaterializerTypeConverter converter, bool? populateComplexObject = null)
			where T : class
		{
			//If we didn't get a constructor, look for a default constructor to use.
			if (constructor == null)
				constructor = MetadataCache.GetMetadata(typeof(T)).Constructors.Find(s_EmptyTypeList);
			if (constructor == null)
				throw new MappingException($"Cannot find a default constructor for {typeof(T).Name}");

			//populate properties when using a default constructor
			if (!populateComplexObject.HasValue)
				populateComplexObject = constructor.Signature.Length == 0;

			//Make sure we have all of the requested parameters
			var constructorParameters = constructor.ParameterNames;
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				if (!source.ContainsKey(constructorParameters[i]))
					throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
			}

			//Build the constructor
			var parameters = new object?[constructorParameters.Length];
			for (var i = 0; i < constructorParameters.Length; i++)
			{
				parameters[i] = source[constructorParameters[i]];
			}
			var result = (T)constructor.ConstructorInfo.Invoke(parameters);

			if (populateComplexObject.Value) //Populate properties and child objects
				PopulateComplexObject(source, result, null, converter);

			//Change tracking objects should be materialized as unchanged.
			(result as IChangeTracking)?.AcceptChanges();

			return result;
		}

		/// <summary>
		/// Populates the complex object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="target">The object being populated.</param>
		/// <param name="decompositionPrefix">The decomposition prefix.</param>
		/// <param name="converter">The type converter.</param>
		/// <exception cref="System.ArgumentNullException">source</exception>
		/// <exception cref="System.ArgumentNullException">target</exception>
		/// <remarks>This honors the Column and Decompose attributes.</remarks>
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		static internal void PopulateComplexObject<T>(IReadOnlyDictionary<string, object?> source, T target, string? decompositionPrefix, MaterializerTypeConverter converter)
			where T : class
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
			if (target == null)
				throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

			foreach (var property in MetadataCache.GetMetadata(target.GetType()).Properties)
			{
				string mappedColumnName = decompositionPrefix + property.MappedColumnName;
				if (property.CanWrite && source.ContainsKey(mappedColumnName))
				{
					var value = source[mappedColumnName];

					value = SetProperty(target, property, value, null, converter);
				}
				else if (property.Decompose)
				{
					SetDecomposedProperty(source, target, decompositionPrefix, property, converter);
				}
			}
		}

		static internal void PopulateComplexObject<T>(IReadOnlyDictionary<int, object?> source, T target, string? decompositionPrefix, IList<OrdinalMappedProperty<T>> mappedProperties, IList<MappedProperty<T>> decomposedProperties, MaterializerTypeConverter converter)
			where T : class
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source), $"{nameof(source)} is null.");
			if (target == null)
				throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

			foreach (var property in mappedProperties)
			{
				var value = source[property.Ordinal];

				SetProperty(target, property.PropertyMetadata, value, property, converter);
			}

			foreach (var property in decomposedProperties)
			{
				SetDecomposedProperty((IReadOnlyDictionary<string, object?>)source, target, decompositionPrefix, property.PropertyMetadata, converter);
			}
		}

		internal static void PopulateDictionary(this DbDataReader source, IDictionary<string, object?> result)
		{
			for (var i = 0; i < source.FieldCount; i++)
			{
				object? temp = source[i];
				if (temp == DBNull.Value)
					temp = null;

				result.Add(source.GetName(i), temp);
			}
		}

		static internal Dictionary<string, object?>? ReadDictionary(this DbDataReader source)
		{
			if (!source.Read())
				return null;

			return ToDictionary(source);
		}

		static internal async Task<Dictionary<string, object?>?> ReadDictionaryAsync(this DbDataReader source)
		{
			if (!(await source.ReadAsync().ConfigureAwait(false)))
				return null;

			return ToDictionary(source);
		}

		static internal int RemainingRowCount(this DbDataReader source)
		{
			var count = 0;
			while (source.Read())
				count += 1;
			return count;
		}

		static internal async Task<int> RemainingRowCountAsync(this DbDataReader source)
		{
			var count = 0;
			while (await source.ReadAsync().ConfigureAwait(false))
				count += 1;
			return count;
		}

		internal static Dictionary<string, object?> ToDictionary(this DbDataReader source)
		{
			var result = new Dictionary<string, object?>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
			PopulateDictionary(source, result);
			return result;
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteOptions")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IgnoreRowsAffected")]
		static void CheckDeleteRowCount(object? sender, CommandExecutedEventArgs e)
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} is null.");

			var token = (ExecutionToken)sender;

			if (e.RowsAffected == null)
				throw new InvalidOperationException($"The database did not report how many rows were affected by operation {token.OperationName}. Either remove the DeleteOptions.CheckRowsAffected flag or report this as an bug in {token.GetType().FullName}.");
			else if (e.RowsAffected == 0)
				throw new MissingDataException($"Expected one row to be affected by the operation {token.OperationName} but none were.");
			else if (e.RowsAffected > 1)
				throw new UnexpectedDataException($"Expected one row to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IgnoreRowsAffected")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteOptions")]
		static void CheckDeleteRowCount(object? sender, CommandExecutedEventArgs e, int expectedRowCount)
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} is null.");

			var token = (ExecutionToken)sender;

			if (e.RowsAffected == null)
				throw new InvalidOperationException($"The database did not report how many rows were affected by operation {token.OperationName}. Either remove the DeleteOptions.CheckRowsAffected flag or report this as an bug in {token.GetType().FullName}.");
			else if (e.RowsAffected > expectedRowCount)
				throw new UnexpectedDataException($"Expected {expectedRowCount} rows to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
			else if (e.RowsAffected < expectedRowCount)
				throw new MissingDataException($"Expected {expectedRowCount} rows to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateOptions")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IgnoreRowsAffected")]
		static void CheckUpdateRowCount(object? sender, CommandExecutedEventArgs e)
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} is null.");

			var token = (ExecutionToken)sender;

			if (e.RowsAffected == null)
				throw new InvalidOperationException($"The database did not report how many rows were affected by operation {token.OperationName}. Either use the  UpdateOptions.IgnoreRowsAffected flag or report this as an bug in {token.GetType().FullName}.");
			else if (e.RowsAffected == 0)
				throw new MissingDataException($"Expected one row to be affected by the operation {token.OperationName} but none were.");
			else if (e.RowsAffected > 1)
				throw new UnexpectedDataException($"Expected one row to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IgnoreRowsAffected")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateOptions")]
		static void CheckUpdateRowCount(object? sender, CommandExecutedEventArgs e, int expectedRowCount)
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} is null.");

			var token = (ExecutionToken)sender;

			if (e.RowsAffected == null)
				throw new InvalidOperationException($"The database did not report how many rows were affected by operation {token.OperationName}. Either use the UpdateOptions.IgnoreRowsAffected flag or report this as an bug in {token.GetType().FullName}.");
			else if (e.RowsAffected > expectedRowCount)
				throw new UnexpectedDataException($"Expected {expectedRowCount} rows to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
			else if (e.RowsAffected < expectedRowCount)
				throw new MissingDataException($"Expected {expectedRowCount} rows to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
		}

		static void SetDecomposedProperty(IReadOnlyDictionary<string, object?> source, object target, string? decompositionPrefix, PropertyMetadata property, MaterializerTypeConverter converter)
		{
			object? child = null;

			if (property.CanRead)
				child = property.InvokeGet(target);

			if (child == null && property.CanWrite && property.PropertyType.GetConstructor(Array.Empty<Type>()) != null)
			{
				child = Activator.CreateInstance(property.PropertyType);
				property.InvokeSet(target, child);
			}

			if (child != null)
				PopulateComplexObject(source, child, decompositionPrefix + property.DecompositionPrefix, converter);
		}

		static object? SetProperty<T>(T target, PropertyMetadata property, object? value, OrdinalMappedProperty<T>? mapper, MaterializerTypeConverter converter)
			where T : class
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target), $"{nameof(target)} is null.");

			if (property == null)
				throw new ArgumentNullException(nameof(property), $"{nameof(property)} is null.");

			var targetType = property.PropertyType;

			if (value != null && targetType != value.GetType())
			{
				if (!converter.TryConvertType(targetType, ref value, out var conversionException))
				{
					throw new MappingException($"Cannot map value of type {value!.GetType().FullName} to property {property.Name} of type {targetType.Name}.", conversionException);
				}
			}

			if (mapper == null || value == null)
				property.InvokeSet(target, value);
			else
				mapper.InvokeSet(target, value);

			return value;
		}
	}
}