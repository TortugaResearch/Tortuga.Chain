using System;
using System.Data;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return one row.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class SingleRowDbCommandBuilder<TCommand, TParameter> : ScalarDbCommandBuilder<TCommand, TParameter>, ISingleRowDbCommandBuilder
    where TCommand : DbCommand
    where TParameter : DbParameter
    {
        /// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
        /// <param name="dataSource">The data source.</param>
        protected SingleRowDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<DataRow> ToDataRow(RowOptions rowOptions = RowOptions.None)
        {
            if (rowOptions.HasFlag(RowOptions.AllowEmptyResults))
                throw new ArgumentException($"Cannot use {nameof(RowOptions)}.{nameof(RowOptions.AllowEmptyResults)} with {nameof(ToDataRow)}. Use {nameof(ToDataRowOrNull)} instead.", nameof(rowOptions));

            return new DataRowMaterializer<TCommand, TParameter>(this, rowOptions).NeverNull();
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<DataRow?> ToDataRowOrNull(RowOptions rowOptions = RowOptions.AllowEmptyResults) => new DataRowMaterializer<TCommand, TParameter>(this, rowOptions);

        /// <summary>
        /// Materializes the result as a dynamic object
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<dynamic> ToDynamicObject(RowOptions rowOptions = RowOptions.None)
        {
            if (rowOptions.HasFlag(RowOptions.AllowEmptyResults))
                throw new ArgumentException($"Cannot use {nameof(RowOptions)}.{nameof(RowOptions.AllowEmptyResults)} with {nameof(ToDynamicObject)}. Use {nameof(ToDynamicObjectOrNull)} instead.", nameof(rowOptions));

            return new DynamicObjectMaterializer<TCommand, TParameter>(this, rowOptions).NeverNull();
        }

        /// <summary>
        /// Materializes the result as a dynamic object
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<dynamic?> ToDynamicObjectOrNull(RowOptions rowOptions = RowOptions.AllowEmptyResults) => new DynamicObjectMaterializer<TCommand, TParameter>(this, rowOptions);

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class
        {
            if (rowOptions.HasFlag(RowOptions.AllowEmptyResults))
                throw new ArgumentException($"Cannot use {nameof(RowOptions)}.{nameof(RowOptions.AllowEmptyResults)} with {nameof(ToObject)}. Use {nameof(ToObjectOrNull)} instead.", nameof(rowOptions));

            return new ObjectMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<TObject?> ToObjectOrNull<TObject>(RowOptions rowOptions = RowOptions.AllowEmptyResults)
            where TObject : class
        {
            return new ObjectOrNullMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<Row> ToRow(RowOptions rowOptions = RowOptions.None)
        {
            if (rowOptions.HasFlag(RowOptions.AllowEmptyResults))
                throw new ArgumentException($"Cannot use {nameof(RowOptions)}.{nameof(RowOptions.AllowEmptyResults)} with {nameof(ToRow)}. Use {nameof(ToRowOrNull)} instead.", nameof(rowOptions));

            return new RowMaterializer<TCommand, TParameter>(this, rowOptions).NeverNull();
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<Row?> ToRowOrNull(RowOptions rowOptions = RowOptions.AllowEmptyResults) => new RowMaterializer<TCommand, TParameter>(this, rowOptions);
    }
}
