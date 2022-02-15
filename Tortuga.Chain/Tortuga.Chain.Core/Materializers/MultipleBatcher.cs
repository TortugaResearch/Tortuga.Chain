using Tortuga.Anchor;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.CommandBuilders
{
	/// <summary>
	/// MultiBatcher is used by InsertMultipleBatch to perform a series of batch inserts.
	/// </summary>
	internal class MultiBatcher<TObject> : ILink<int>
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
