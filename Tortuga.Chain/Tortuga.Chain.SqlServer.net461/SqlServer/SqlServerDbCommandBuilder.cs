using System.Data.SqlClient;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerDbCommandBuilder.
    /// </summary>
    public abstract class SqlServerDbCommandBuilder : DbCommandBuilder<SqlCommand, SqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDbCommandBuilder"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected SqlServerDbCommandBuilder(SqlServerDataSourceBase dataSource)
            : base(dataSource)
        {

        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        protected new SqlServerDataSourceBase DataSource
        {
            get { return (SqlServerDataSourceBase)base.DataSource; }
        }

    }
}
