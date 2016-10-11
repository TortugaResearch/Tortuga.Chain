using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Csv;

namespace Tortuga.Chain
{
    /// <summary>
    /// Extension methods that expose CSV based materializers.
    /// </summary>
    public static class CsvMaterializers
    {
        /// <summary>
        /// Materializes the output as a CSV formatted string.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="csvSerializer">Optional CSV serializer. Used to override how non-null values are converted into strings.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <param name="desiredColumns">The desired columns. If null, all columns are returned.</param>
        /// <returns>ILink&lt;System.String&gt;.</returns>
        public static ILink<string> ToCsv<TCommand, TParameter>(this SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder, IReadOnlyList<string> desiredColumns = null, CsvSerializer csvSerializer = null, bool includeHeaders = true)
    where TCommand : DbCommand
    where TParameter : DbParameter
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");

            return new CsvStringMaterializer<TCommand, TParameter>(commandBuilder, csvSerializer ?? new CsvSerializer(), includeHeaders, desiredColumns);
        }

        /// <summary>
        /// Materializes the output as a CSV formatted string.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="csvSerializer">Optional CSV serializer. Used to override how non-null values are converted into strings.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <param name="desiredColumns">The desired columns. If null, all columns are returned.</param>
        /// <returns>ILink&lt;System.Int32&gt;.</returns>
        public static ILink<int> ToCsv<TCommand, TParameter>(this SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder, TextWriter outputStream, IReadOnlyList<string> desiredColumns = null, CsvSerializer csvSerializer = null, bool includeHeaders = true)
where TCommand : DbCommand
where TParameter : DbParameter
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream), $"{nameof(outputStream)} is null.");

            return new CsvStreamMaterializer<TCommand, TParameter>(commandBuilder, outputStream, csvSerializer ?? new CsvSerializer(), includeHeaders, desiredColumns);
        }

        /// <summary>
        /// Materializes the output as a CSV formatted string.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="desiredColumns">The properties of this type will be used to determine the desired columns..</param>
        /// <param name="csvSerializer">Optional CSV serializer. Used to override how non-null values are converted into strings.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <returns>ILink&lt;System.String&gt;.</returns>
        public static ILink<string> ToCsv<TCommand, TParameter>(this SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder, Type desiredColumns, CsvSerializer csvSerializer = null, bool includeHeaders = true)
    where TCommand : DbCommand
    where TParameter : DbParameter
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
            if (desiredColumns == null)
                throw new ArgumentNullException(nameof(desiredColumns), $"{nameof(desiredColumns)} is null.");

            var columnsFor = MetadataCache.GetMetadata(desiredColumns).ColumnsFor;
            if (columnsFor.Length == 0)
                throw new MappingException($"Type {desiredColumns.Name} has no writable columns.");
            return new CsvStringMaterializer<TCommand, TParameter>(commandBuilder, csvSerializer ?? new CsvSerializer(), includeHeaders, columnsFor);
        }

        /// <summary>
        /// Materializes the output as a CSV formatted string.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <typeparam name="TParameter">The type of the t parameter.</typeparam>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="desiredColumns">The properties of this type will be used to determine the desired columns..</param>
        /// <param name="csvSerializer">Optional CSV serializer. Used to override how non-null values are converted into strings.</param>
        /// <param name="includeHeaders">if set to <c>true</c> [include headers].</param>
        /// <returns>ILink&lt;System.Int32&gt;.</returns>
        public static ILink<int> ToCsv<TCommand, TParameter>(this SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder, TextWriter outputStream, Type desiredColumns, CsvSerializer csvSerializer = null, bool includeHeaders = true)
where TCommand : DbCommand
where TParameter : DbParameter
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream), $"{nameof(outputStream)} is null.");
            if (desiredColumns == null)
                throw new ArgumentNullException(nameof(desiredColumns), $"{nameof(desiredColumns)} is null.");

            var columnsFor = MetadataCache.GetMetadata(desiredColumns).ColumnsFor;
            if (columnsFor.Length == 0)
                throw new MappingException($"Type {desiredColumns.Name} has no writable columns.");
            return new CsvStreamMaterializer<TCommand, TParameter>(commandBuilder, outputStream, csvSerializer ?? new CsvSerializer(), includeHeaders, columnsFor);
        }


    }
}


