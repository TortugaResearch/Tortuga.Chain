namespace Tortuga.Chain.Materializers;

partial class MasterDetailCollectionMaterializer<TCommand, TParameter, TMaster, TDetail> : IMasterDetailMaterializer<TMaster?>

{
	/// <summary>
	/// Excludes the properties from the list of what will be populated in the object.
	/// </summary>
	/// <param name="propertiesToOmit">The properties to omit.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.ExceptProperties(params string[] propertiesToOmit)
	{
		ExceptProperties(propertiesToOmit);
		return this;
	}

	TMaster? ILink<TMaster?>.Execute(object? state)
	{
		return ConvertResult(Execute(state));
	}

	async Task<TMaster?> ILink<TMaster?>.ExecuteAsync(object? state)
	{
		return ConvertResult(await ExecuteAsync(state).ConfigureAwait(false));
	}

	async Task<TMaster?> ILink<TMaster?>.ExecuteAsync(CancellationToken cancellationToken, object? state)
	{
		return ConvertResult(await ExecuteAsync(cancellationToken, state).ConfigureAwait(false));
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor(params Type[] constructorSignature)
	{
		SetDetailConstructor(constructorSignature);
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1>()
	{
		SetDetailConstructor(new[] { typeof(T1) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5, T6>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		SetDetailConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor(params Type[] constructorSignature)
	{
		SetMasterConstructor(constructorSignature);
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1>()
	{
		SetMasterConstructor(new[] { typeof(T1) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5, T6>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
		return this;
	}

	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		SetMasterConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });
		return this;
	}

	/// <summary>
	/// Limits the list of properties to populate to just the indicated list.
	/// </summary>
	/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TMaster?> IMasterDetailMaterializer<TMaster?>.WithProperties(params string[] propertiesToPopulate)
	{
		WithProperties(propertiesToPopulate);
		return this;
	}

	TMaster? ConvertResult(List<TMaster> result)
	{
		if (result.Count == 0 && m_PreventEmptyResults)
			throw new MissingDataException("No records were returned");

		if (result.Count > 1 && !m_DiscardExtraRows)
			throw new UnexpectedDataException("More records were returned than expected");

		return result[0];
	}
}
