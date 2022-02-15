using System.Data.OleDb;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Core
{
	/// <summary>
	/// Class OleDbOperationExecutionToken.
	/// </summary>
	/// <seealso cref="OperationExecutionToken{OleDbConnection, OleDbTransaction}" />
	public class OleDbOperationExecutionToken : OperationExecutionToken<OleDbConnection, OleDbTransaction>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbOperationExecutionToken" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="operationName">Name of the operation. This is used for logging.</param>
		public OleDbOperationExecutionToken(IOperationDataSource<OleDbConnection, OleDbTransaction> dataSource, string operationName) : base(dataSource, operationName)
		{
		}
	}
}
