#if !WINDOWS_UWP
using System.Data;
#endif

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This command builder may return multiple recordsets
    /// </summary>
    public interface IMultipleTableDbCommandBuilder : IMultipleRowDbCommandBuilder
    {
#if !WINDOWS_UWP
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        ILink<DataSet> ToDataSet(params string[] tableNames);
#endif
        /// <summary>
        /// Indicates the results should be materialized as a set of tables.
        /// </summary>
        ILink<TableSet> ToTableSet(params string[] tableNames);
    }
}
