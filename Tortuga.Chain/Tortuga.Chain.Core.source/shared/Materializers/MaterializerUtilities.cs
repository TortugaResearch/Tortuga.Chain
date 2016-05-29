using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{


    /// <summary>
    /// Materializer utilities are used for constructing materializers. 
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public static class MaterializerUtilities
    {
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
            if (!updateOptions.HasFlag(UpdateOptions.IgnoreRowsAffected))
                executionToken.CommandExecuted += CheckUpdateRowCount;
            return executionToken;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateOptions")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IgnoreRowsAffected")]
        private static void CheckUpdateRowCount(object sender, CommandExecutedEventArgs e)
        {
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
        private static void CheckUpdateRowCount(object sender, CommandExecutedEventArgs e, int expectedRowCount)
        {
            var token = (ExecutionToken)sender;

            if (e.RowsAffected == null)
                throw new InvalidOperationException($"The database did not report how many rows were affected by operation {token.OperationName}. Either use the UpdateOptions.IgnoreRowsAffected flag or report this as an bug in {token.GetType().FullName}.");
            else if (e.RowsAffected != expectedRowCount)
                throw new UnexpectedDataException($"Expected {expectedRowCount} rows to be affected by the operation {token.OperationName} but {e.RowsAffected} were affected instead.");
        }

        internal static StreamingObjectConstructor<T> AsObjectConstructor<T>(this DbDataReader reader, IReadOnlyList<Type> constructorSignature)
            where T : class
        {
            return new StreamingObjectConstructor<T>(reader, constructorSignature);
        }



        static readonly Type[] s_EmptyTypeList = new Type[0];

        static internal T ConstructObject<T>(IReadOnlyDictionary<string, object> source, IReadOnlyList<Type> constructorSignature, bool? populateComplexObject = null)
        {
            if (source == null || source.Count == 0)
                throw new ArgumentException($"{nameof(source)} is null or empty.", nameof(source));

            constructorSignature = constructorSignature ?? s_EmptyTypeList;

            if (!populateComplexObject.HasValue)
                populateComplexObject = constructorSignature.Count == 0;

            var desiredType = typeof(T);
            var constructor = MetadataCache.GetMetadata(desiredType).Constructors.Find(constructorSignature);

            if (constructor == null)
            {
                var types = string.Join(", ", constructorSignature.Select(t => t.Name));
                throw new MappingException($"Cannot find a constructor on {desiredType.Name} with the types [{types}]");
            }

            var constructorParameters = constructor.ParameterNames;
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                if (!source.ContainsKey(constructorParameters[i]))
                    throw new MappingException($"Cannot find a column that matches the parameter {constructorParameters[i]}");
            }

            return ConstructObject<T>(source, constructor);
        }

        static internal T ConstructObject<T>(IReadOnlyDictionary<string, object> source, ConstructorMetadata constructor, bool? populateComplexObject = null)
        {
            if (!populateComplexObject.HasValue)
                populateComplexObject = constructor.ParameterNames.Length == 0;

            var constructorParameters = constructor.ParameterNames;
            var parameters = new object[constructorParameters.Length];
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                parameters[i] = source[constructorParameters[i]];
            }
            var result = (T)constructor.ConstructorInfo.Invoke(parameters);

            if (populateComplexObject.Value)
                PopulateComplexObject(source, result, null);

            //Change tracking objects shouldn't be materialized as unchanged.
            (result as IChangeTracking)?.AcceptChanges();

            return result;
        }

        /// <summary>
        /// Populates the complex object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The object being populated.</param>
        /// <param name="decompositionPrefix">The decomposition prefix.</param>
        /// <remarks>This honors the Column and Decompose attributes.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static internal void PopulateComplexObject(IReadOnlyDictionary<string, object> source, object target, string decompositionPrefix)
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

                    if (value != null && property.PropertyType != value.GetType())
                    {
                        var targetType = property.PropertyType;
                        var targetTypeInfo = targetType.GetTypeInfo();

                        //For Nullable<T>, we only care about the type parameter
                        if (targetType.Name == "Nullable`1" && targetTypeInfo.IsGenericType)
                        {
                            targetType = targetType.GenericTypeArguments[0];
                            targetTypeInfo = targetType.GetTypeInfo();
                        }

                        //some database return strings when we want strong types
                        if (value is string)
                        {
                            if (targetType == typeof(XElement))
                                value = XElement.Parse((string)value);
                            else if (targetType == typeof(XDocument))
                                value = XDocument.Parse((string)value);
                            else if (targetTypeInfo.IsEnum)
                                value = Enum.Parse(targetType, (string)value);

                            else if (targetType == typeof(bool))
                                value = bool.Parse((string)value);
                            else if (targetType == typeof(short))
                                value = short.Parse((string)value);
                            else if (targetType == typeof(int))
                                value = int.Parse((string)value);
                            else if (targetType == typeof(long))
                                value = long.Parse((string)value);
                            else if (targetType == typeof(float))
                                value = float.Parse((string)value);
                            else if (targetType == typeof(double))
                                value = double.Parse((string)value);
                            else if (targetType == typeof(decimal))
                                value = decimal.Parse((string)value);

                            else if (targetType == typeof(DateTime))
                                value = DateTime.Parse((string)value);
                            else if (targetType == typeof(DateTimeOffset))
                                value = DateTimeOffset.Parse((string)value);
                        }
                        else
                        {
                            if (targetTypeInfo.IsEnum)
                                value = Enum.ToObject(targetType, value);
                        }

                        //this will handle numeric conversions
                        if (value != null && targetType != value.GetType())
                        {
                            try
                            {
                                value = Convert.ChangeType(value, targetType);
                            }
                            catch (Exception ex)
                            {
                                throw new MappingException($"Cannot map value of type {value.GetType().FullName} to property {property.Name} of type {targetType.Name}.", ex);
                            }
                        }
                    }

                    property.InvokeSet(target, value);
                }
                else if (property.Decompose)
                {
                    object child = null;

                    if (property.CanRead)
                        child = property.InvokeGet(target);

                    if (child == null && property.CanWrite && property.PropertyType.GetConstructor(new Type[0]) != null)
                    {
                        child = Activator.CreateInstance(property.PropertyType);
                        property.InvokeSet(target, child);
                    }

                    if (child != null)
                        PopulateComplexObject(source, child, decompositionPrefix + property.DecompositionPrefix);
                }
            }
        }

        static internal Dictionary<string, object> ReadDictionary(this DbDataReader source)
        {
            if (!source.Read())
                return null;

            return ToDictionary(source);
        }

        static internal async Task<Dictionary<string, object>> ReadDictionaryAsync(this DbDataReader source)
        {
            if (!(await source.ReadAsync()))
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
            while (await source.ReadAsync())
                count += 1;
            return count;
        }
        internal static Dictionary<string, object> ToDictionary(this DbDataReader source)
        {
            var result = new Dictionary<string, object>(source.FieldCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < source.FieldCount; i++)
            {
                var temp = source[i];
                if (temp == DBNull.Value)
                    temp = null;

                result.Add(source.GetName(i), temp);
            }
            return result;
        }
    }
}
