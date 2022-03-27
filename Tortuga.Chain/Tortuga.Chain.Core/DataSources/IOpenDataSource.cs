using System.Data.Common;

namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// This represents a data source that wraps a connection, and optional transaction, that it does not own.
	/// </summary>
	/// <remarks>
	/// This interface is primarily for testing purposes.
	/// </remarks>
	public interface IOpenDataSource
	{
		/// <summary>
		/// Returns the associated connection.
		/// </summary>
		DbConnection AssociatedConnection { get; }

		/// <summary>
		/// Returns the associated transaction.
		/// </summary>
		/// <returns></returns>
		DbTransaction? AssociatedTransaction { get; }

		/// <summary>
		/// Closes the connection and transaction associated with this data source.
		/// </summary>
		void Close();

		/// <summary>
		/// Tries the commit the transaction associated with this data source.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		bool TryCommit();

		/// <summary>
		/// Tries the rollback the transaction associated with this data source.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		bool TryRollback();
	}
}