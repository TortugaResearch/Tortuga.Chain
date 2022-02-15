using System.Data.OleDb;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Access
{

	/// <summary>
	/// Class AccessExecutionToken.
	/// </summary>
	public sealed class AccessOperationExecutionToken : OperationExecutionToken<OleDbConnection, OleDbTransaction>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccessOperationExecutionToken" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="operationName">Name of the operation. This is used for logging.</param>
		public AccessOperationExecutionToken(IOperationDataSource<OleDbConnection, OleDbTransaction> dataSource, string operationName)
			: base(dataSource, operationName)
		{

		}



	}
}
