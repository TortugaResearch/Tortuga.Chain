using Tortuga.Chain.Materializers;

#if SQL_SERVER_SDS

using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractParameter = System.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<System.Data.SqlClient.SqlCommand, System.Data.SqlClient.SqlParameter>;

#elif SQL_SERVER_MDS

using AbstractCommand = Microsoft.Data.SqlClient.SqlCommand;
using AbstractParameter = Microsoft.Data.SqlClient.SqlParameter;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<Microsoft.Data.SqlClient.SqlCommand, Microsoft.Data.SqlClient.SqlParameter>;

#elif SQL_SERVER_OLEDB

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.SqlServerLimitOption;
using AbstractObjectName = Tortuga.Chain.SqlServer.SqlServerObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#elif SQLITE

using AbstractCommand = System.Data.SQLite.SQLiteCommand;
using AbstractParameter = System.Data.SQLite.SQLiteParameter;
using AbstractObjectName = Tortuga.Chain.SQLite.SQLiteObjectName;
using AbstractLimitOption = Tortuga.Chain.SQLiteLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.DbCommandBuilder<System.Data.SQLite.SQLiteCommand, System.Data.SQLite.SQLiteParameter>;

#elif MYSQL

using AbstractCommand = MySqlConnector.MySqlCommand;
using AbstractParameter = MySqlConnector.MySqlParameter;
using AbstractObjectName = Tortuga.Chain.MySql.MySqlObjectName;
using AbstractLimitOption = Tortuga.Chain.MySqlLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.DbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter>;

#elif POSTGRESQL

using AbstractCommand = Npgsql.NpgsqlCommand;
using AbstractParameter = Npgsql.NpgsqlParameter;
using AbstractObjectName = Tortuga.Chain.PostgreSql.PostgreSqlObjectName;
using AbstractLimitOption = Tortuga.Chain.PostgreSqlLimitOption;
using InsertBatchResult = Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<Npgsql.NpgsqlCommand, Npgsql.NpgsqlParameter>;

#elif ACCESS

using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractLimitOption = Tortuga.Chain.AccessLimitOption;
using AbstractObjectName = Tortuga.Chain.Access.AccessObjectName;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerDataSourceBase
    {

#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerDataSourceBase
    {

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteDataSourceBase
    {

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase
	{

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlDataSourceBase
    {

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessDataSourceBase
    {

#endif
#if !SQL_SERVER_OLEDB && !ACCESS

		/// <summary>
		/// Performs a series of batch inserts.
		/// </summary>
		/// <typeparam name="TObject">The type of the t object.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
		public ILink<int> InsertMultipleBatch<TObject>(string tableName, IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
				where TObject : class
		{
			return InsertMultipleBatch(new AbstractObjectName(tableName), objects, options);
		}

		/// <summary>
		/// Performs a series of batch inserts.
		/// </summary>
		/// <typeparam name="TObject">The type of the t object.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
		public ILink<int> InsertMultipleBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
				where TObject : class
		{
			if (objects == null)
				throw new ArgumentNullException(nameof(objects), $"{nameof(objects)} is null.");

			var batchSize = MaxObjectsPerBatch(tableName, objects.First(), options);

			Func<IEnumerable<TObject>, ILink<int>> callBack = (o) => (OnInsertBatch<TObject>(tableName, o, options)).AsNonQuery().NeverNull();

			return CreateMultiBatcher(callBack, objects, batchSize);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public ILink<int> InsertMultipleBatch<TObject>(IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
		where TObject : class
		{
			return InsertMultipleBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public InsertBatchResult InsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
		where TObject : class
		{
			return OnInsertBatch(tableName, objects, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public InsertBatchResult InsertBatch<TObject>(IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
		where TObject : class
		{
			return InsertBatch<TObject>(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
		}

		int MaxObjectsPerBatch<TObject>(AbstractObjectName tableName, TObject sampleObject, InsertOptions options)
	where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			var sqlBuilder = table.CreateSqlBuilder(false);
			sqlBuilder.ApplyDesiredColumns(Materializer.NoColumns);
			sqlBuilder.ApplyArgumentValue(this, sampleObject, options);
			sqlBuilder.GetInsertColumns(options.HasFlag(InsertOptions.IdentityInsert)).Count(); //Call .Count() to trigger needed side-effects

			var parametersPerRow = sqlBuilder.GetParameters().Count;

			var maxParams = DatabaseMetadata.MaxParameters;
			if (maxParams == null)
				return int.MaxValue;

			var maxRows = maxParams.Value / parametersPerRow;

#if SQL_SERVER_SDS || SQL_SERVER_MDS
            //Max rows per VALUES clause is 1000.
            maxRows = Math.Min(1000, maxRows);
#endif
			return maxRows;
		}

#endif
	}
}
