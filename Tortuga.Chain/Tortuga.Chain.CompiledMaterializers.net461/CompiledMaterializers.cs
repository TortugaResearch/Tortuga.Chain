using CSScriptLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    public static class CompiledMaterializers
    {

        public static Compiled<TCommand, TParameter> Compiled<TCommand, TParameter>(this DbCommandBuilder<TCommand, TParameter> commandBuilder)
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

            var localsList = MetadataCache.GetMetadata(typeof(TObject)).ColumnsFor;

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


            var eval = CSScript.Evaluator.ReferenceAssemblyOf<TObject>().ReferenceAssemblyOf<IDataReader>(); //.ReferenceDomainAssemblies(DomainAssemblies.None);


            code.AppendLine($"{typeName} Load(System.Data.IDataReader reader)");
            code.AppendLine("{");
            code.AppendLine($"    var result = new {typeName}();");
            foreach (var property in MetadataCache.GetMetadata(typeof(TObject)).Properties)
            {
                if (property.Decompose)
                    throw new InvalidOperationException("Compiled materializers do not support the Decompose atrtibute");

                if (property.MappedColumnName == null)
                    continue;

                Tuple<int, Type, string> column;
                if (!columns.TryGetValue(property.MappedColumnName, out column))
                    continue;


                if (property.PropertyType.IsClass || property.PropertyType.Name == "Nullable`1" && property.PropertyType.IsGenericType)
                {
                    //null handler
                    code.AppendLine($"    if (reader.IsDBNull({column.Item1}))");
                    code.AppendLine($"        result.{property.Name} = null;");
                    code.AppendLine($"    else");
                    code.AppendLine($"        result.{property.Name} = ({column.Item2.FullName}){column.Item3}({column.Item1});");
                }
                else
                {
                    //non-null handler
                    code.AppendLine($"    result.{property.Name} = ({column.Item2.FullName}){column.Item3}({column.Item1});");
                }

            }

            code.AppendLine("    return result;");
            code.AppendLine("}");


            try
            {
                result = eval.CreateDelegate<TObject>(code.ToString());

                MaterializerCompiled?.Invoke(typeof(CompiledMaterializers), new MaterializerCompiledEventArgs(dataSource, sql, code.ToString(), typeof(TObject)));

                cache.StoreBuilder(sql, result);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(code.ToString());
                ex.Data["Code"] = code.ToString();
                throw;
            }

        }

        /// <summary>
        /// Occurs when a materializer is compiled.
        /// </summary>
        public static event EventHandler<MaterializerCompiledEventArgs> MaterializerCompiled;
    }

}


