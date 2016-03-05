using System;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerTableFunction.
    /// </summary>
    public class SqlServerTableFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableFunction"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dataSource")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "whereClause")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "tableFunctionName")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentValue")]
        public SqlServerTableFunction(SqlServerDataSourceBase dataSource, SqlServerObjectName tableFunctionName, string whereClause, object argumentValue)
        {
            throw new NotImplementedException();
        }
    }
}