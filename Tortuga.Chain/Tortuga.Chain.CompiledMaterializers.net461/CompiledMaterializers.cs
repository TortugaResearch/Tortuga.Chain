using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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
        /// Allows compilation of the ToObject and ToCollection materializer.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <returns></returns>
        public static Compiled<TCommand, TParameter> Compile<TCommand, TParameter>(this DbCommandBuilder<TCommand, TParameter> commandBuilder)
            where TCommand : DbCommand
            where TParameter : DbParameter
        {
            return new Compiled<TCommand, TParameter>(commandBuilder);
        }

        internal static MethodDelegate<TObject> CreateBuilder<TObject>(DataSource dataSource, string sql, IDataReader reader)
            where TObject : new()
        {
            var cache = dataSource.GetExtensionData<CompilerCache>();
            var result = cache.GetBuilder<TObject>(sql);
            if (result != null)
                return result;

            var code = new StringBuilder();

            var typeName = typeof(TObject).FullName; //TODO: Add support for generic types and inner classes


            var columns = new Dictionary<string, Tuple<int, Type, string>>(StringComparer.OrdinalIgnoreCase);
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
                else getter = "reader.GetValue";

                columns.Add(columnName, Tuple.Create(i, columnType, getter));
            }


            var eval = CreateScriptEvaluator(typeof(TObject), CSScript.Evaluator.ReferenceAssemblyOf(reader).ReferenceAssemblyOf<IDataReader>());


            code.AppendLine($"{typeName} Load({reader.GetType().FullName} reader)");
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
            code.AppendLine("    return result;");
            code.AppendLine("}");


            var codeToString = code.ToString();
            try
            {
                result = eval.CreateDelegate<TObject>(codeToString);

                MaterializerCompiled?.Invoke(typeof(CompiledMaterializers), new MaterializerCompilerEventArgs(dataSource, sql, codeToString, typeof(TObject)));

                cache.StoreBuilder(sql, result);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(codeToString);
                ex.Data["Code"] = codeToString;
                throw;
            }

        }

        /// <summary>
        /// Creates the script evaluator by ensuring that all of the relevant assemblies are loaded.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <returns></returns>
        private static IEvaluator CreateScriptEvaluator(Type type, IEvaluator evaluator)
        {
            evaluator = evaluator.ReferenceAssembly(type.Assembly);
            foreach (var property in MetadataCache.GetMetadata(type).Properties.Where(p => p.Decompose))
                evaluator = CreateScriptEvaluator(property.PropertyType, evaluator);
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
        /// <param name="properties">The properties for the curent object.</param>
        /// <param name="columnIndex">Index of the column being read.</param>
        /// <param name="path">The path to the object whose properties are being set.</param>
        /// <param name="decompositionPrefix">The decomposition prefix used when reading the column data.</param>
        private static void SetProperties(StringBuilder code, Dictionary<string, Tuple<int, Type, string>> columns, PropertyMetadataCollection properties, int columnIndex, string path, string decompositionPrefix)
        {
            foreach (var property in properties)
            {
                if (property.Decompose)
                {
                    SetProperties(code, columns, MetadataCache.GetMetadata(property.PropertyType).Properties, columnIndex, path + "." + property.Name, decompositionPrefix + property.DecompositionPrefix);
                }

                if (property.MappedColumnName == null)
                    continue;

                Tuple<int, Type, string> column;
                if (!columns.TryGetValue(decompositionPrefix + property.MappedColumnName, out column))
                    continue; //not a valid column

                if (column.Item1 != columnIndex)
                    continue; //we'll get it on another iteration

                if (property.PropertyType.IsClass || property.PropertyType.Name == "Nullable`1" && property.PropertyType.IsGenericType)
                {
                    //null handler
                    code.AppendLine($"    if (reader.IsDBNull({column.Item1}))");
                    code.AppendLine($"        {path}.{property.Name} = null;");
                    code.AppendLine($"    else");
                    code.AppendLine($"        {path}.{property.Name} = ({column.Item2.FullName}){column.Item3}({column.Item1});");
                }
                else
                {
                    //non-null handler
                    code.AppendLine($"    {path}.{property.Name} = ({column.Item2.FullName}){column.Item3}({column.Item1});");
                }

            }
        }

        /// <summary>
        /// Occurs when a materializer is compiled.
        /// </summary>
        public static event EventHandler<MaterializerCompilerEventArgs> MaterializerCompiled;
    }

}


