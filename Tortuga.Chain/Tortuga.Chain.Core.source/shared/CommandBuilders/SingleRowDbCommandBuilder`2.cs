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
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected SingleRowDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<DataRow> ToDataRow(RowOptions rowOptions = RowOptions.None) => new DataRowMaterializer<TCommand, TParameter>(this, rowOptions);

        /// <summary>
        /// Materializes the result as a dynamic object
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<dynamic> ToDynamicObject(RowOptions rowOptions = RowOptions.None) => new DynamicObjectMaterializer<TCommand, TParameter>(this, rowOptions);

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class
        {
            return new ObjectMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<Row> ToRow(RowOptions rowOptions = RowOptions.None) => new RowMaterializer<TCommand, TParameter>(this, rowOptions);
    }
}