using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Metadata;
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
		public MySqlInsertBulk InsertBulk(MySqlObjectName tableName, DataTable dataTable)
		{
			return new MySqlInsertBulk(this, tableName, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>MySqlInsertBulk.</returns>
		public MySqlInsertBulk InsertBulk(MySqlObjectName tableName, IDataReader dataReader)
		{
			return new MySqlInsertBulk(this, tableName, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>MySqlInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public MySqlInsertBulk InsertBulk<TObject>(MySqlObjectName tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new MySqlInsertBulk(this, tableName, new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataTable">The data table.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public MySqlInsertBulk InsertBulk<TObject>(DataTable dataTable) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public MySqlInsertBulk InsertBulk<TObject>(IDataReader dataReader) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="objects">The objects.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		public MySqlInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects);
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		[SuppressMessage("Globalization", "CA1305")]
		public ILink<long> GetTableApproximateCount(MySqlObjectName tableName)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName).Name; //get the real name
			var sql = $@"SHOW TABLE STATUS IN {table.Schema} like '{table.Name}'";
			return Sql(sql).ToRow().Transform(row => Convert.ToInt64(row["Rows"]));
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name);


	}
}
