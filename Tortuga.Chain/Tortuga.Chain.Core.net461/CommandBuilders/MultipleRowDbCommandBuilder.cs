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
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class MultipleRowDbCommandBuilder<TCommand, TParameter> : SingleRowDbCommandBuilder<TCommand, TParameter>, IMultipleRowDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected MultipleRowDbCommandBuilder(DataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {

        }


        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<bool>> ToBooleanList(ListOptions listOptions = ListOptions.None) { return new BooleanListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        public ILink<List<TObject>> ToCollection<TObject>()
            where TObject : class, new()
        {
            return new CollectionMaterializer<TCommand, TParameter, TObject, List<TObject>>(this);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ILink<TCollection> ToCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
        {
            return new CollectionMaterializer<TCommand, TParameter, TObject, TCollection>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        public ILink<DataTable> ToDataTable() { return new DataTableMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTime>> ToDateTimeList(ListOptions listOptions = ListOptions.None) { return new DateTimeListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTimeOffset>> ToDateTimeOffsetList(ListOptions listOptions = ListOptions.None) { return new DateTimeOffsetListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<decimal>> ToDecimalList(ListOptions listOptions = ListOptions.None) { return new DecimalListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<double>> ToDoubleList(ListOptions listOptions = ListOptions.None) { return new DoubleListMaterializer<TCommand, TParameter>(this, listOptions); }
        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<Guid>> ToGuidList(ListOptions listOptions = ListOptions.None) { return new GuidListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<short>> ToInt16List(ListOptions listOptions = ListOptions.None) { return new Int16ListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<int>> ToInt32List(ListOptions listOptions = ListOptions.None) { return new Int32ListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<long>> ToInt64List(ListOptions listOptions = ListOptions.None) { return new Int64ListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<float>> ToSingleList(ListOptions listOptions = ListOptions.None) { return new SingleListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<string>> ToStringList(ListOptions listOptions = ListOptions.None) { return new StringListMaterializer<TCommand, TParameter>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        public ILink<Table> ToTable() { return new TableMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<TimeSpan>> ToTimeSpanList(ListOptions listOptions = ListOptions.None) { return new TimeSpanListMaterializer<TCommand, TParameter>(this, listOptions); }
    }
}
