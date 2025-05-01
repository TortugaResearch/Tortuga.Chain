using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// MasterDetailObjectMaterializer wraps MasterDetailCollectionMaterializer.
/// For a nullable version, use MasterDetailCollectionMaterializer directly.
/// </summary>
sealed partial class MasterDetailObjectMaterializer<TMaster> : IMasterDetailMaterializer<TMaster>

{
	IMasterDetailMaterializer<TMaster?> m_CommandBuilder;

	public MasterDetailObjectMaterializer(IMasterDetailMaterializer<TMaster?> commandBuilder)
	{
		m_CommandBuilder = commandBuilder;
	}

	event EventHandler<ExecutionTokenPreparedEventArgs>? ILink<TMaster>.ExecutionTokenPrepared
	{
		add => m_CommandBuilder.ExecutionTokenPrepared += value;

		remove => m_CommandBuilder.ExecutionTokenPrepared -= value;
	}

	event EventHandler<ExecutionTokenPreparingEventArgs>? ILink<TMaster>.ExecutionTokenPreparing
	{
		add => m_CommandBuilder.ExecutionTokenPreparing += value;

		remove => m_CommandBuilder.ExecutionTokenPreparing -= value;
	}

	IDataSource ILink<TMaster>.DataSource => m_CommandBuilder.DataSource;

	string? ILink<TMaster>.CommandText() => m_CommandBuilder.CommandText();

	/// <summary>
	/// Excludes the properties from the list of what will be populated in the object.
	/// </summary>
	/// <param name="propertiesToOmit">The properties to omit.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.ExceptProperties(params string[] propertiesToOmit)
	{
		m_CommandBuilder.ExceptProperties(propertiesToOmit);
		return this;
	}

	TMaster ILink<TMaster>.Execute(object? state)
	{
		return MasterDetailObjectMaterializer<TMaster>.ConvertResult(m_CommandBuilder.Execute(state));
	}

	async Task<TMaster> ILink<TMaster>.ExecuteAsync(object? state)
	{
		return MasterDetailObjectMaterializer<TMaster>.ConvertResult(await m_CommandBuilder.ExecuteAsync(state).ConfigureAwait(false));
	}

	async Task<TMaster> ILink<TMaster>.ExecuteAsync(CancellationToken cancellationToken, object? state)
	{
		return MasterDetailObjectMaterializer<TMaster>.ConvertResult(await m_CommandBuilder.ExecuteAsync(cancellationToken, state).ConfigureAwait(false));
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor(params Type[] constructorSignature)
	{
		m_CommandBuilder.WithDetailConstructor(constructorSignature);
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5, T6>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		m_CommandBuilder.WithDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor(params Type[] constructorSignature)
	{
		m_CommandBuilder.WithMasterConstructor(constructorSignature);
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5, T6>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
		return this;
	}

	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		m_CommandBuilder.WithMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });
		return this;
	}

	/// <summary>
	/// Limits the list of properties to populate to just the indicated list.
	/// </summary>
	/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TMaster> IMasterDetailMaterializer<TMaster>.WithProperties(params string[] propertiesToPopulate)
	{
		m_CommandBuilder.WithProperties(propertiesToPopulate);
		return this;
	}

	static TMaster ConvertResult(TMaster? result)
	{
		if (result == null)
			throw new MissingDataException("No records were returned");
		return result;
	}
}
