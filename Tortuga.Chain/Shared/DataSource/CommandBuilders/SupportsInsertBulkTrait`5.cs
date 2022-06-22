using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits
{
	[Trait]
	class SupportsInsertBulkTrait<TInsertBulkCommand, TCommand, TParameter, TObjectName, TDbType> : ISupportsInsertBulk
	where TInsertBulkCommand : Tortuga.Chain.CommandBuilders.DbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
	{

		[Container(RegisterInterface = true)]
		internal IInsertBulkHelper<TInsertBulkCommand, TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>TInsertBulkCommand.</returns>
		[Expose]
		public TInsertBulkCommand InsertBulk(TObjectName tableName, DataTable dataTable)
		{
			return DataSource.OnInsertBulk(tableName, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>TInsertBulkCommand.</returns>
		[Expose]
		public TInsertBulkCommand InsertBulk(TObjectName tableName, IDataReader dataReader)
		{
			return DataSource.OnInsertBulk(tableName, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>TInsertBulkCommand.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		[Expose]
		public TInsertBulkCommand InsertBulk<TObject>(TObjectName tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
			return DataSource.OnInsertBulk(tableName, new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataTable">The data table.</param>
		/// <returns>
		/// TInsertBulkCommand.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[Expose]
		public TInsertBulkCommand InsertBulk<TObject>(DataTable dataTable) where TObject : class
		{
			return DataSource.OnInsertBulk(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>
		/// TInsertBulkCommand.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[Expose]
		public TInsertBulkCommand InsertBulk<TObject>(IDataReader dataReader) where TObject : class
		{
			return DataSource.OnInsertBulk(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="objects">The objects.</param>
		/// <returns>
		/// TInsertBulkCommand.
		/// </returns>
		[Expose]
		public TInsertBulkCommand InsertBulk<TObject>(IEnumerable<TObject> objects) where TObject : class
		{
			return InsertBulk(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects);
		}

		ILink<int> ISupportsInsertBulk.InsertBulk(string tableName, DataTable dataTable)
		{
			return InsertBulk(DataSource.DatabaseMetadata.ParseObjectName(tableName), dataTable).AsNonQuery().NeverNull();
		}

		ILink<int> ISupportsInsertBulk.InsertBulk(string tableName, IDataReader dataReader)
		{
			return InsertBulk(DataSource.DatabaseMetadata.ParseObjectName(tableName), dataReader).AsNonQuery().NeverNull();
		}

		ILink<int> ISupportsInsertBulk.InsertBulk<TObject>(string tableName, IEnumerable<TObject> objects)
		{
			return InsertBulk(DataSource.DatabaseMetadata.ParseObjectName(tableName), objects).AsNonQuery().NeverNull();
		}

		ILink<int> ISupportsInsertBulk.InsertBulk<TObject>(DataTable dataTable)
		{
			return InsertBulk<TObject>(dataTable).AsNonQuery().NeverNull();
		}

		ILink<int> ISupportsInsertBulk.InsertBulk<TObject>(IDataReader dataReader)
		{
			return InsertBulk<TObject>(dataReader).AsNonQuery().NeverNull();
		}

		ILink<int> ISupportsInsertBulk.InsertBulk<TObject>(IEnumerable<TObject> objects)
		{
			return InsertBulk(objects).AsNonQuery().NeverNull();
		}
	}
}



