using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return multiple rows.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class MultipleRowDbCommandBuilder<TCommandType, TParameterType> : SingleRowDbCommandBuilder<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected MultipleRowDbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
            : base(dataSource)
        {

        }


        /// <summary>
        /// Indicates the results should be formatted as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public BooleanListResult<TCommandType, TParameterType> AsBooleanList(ListOptions listOptions = ListOptions.None) { return new BooleanListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Formats the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        public CollectionResult<TCommandType, TParameterType, TObject, List<TObject>> AsCollection<TObject>()
            where TObject : class, new()
        {
            return new CollectionResult<TCommandType, TParameterType, TObject, List<TObject>>(this);
        }

        /// <summary>
        /// Formats the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public CollectionResult<TCommandType, TParameterType, TObject, TCollection> AsCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
        {
            return new CollectionResult<TCommandType, TParameterType, TObject, TCollection>(this);
        }

        /// <summary>
        /// Indicates the results should be formatted as a DataSet.
        /// </summary>
        public DataTableResult<TCommandType, TParameterType> AsDataTableResult() { return new DataTableResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DateTimeListResult<TCommandType, TParameterType> AsDateTimeList(ListOptions listOptions = ListOptions.None) { return new DateTimeListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DateTimeOffsetListResult<TCommandType, TParameterType> AsDateTimeOffsetList(ListOptions listOptions = ListOptions.None) { return new DateTimeOffsetListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DecimalListResult<TCommandType, TParameterType> AsDecimalList(ListOptions listOptions = ListOptions.None) { return new DecimalListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DoubleListResult<TCommandType, TParameterType> AsDoubleList(ListOptions listOptions = ListOptions.None) { return new DoubleListResult<TCommandType, TParameterType>(this, listOptions); }
        /// <summary>
        /// Indicates the results should be formatted as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public GuidListResult<TCommandType, TParameterType> AsGuidList(ListOptions listOptions = ListOptions.None) { return new GuidListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int16ListResult<TCommandType, TParameterType> AsInt16List(ListOptions listOptions = ListOptions.None) { return new Int16ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int32ListResult<TCommandType, TParameterType> AsInt32List(ListOptions listOptions = ListOptions.None) { return new Int32ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int64ListResult<TCommandType, TParameterType> AsInt64List(ListOptions listOptions = ListOptions.None) { return new Int64ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public SingleListResult<TCommandType, TParameterType> AsSingleList(ListOptions listOptions = ListOptions.None) { return new SingleListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public StringListResult<TCommandType, TParameterType> AsStringList(ListOptions listOptions = ListOptions.None) { return new StringListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a Table.
        /// </summary>
        public TableResult<TCommandType, TParameterType> AsTable() { return new TableResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public TimeSpanListResult<TCommandType, TParameterType> AsTimeSpanList(ListOptions listOptions = ListOptions.None) { return new TimeSpanListResult<TCommandType, TParameterType>(this, listOptions); }
    }
}
