using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.MySql.CommandBuilders;

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase
	{
		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>MySqlInsertBulk.</returns>
		public MySqlInsertBulk InsertBulk(string tableName, DataTable dataTable)
		{
			return new MySqlInsertBulk(this, new MySqlObjectName(tableName), dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>MySqlInsertBulk.</returns>
		public MySqlInsertBulk InsertBulk(string tableName, IDataReader dataReader)
		{
			return new MySqlInsertBulk(this, new MySqlObjectName(tableName), dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>MySqlInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public MySqlInsertBulk InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new MySqlInsertBulk(this, new MySqlObjectName(tableName), new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		public ILink<long> GetTableApproximateCount(string tableName) => GetTableApproximateCount(new MySqlObjectName(tableName));
	}
}
