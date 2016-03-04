using System;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This allows the use of multi-row materializers against a command builder.
    /// </summary>
    public interface IMultipleRowDbCommandBuilder : ISingleRowDbCommandBuilder
    {
        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<bool>> AsBooleanList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        IMaterializer<List<TObject>> AsCollection<TObject>()
            where TObject : class, new();
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IMaterializer<TCollection> AsCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new();
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        IMaterializer<DataTable> AsDataTable();
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<DateTime>> AsDateTimeList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<DateTimeOffset>> AsDateTimeOffsetList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<decimal>> AsDecimalList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<double>> AsDoubleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<Guid>> AsGuidList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<short>> AsInt16List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<int>> AsInt32List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<long>> AsInt64List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<float>> AsSingleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<string>> AsStringList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        IMaterializer<Table> AsTable();
        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        IMaterializer<List<TimeSpan>> AsTimeSpanList(ListOptions listOptions = ListOptions.None);
    }
}
