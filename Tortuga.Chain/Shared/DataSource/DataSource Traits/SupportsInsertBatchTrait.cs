using System.Data.Common;
using Tortuga.Anchor;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Shipwright;

namespace Traits;


[Trait]
class SupportsInsertBatchTrait<TCommand, TParameter, TObjectName, TDbType, TResult> : ISupportsInsertBatch
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
	where TResult : DbCommandBuilder<TCommand, TParameter>
{

	//[Owner]
	//public IDataSource DataSource { get; set; } = null!;

	//[Partial]
	//public Func<DatabaseMetadataCache<TObjectName, TDbType>> OnGetDatabaseMetadata2 { get; set; } = null!;
	//[Partial("builder")]
	//public Func<SqlBuilder<TDbType>, List<TParameter>> OnGetParameters { get; set; } = null!;

	//[Partial("objectName")] public Func<string, TObjectName> OnParseObjectName { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal IInsertBatchHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IDbCommandBuilder ISupportsInsertBatch.InsertBatch<TObject>(IEnumerable<TObject> objects, InsertOptions options)
	{
		return InsertBatch(objects, options);
	}

	/// <summary>
	/// Inserts the batch of records as one operation.
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="objects">The objects to insert.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
	[Expose]
	public TResult InsertBatch<TObject>(TObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
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
	[Expose]
	public TResult InsertBatch<TObject>(IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
	where TObject : class
	{
		return InsertBatch<TObject>(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
	}

	ILink<int> ISupportsInsertBatch.InsertMultipleBatch<TObject>(string tableName, IReadOnlyList<TObject> objects, InsertOptions options)
	{
		return InsertMultipleBatch(DataSource.ParseObjectName(tableName), objects, options);
	}

	ILink<int> ISupportsInsertBatch.InsertMultipleBatch<TObject>(IReadOnlyList<TObject> objects, InsertOptions options)
	{
		return InsertMultipleBatch<TObject>(objects, options);
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
	[Expose]
	public ILink<int> InsertMultipleBatch<TObject>(TObjectName tableName, IEnumerable<TObject> objects, InsertOptions options = InsertOptions.None)
			where TObject : class
	{
		if (objects == null)
			throw new ArgumentNullException(nameof(objects), $"{nameof(objects)} is null.");

		var batchSize = MaxObjectsPerBatch(tableName, objects.First(), options);

		Func<IEnumerable<TObject>, ILink<int>> callBack = (o) => (OnInsertBatch<TObject>(tableName, o, options)).AsNonQuery().NeverNull();

		return new MultiBatcher<TObject>(DataSource, callBack, objects, batchSize);
	}

	/// <summary>
	/// Inserts the batch of records as one operation.
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <param name="objects">The objects to insert.</param>
	/// <param name="options">The options.</param>
	/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
	[Expose]
	public ILink<int> InsertMultipleBatch<TObject>(IReadOnlyList<TObject> objects, InsertOptions options = InsertOptions.None)
	where TObject : class
	{
		return InsertMultipleBatch(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
	}
	int MaxObjectsPerBatch<TObject>(TObjectName tableName, TObject sampleObject, InsertOptions options)
		where TObject : class
	{
		var metadata = DataSource.DatabaseMetadata;

		var table = metadata.GetTableOrView(tableName);
		var sqlBuilder = table.CreateSqlBuilder(false);
		sqlBuilder.ApplyDesiredColumns(Materializer.NoColumns);
		sqlBuilder.ApplyArgumentValue(DataSource, sampleObject, options);
		sqlBuilder.GetInsertColumns(options.HasFlag(InsertOptions.IdentityInsert)).Count(); //Call .Count() to trigger needed side-effects

		var parametersPerRow = DataSource.GetParameters(sqlBuilder).Count;

		var maxParams = metadata.MaxParameters;
		if (maxParams == null)
			return int.MaxValue;

		var maxRows = maxParams.Value / parametersPerRow;

		if (metadata.MaxRowsPerValuesClause.HasValue)
			maxRows = Math.Min(metadata.MaxRowsPerValuesClause.Value, maxRows);

		return maxRows;
	}
	TResult OnInsertBatch<TObject>(TObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
where TObject : class
	{
		return (TResult)DataSource.OnInsertBatch(tableName, objects, options);
	}


	/// <summary>
	/// MultiBatcher is used by InsertMultipleBatch to perform a series of batch inserts.
	/// </summary>
	class MultiBatcher<TObject> : ILink<int>
	{
		int m_BatchSize;

		Func<IReadOnlyList<TObject>, ILink<int>> m_CallBack;

		IEnumerable<TObject> m_Objects;

		internal MultiBatcher(IDataSource dataSource, Func<IEnumerable<TObject>, ILink<int>> callBack, IEnumerable<TObject> objects, int batchSize)
		{
			DataSource = dataSource;
			m_CallBack = callBack;
			m_Objects = objects;
			m_BatchSize = batchSize;
		}

		public event EventHandler<ExecutionTokenPreparedEventArgs>? ExecutionTokenPrepared;

		public event EventHandler<ExecutionTokenPreparingEventArgs>? ExecutionTokenPreparing;

		public IDataSource DataSource { get; }

		string? ILink<int>.CommandText()
		{
			var result = new System.Text.StringBuilder();
			foreach (var batch in m_Objects.BatchAsLists(m_BatchSize))
			{
				ILink<int> link = m_CallBack.Invoke(batch);
				link.ExecutionTokenPrepared += OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing += OnExecutionTokenPreparing;
				result.AppendLine(link.CommandText());
				link.ExecutionTokenPrepared -= OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing -= OnExecutionTokenPreparing;
			}

			return result.ToString();
		}

		int ILink<int>.Execute(object? state)
		{
			var result = 0;
			foreach (var batch in m_Objects.BatchAsLists(m_BatchSize))
			{
				ILink<int> link = m_CallBack.Invoke(batch);
				link.ExecutionTokenPrepared += OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing += OnExecutionTokenPreparing;
				result += link.Execute(state);
				link.ExecutionTokenPrepared -= OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing -= OnExecutionTokenPreparing;
			}

			return result;
		}

		Task<int> ILink<int>.ExecuteAsync(object? state)
		{
			return ((ILink<int>)this).ExecuteAsync(CancellationToken.None, state);
		}

		async Task<int> ILink<int>.ExecuteAsync(CancellationToken cancellationToken, object? state)
		{
			var result = 0;
			foreach (var batch in m_Objects.BatchAsLists(m_BatchSize))
			{
				ILink<int> link = m_CallBack.Invoke(batch);
				link.ExecutionTokenPrepared += OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing += OnExecutionTokenPreparing;
				result += await link.ExecuteAsync(cancellationToken, state).ConfigureAwait(false);
				link.ExecutionTokenPrepared -= OnExecutionTokenPrepared;
				link.ExecutionTokenPreparing -= OnExecutionTokenPreparing;
			}

			return result;
		}



		void OnExecutionTokenPrepared(object? sender, ExecutionTokenPreparedEventArgs e)
		{
			ExecutionTokenPrepared?.Invoke(sender, e);
		}

		void OnExecutionTokenPreparing(object? sender, ExecutionTokenPreparingEventArgs e)
		{
			ExecutionTokenPreparing?.Invoke(sender, e);
		}
	}
}