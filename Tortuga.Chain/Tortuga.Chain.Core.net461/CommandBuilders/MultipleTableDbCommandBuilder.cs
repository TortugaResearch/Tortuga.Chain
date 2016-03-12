using System.Data;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return multiple result sets.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class MultipleTableDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected MultipleTableDbCommandBuilder(DataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        { }


        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        public ILink<DataSet> ToDataSet(params string[] tableNames) { return new DataSetMaterializer<TCommand, TParameter>(this, tableNames); }

        /// <summary>
        /// Indicates the results should be materialized as a set of tables.
        /// </summary>
        public ILink<TableSet> ToTableSet(params string[] tableNames) { return new TableSetMaterializer<TCommand, TParameter>(this, tableNames); }

    }
}
