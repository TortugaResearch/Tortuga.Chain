using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.PostgreSql.CommandBuilders;

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase
	{
		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		public PostgreSqlInsertBulk InsertBulk(string tableName, DataTable dataTable)
		{
			return new PostgreSqlInsertBulk(this, new PostgreSqlObjectName(tableName), dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		public PostgreSqlInsertBulk InsertBulk(string tableName, IDataReader dataReader)
		{
			return new PostgreSqlInsertBulk(this, new PostgreSqlObjectName(tableName), dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public PostgreSqlInsertBulk InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new PostgreSqlInsertBulk(this, new PostgreSqlObjectName(tableName), new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		public ILink<long> GetTableApproximateCount(string tableName) => GetTableApproximateCount(new PostgreSqlObjectName(tableName));
	}
}
