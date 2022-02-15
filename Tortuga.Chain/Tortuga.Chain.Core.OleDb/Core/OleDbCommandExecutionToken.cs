using System.Data.OleDb;
using Tortuga.Chain.DataSources;



namespace Tortuga.Chain.Core
{
	/// <summary>
	/// An execution token for any OleDb provider.
	/// </summary>
	/// <seealso cref="CommandExecutionToken{OleDbCommand, OleDbParameter}" />
	public sealed class OleDbCommandExecutionToken : CommandExecutionToken<OleDbCommand, OleDbParameter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbCommandExecutionToken"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="operationName">Name of the operation. This is used for logging.</param>
		/// <param name="commandText">The SQL to be executed.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="commandType">Type of the command.</param>
		public OleDbCommandExecutionToken(ICommandDataSource<OleDbCommand, OleDbParameter> dataSource, string operationName, string commandText, IReadOnlyList<OleDbParameter> parameters, CommandType commandType = CommandType.Text)
			: base(dataSource, operationName, commandText, parameters, commandType)
		{
		}
	}
}
