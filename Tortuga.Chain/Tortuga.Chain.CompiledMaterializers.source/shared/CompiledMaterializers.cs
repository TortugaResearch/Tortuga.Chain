using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain
{
    /// <summary>
    /// Utility class that enables access to the compiled version of the Object and Collection materializers.
    /// </summary>
    public static class CompiledMaterializers
    {
        /// <summary>
        /// Occurs when a materializer is compiled.
        /// </summary>
        public static event EventHandler<MaterializerCompilerEventArgs> MaterializerCompiled;

        /// <summary>
        /// Occurs when materializer fails to compile.
        /// </summary>
        public static event EventHandler<MaterializerCompilerEventArgs> MaterializerCompilerFailed;

        /// <summary>
        /// Allows compilation of the ToObject materializer.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <returns></returns>
        public static CompiledSingleRow<TCommand, TParameter> Compile<TCommand, TParameter>(this SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
            where TCommand : DbCommand
            where TParameter : DbParameter
        {
            return new CompiledSingleRow<TCommand, TParameter>(commandBuilder);
        }

        /// <summary>
        /// Allows compilation of the ToObject and ToCollection materializer.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <returns></returns>
        public static CompiledMultipleRow<TCommand, TParameter> Compile<TCommand, TParameter>(this MultipleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
            where TCommand : DbCommand
            where TParameter : DbParameter
        {
            return new CompiledMultipleRow<TCommand, TParameter>(commandBuilder);
        }

        /// <summary>
        /// Allows compilation of the ToObject and ToCollection materializer.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <returns></returns>
        public static CompiledMultipleTable<TCommand, TParameter> Compile<TCommand, TParameter>(this MultipleTableDbCommandBuilder<TCommand, TParameter> commandBuilder)
            where TCommand : DbCommand
            where TParameter : DbParameter
        {
            return new CompiledMultipleTable<TCommand, TParameter>(commandBuilder);
        }

        /// <summary>
        /// Creates the builder.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static MethodDelegate<TObject> CreateBuilder<TObject>(IDataSource dataSource, string sql, IDataReader reader)
            where TObject : new()
        {
            var cache = dataSource.GetExtensionData<CompilerCache>();
            var result = cache.GetBuilder<TObject>(sql);
            if (result != null)
                return result;

            var code = new StringBuilder();

            var typeName = MetadataCache.GetMetadata(typeof(TObject)).CSharpFullName;

            var changeTracker = typeof(TObject).GetInterfaces().Any(x => x == typeof(IChangeTracking));

            var columns = new Dictionary<string, ColumnData>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnType = reader.GetFieldType(i);
                //var columnTypeName = columnType.FullName;
                string getter;
                if (columnType == typeof(bool)) getter = "reader.GetBoolean";
                else if (columnType == typeof(byte)) getter = "reader.GetByte";
                else if (columnType == typeof(char)) getter = "reader.GetChar";
                else if (columnType == typeof(DateTime)) getter = "reader.GetDateTime";
                else if (columnType == typeof(decimal)) getter = "reader.GetDecimal";
                else if (columnType == typeof(double)) getter = "reader.GetDouble";
                else if (columnType == typeof(float)) getter = "reader.GetFloat";
                else if (columnType == typeof(Guid)) getter = "reader.GetGuid";
                else if (columnType == typeof(short)) getter = "reader.GetInt16";
                else if (columnType == typeof(int)) getter = "reader.GetInt32";
                else if (columnType == typeof(long)) getter = "reader.GetInt64";
                else if (columnType == typeof(string)) getter = "reader.GetString";
                else if (columnType == typeof(byte[])) getter = "(byte[])reader.GetValue";
                else if (columnType == typeof(UInt16)) getter = "(System.UInt16)reader.GetValue";
                else if (columnType == typeof(UInt32)) getter = "(System.UInt32)reader.GetValue";
                else if (columnType == typeof(UInt64)) getter = "(System.UInt64)reader.GetValue";
                else getter = "reader.GetValue";

                columns.Add(columnName, new ColumnData(i, columnType, getter));
            }

            var evaluator = CSScript.RoslynEvaluator.Reset(false);
            //evaluator = evaluator.ReferenceAssemblyOf<string>();
            //evaluator = evaluator.ReferenceAssemblyOf<IDataReader>();
            //evaluator = evaluator.ReferenceAssemblyOf<IChangeTracking>();
            evaluator = evaluator.ReferenceAssemblyOf(reader);
            evaluator = AugmentScriptEvaluator(evaluator, typeof(TObject));

            //We need a public version of the DataReader class.
            //If we can't find one, default to the slower IDataReader interface.
            var readerType = reader.GetType();
            while (!readerType.IsPublic)
                readerType = readerType.BaseType;
            if (readerType.GetInterface("IDataReader") == null)
                readerType = typeof(IDataReader);

            code.AppendLine($"{typeName} Load({readerType.FullName} reader)");
            code.AppendLine("{");
            code.AppendLine($"    var result = new {typeName}();");

            var properties = MetadataCache.GetMetadata(typeof(TObject)).Properties;
            var path = "result";

            ConstructDecomposedObjects(code, path, properties);

            //This loop is slow, but it ensure that we always generate the reader.GetXxx methods in the correct order.
            for (var i = 0; i < reader.FieldCount; i++)
            {
                SetProperties(code, columns, properties, i, "result", null);
            }

            if (changeTracker)
                code.AppendLine("    ((System.ComponentModel.IChangeTracking)result).AcceptChanges();");

            code.AppendLine("    return result;");
            code.AppendLine("}");

            var codeToString = code.ToString();
            try
            {
                result = evaluator.CreateDelegate<TObject>(codeToString);

                MaterializerCompiled?.Invoke(typeof(CompiledMaterializers), new MaterializerCompilerEventArgs(dataSource, sql, codeToString, typeof(TObject)));

                cache.StoreBuilder(sql, result);

                return result;
            }
            catch (Exception ex)
            {
                MaterializerCompilerFailed?.Invoke(typeof(CompiledMaterializers), new MaterializerCompilerEventArgs(dataSource, sql, codeToString, typeof(TObject)));

                //Debug.WriteLine(codeToString);
                //foreach (var item in evaluator.GetReferencedAssemblies())
                //    Debug.WriteLine("Referenced Assembly: " + item.FullName);
                ex.Data["Code"] = codeToString;
                //ex.Data["Evaluator"] = evaluator;
                throw;
            }
        }

        /// <summary>
        /// Creates the script evaluator by ensuring that all of the relevant assemblies are loaded.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static IEvaluator AugmentScriptEvaluator(IEvaluator evaluator, Type type)
        {
            evaluator = evaluator.ReferenceAssembly(type.Assembly);

            if (type.BaseType != typeof(object))
                evaluator = AugmentScriptEvaluator(evaluator, type.BaseType);

            foreach (var property in MetadataCache.GetMetadata(type).Properties.Where(p => p.Decompose))
                evaluator = AugmentScriptEvaluator(evaluator, property.PropertyType);

            return evaluator;
        }

        /// <summary>
        /// Constructs the decomposed objects as necessary.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="path">The path.</param>
        /// <param name="properties">The properties.</param>
        private static void ConstructDecomposedObjects(StringBuilder code, string path, PropertyMetadataCollection properties)
        {
            foreach (var property in properties)
            {
                if (property.Decompose)
                {
                    if (property.CanWrite)
                    {
                        code.AppendLine($"    if ({path}.{property.Name} == null)");
                        code.AppendLine($"    {path}.{property.Name} = new {property.PropertyType.FullName}();");
                    }

                    ConstructDecomposedObjects(code, path + "." + property.Name, MetadataCache.GetMetadata(property.PropertyType).Properties);
                }
            }
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="code">The code being generated.</param>
        /// <param name="columns">The columns in the data reader.</param>
        /// <param name="properties">The properties for the current object.</param>
        /// <param name="columnIndex">Index of the column being read.</param>
        /// <param name="path">The path to the object whose properties are being set.</param>
        /// <param name="decompositionPrefix">The decomposition prefix used when reading the column data.</param>
        private static void SetProperties(StringBuilder code, Dictionary<string, ColumnData> columns, PropertyMetadataCollection properties, int columnIndex, string path, string decompositionPrefix)
        {
            foreach (var property in properties)
            {
                if (property.Decompose)
                {
                    SetProperties(code, columns, MetadataCache.GetMetadata(property.PropertyType).Properties, columnIndex, path + "." + property.Name, decompositionPrefix + property.DecompositionPrefix);
                }

                if (property.MappedColumnName == null)
                    continue;

                ColumnData column;
                if (!columns.TryGetValue(decompositionPrefix + property.MappedColumnName, out column))
                    continue; //not a valid column

                if (column.Index != columnIndex)
                    continue; //we'll get it on another iteration

                if (property.PropertyType == column.ColumnType || (string.Equals(property.PropertyType.Name, "Nullable`1", StringComparison.Ordinal) && property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments[0] == column.ColumnType))
                {
                    if (property.PropertyType.IsClass || (string.Equals(property.PropertyType.Name, "Nullable`1", StringComparison.Ordinal) && property.PropertyType.IsGenericType))
                    {
                        //null handler
                        code.AppendLine($"    if (reader.IsDBNull({column.Index}))");
                        code.AppendLine($"        {path}.{property.Name} = null;");
                        code.AppendLine($"    else");
                        code.AppendLine($"        {path}.{property.Name} = {column.Getter}({column.Index});");
                    }
                    else
                    {
                        //non-null handler
                        code.AppendLine($"    {path}.{property.Name} = {column.Getter}({column.Index});");
                    }
                }
                else //type casting is required
                {
                    var propertyTypeName = MetadataCache.GetMetadata(property.PropertyType).CSharpFullName;

                    if (property.PropertyType.IsClass || (string.Equals(property.PropertyType.Name, "Nullable`1", StringComparison.Ordinal) && property.PropertyType.IsGenericType))
                    {
                        //null handler
                        code.AppendLine($"    if (reader.IsDBNull({column.Index}))");
                        code.AppendLine($"        {path}.{property.Name} = null;");
                        code.AppendLine($"    else");
                        code.AppendLine($"        {path}.{property.Name} = ({propertyTypeName}){column.Getter}({column.Index});");
                    }
                    else
                    {
                        //non-null handler
                        code.AppendLine($"    {path}.{property.Name} = ({propertyTypeName}){column.Getter}({column.Index});");
                    }
                }
            }
        }

        private class ColumnData
        {
            public ColumnData(int index, Type columnType, string getter)
            {
                ColumnType = columnType;
                Getter = getter;
                Index = index;
            }

            public Type ColumnType { get; }
            public string Getter { get; }
            public int Index { get; }
        }
    }
}