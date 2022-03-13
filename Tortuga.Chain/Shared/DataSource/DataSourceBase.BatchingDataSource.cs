//using Tortuga.Chain.Materializers;


//#if SQL_SERVER_SDS || SQL_SERVER_MDS

//namespace Tortuga.Chain.SqlServer
//{
//	partial class SqlServerDataSourceBase
//	{

//#elif SQL_SERVER_OLEDB

//namespace Tortuga.Chain.SqlServer
//{
//	partial class OleDbSqlServerDataSourceBase
//	{

//#elif SQLITE

//namespace Tortuga.Chain.SQLite
//{
//	partial class SQLiteDataSourceBase
//	{

//#elif MYSQL

//namespace Tortuga.Chain.MySql
//{
//	partial class MySqlDataSourceBase
//	{

//#elif POSTGRESQL

//namespace Tortuga.Chain.PostgreSql
//{
//	partial class PostgreSqlDataSourceBase
//	{

//#elif ACCESS

//namespace Tortuga.Chain.Access
//{
//	partial class AccessDataSourceBase
//	{

//#endif
//#if !SQL_SERVER_OLEDB && !ACCESS

//		/// <summary>
//		/// Performs a series of batch inserts.
//		/// </summary>
//		/// <typeparam name="TObject">The type of the t object.</typeparam>
//		/// <param name="tableName">Name of the table.</param>
//		/// <param name="objects">The objects.</param>
//		/// <param name="options">The options.</param>
//		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
//		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
//		public ILink<int> InsertMultipleBatch<TObject>(string tableName, IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
//				where TObject : class
//		{
//			return InsertMultipleBatch(new AbstractObjectName(tableName), objects, options);
//		}

//		/// <summary>
//		/// Performs a series of batch inserts.
//		/// </summary>
//		/// <typeparam name="TObject">The type of the t object.</typeparam>
//		/// <param name="tableName">Name of the table.</param>
//		/// <param name="objects">The objects.</param>
//		/// <param name="options">The options.</param>
//		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
//		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
//		public ILink<int> InsertMultipleBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
//				where TObject : class
//		{
//			if (objects == null)
//				throw new ArgumentNullException(nameof(objects), $"{nameof(objects)} is null.");

//			var batchSize = MaxObjectsPerBatch(tableName, objects.First(), options);

//			Func<IEnumerable<TObject>, ILink<int>> callBack = (o) => (OnInsertBatch<TObject>(tableName, o, options)).AsNonQuery().NeverNull();

//			return CreateMultiBatcher(callBack, objects, batchSize);
//		}

//		/// <summary>
//		/// Inserts the batch of records as one operation.
//		/// </summary>
//		/// <typeparam name="TObject"></typeparam>
//		/// <param name="objects">The objects to insert.</param>
//		/// <param name="options">The options.</param>
//		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
//		public ILink<int> InsertMultipleBatch<TObject>(IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
//		where TObject : class
//		{
//			return InsertMultipleBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
//		}

//		/// <summary>
//		/// Inserts the batch of records as one operation.
//		/// </summary>
//		/// <typeparam name="TObject"></typeparam>
//		/// <param name="tableName">Name of the table.</param>
//		/// <param name="objects">The objects to insert.</param>
//		/// <param name="options">The options.</param>
//		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
//		public InsertBatchResult InsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
//		where TObject : class
//		{
//			return OnInsertBatch(tableName, objects, options);
//		}

//		/// <summary>
//		/// Inserts the batch of records as one operation.
//		/// </summary>
//		/// <typeparam name="TObject"></typeparam>
//		/// <param name="objects">The objects to insert.</param>
//		/// <param name="options">The options.</param>
//		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
//		public InsertBatchResult InsertBatch<TObject>(IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
//		where TObject : class
//		{
//			return InsertBatch<TObject>(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
//		}

//		int MaxObjectsPerBatch<TObject>(AbstractObjectName tableName, TObject sampleObject, InsertOptions options)
//	where TObject : class
//		{
//			var table = DatabaseMetadata.GetTableOrView(tableName);
//			var sqlBuilder = table.CreateSqlBuilder(false);
//			sqlBuilder.ApplyDesiredColumns(Materializer.NoColumns);
//			sqlBuilder.ApplyArgumentValue(this, sampleObject, options);
//			sqlBuilder.GetInsertColumns(options.HasFlag(InsertOptions.IdentityInsert)).Count(); //Call .Count() to trigger needed side-effects

//			var parametersPerRow = sqlBuilder.GetParameters().Count;

//			var maxParams = DatabaseMetadata.MaxParameters;
//			if (maxParams == null)
//				return int.MaxValue;

//			var maxRows = maxParams.Value / parametersPerRow;

//			if (DatabaseMetadata.MaxRowsPerValuesClause.HasValue)
//				maxRows = Math.Min(DatabaseMetadata.MaxRowsPerValuesClause.Value, maxRows);

//			return maxRows;
//		}

//		/// <summary>
//		/// Creates a multi-batcher. This is used by InsertMultipleBatch;
//		/// </summary>
//		/// <typeparam name="TObject">The type of the object.</typeparam>
//		/// <param name="callBack">The call back used to insert the batch.</param>
//		/// <param name="objects">The objects to insert.</param>
//		/// <param name="batchSize">Size of the batch.</param>
//		/// <returns>ILink&lt;System.Int32&gt;.</returns>
//		protected ILink<int> CreateMultiBatcher<TObject>(Func<IEnumerable<TObject>, ILink<int>> callBack, IEnumerable<TObject> objects, int batchSize)
//		{
//			return new MultiBatcher<TObject>(this, callBack, objects, batchSize);
//		}
//#endif
//	}
//}
