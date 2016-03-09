using System.Data;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return multiple result sets.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class MultipleTableDbCommandBuilder<TCommandType, TParameterType> : MultipleRowDbCommandBuilder<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected MultipleTableDbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
            : base(dataSource)
        { }


        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        public ILink<DataSet> AsDataSet(params string[] tableNames) { return new DataSetMaterializer<TCommandType, TParameterType>(this, tableNames); }

        /// <summary>
        /// Indicates the results should be materialized as a set of tables.
        /// </summary>
        public ILink<TableSet> AsTableSet(params string[] tableNames) { return new TableSetMaterializer<TCommandType, TParameterType>(this, tableNames); }

    }
}
