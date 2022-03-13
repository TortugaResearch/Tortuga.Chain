using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// Used to mark data sources that support inserting multiple rows at once.
	/// </summary>
	public interface ISupportsInsertBatch
	{
		/// <summary>
		/// Performs a series of batch inserts.
		/// </summary>
		/// <typeparam name="TObject">The type of the t object.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
		ILink<int> InsertMultipleBatch<TObject>(string tableName, IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
				where TObject : class;

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		ILink<int> InsertMultipleBatch<TObject>(IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
		where TObject : class;

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		IMultipleRowDbCommandBuilder InsertBatch<TObject>(IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
		where TObject : class;

	}
}