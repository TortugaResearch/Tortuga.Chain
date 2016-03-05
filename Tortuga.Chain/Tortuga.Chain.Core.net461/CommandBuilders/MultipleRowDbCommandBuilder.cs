using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{

    /// <summary>
    /// This is the base class for command builders that can potentially return multiple rows.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class MultipleRowDbCommandBuilder<TCommandType, TParameterType> : SingleRowDbCommandBuilder<TCommandType, TParameterType>, IMultipleRowDbCommandBuilder
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
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<bool>> AsBooleanList(ListOptions listOptions = ListOptions.None) { return new BooleanListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        public ILink<List<TObject>> AsCollection<TObject>()
            where TObject : class, new()
        {
            return new CollectionMaterializer<TCommandType, TParameterType, TObject, List<TObject>>(this);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ILink<TCollection> AsCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
        {
            return new CollectionMaterializer<TCommandType, TParameterType, TObject, TCollection>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        public ILink<DataTable> AsDataTable() { return new DataTableMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTime>> AsDateTimeList(ListOptions listOptions = ListOptions.None) { return new DateTimeListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTimeOffset>> AsDateTimeOffsetList(ListOptions listOptions = ListOptions.None) { return new DateTimeOffsetListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<decimal>> AsDecimalList(ListOptions listOptions = ListOptions.None) { return new DecimalListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<double>> AsDoubleList(ListOptions listOptions = ListOptions.None) { return new DoubleListMaterializer<TCommandType, TParameterType>(this, listOptions); }
        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<Guid>> AsGuidList(ListOptions listOptions = ListOptions.None) { return new GuidListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<short>> AsInt16List(ListOptions listOptions = ListOptions.None) { return new Int16ListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<int>> AsInt32List(ListOptions listOptions = ListOptions.None) { return new Int32ListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<long>> AsInt64List(ListOptions listOptions = ListOptions.None) { return new Int64ListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<float>> AsSingleList(ListOptions listOptions = ListOptions.None) { return new SingleListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<string>> AsStringList(ListOptions listOptions = ListOptions.None) { return new StringListMaterializer<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        public ILink<Table> AsTable() { return new TableMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<TimeSpan>> AsTimeSpanList(ListOptions listOptions = ListOptions.None) { return new TimeSpanListMaterializer<TCommandType, TParameterType>(this, listOptions); }
    }
}
