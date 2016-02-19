using System;
namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerTableOrView.
    /// </summary>
    public class SqlServerTableOrView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="filterValue">The filter value.</param>
        public SqlServerTableOrView(SqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, object filterValue)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableOrView"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="argumentValue">The argument value.</param>
        public SqlServerTableOrView(SqlServerDataSourceBase dataSource, SqlServerObjectName tableOrViewName, string whereClause, object argumentValue)
        {
            throw new NotImplementedException();
        }
    }
}