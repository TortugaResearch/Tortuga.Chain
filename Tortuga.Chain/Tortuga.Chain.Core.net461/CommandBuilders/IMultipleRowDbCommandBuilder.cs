using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
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
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<bool>> ToBooleanList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<bool>> ToBooleanList(ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of byte arrays.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<byte[]>> ToByteArrayList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of byte arrays.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<byte[]>> ToByteArrayList(ListOptions listOptions = ListOptions.None);


        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        ILink<List<TObject>> ToCollection<TObject>()
            where TObject : class, new();
        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TCollection> ToCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new();

#if !WINDOWS_UWP
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        ILink<DataTable> ToDataTable();
#endif
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTime>> ToDateTimeList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTime>> ToDateTimeList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTimeOffset>> ToDateTimeOffsetList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<DateTimeOffset>> ToDateTimeOffsetList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<decimal>> ToDecimalList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<decimal>> ToDecimalList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<double>> ToDoubleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<double>> ToDoubleList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<Guid>> ToGuidList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<Guid>> ToGuidList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<short>> ToInt16List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<short>> ToInt16List(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<int>> ToInt32List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<int>> ToInt32List(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<long>> ToInt64List(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<long>> ToInt64List(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<float>> ToSingleList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<float>> ToSingleList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<string>> ToStringList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<string>> ToStringList(string columnName, ListOptions listOptions = ListOptions.None);

        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        ILink<Table> ToTable();
        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<TimeSpan>> ToTimeSpanList(ListOptions listOptions = ListOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        ILink<List<TimeSpan>> ToTimeSpanList(string columnName, ListOptions listOptions = ListOptions.None);
    }
}
