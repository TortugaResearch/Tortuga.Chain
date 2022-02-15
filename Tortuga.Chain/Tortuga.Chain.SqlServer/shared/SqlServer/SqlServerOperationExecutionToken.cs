using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tortuga.Chain.SqlServer
{
	/// <summary>
	/// Class SqlServerOperationExecutionToken.
	/// </summary>
	/// <seealso cref="OperationExecutionToken{SqlConnection, SqlTransaction}" />
	public class SqlServerOperationExecutionToken : OperationExecutionToken<SqlConnection, SqlTransaction>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerOperationExecutionToken"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="operationName">Name of the operation. This is used for logging.</param>
		public SqlServerOperationExecutionToken(IOperationDataSource<SqlConnection, SqlTransaction> dataSource, string operationName) : base(dataSource, operationName)
		{
		}
	}
}
