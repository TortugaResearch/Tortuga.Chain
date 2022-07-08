using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

sealed partial class MasterDetailCollectionMaterializer<TCommand, TParameter, TMaster, TDetail> : Materializer<TCommand, TParameter, List<TMaster>>

	where TCommand : DbCommand
	where TParameter : DbParameter
	where TMaster : class
	where TDetail : class

{
	static readonly ClassMetadata s_DetailMetadata = MetadataCache.GetMetadata<TDetail>();
	static readonly ClassMetadata s_MasterMetadata = MetadataCache.GetMetadata<TMaster>();

	readonly bool m_DiscardExtraRows;
	readonly Func<TMaster, ICollection<TDetail>> m_Map;
	readonly string m_MasterKeyColumn;
	readonly bool m_PreventEmptyResults;

	ConstructorMetadata? m_DetailConstructor;
	IReadOnlyList<string>? m_ExcludedColumns;
	IReadOnlyList<string>? m_IncludedColumns;
	ConstructorMetadata? m_MasterConstructor;

	public MasterDetailCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string masterKeyColumn, Func<TMaster, ICollection<TDetail>> map, CollectionOptions masterOptions, CollectionOptions detailOptions) : base(commandBuilder)
	{
		m_MasterKeyColumn = masterKeyColumn;
		m_Map = map;

		if (masterOptions.HasFlag(CollectionOptions.InferConstructor))
			m_MasterConstructor = MaterializerUtilities.InferConstructor(s_MasterMetadata);
		if (detailOptions.HasFlag(CollectionOptions.InferConstructor))
			m_DetailConstructor = MaterializerUtilities.InferConstructor(s_DetailMetadata);
	}

	/// <summary>
	/// This is the constructor to use when you want a MasterDetailObjectOrNull materializer
	/// </summary>
	public MasterDetailCollectionMaterializer(SingleRowDbCommandBuilder<TCommand, TParameter> commandBuilder, string masterKeyColumn, Func<TMaster, ICollection<TDetail>> map, RowOptions masterOptions, CollectionOptions detailOptions) :

		//We're taking advantage of the fact that the RowOptions/CollectionOptions flags are in the same position.
		this(commandBuilder, masterKeyColumn, map, (CollectionOptions)((int)masterOptions & (int)CollectionOptions.InferConstructor), detailOptions)
	{
		m_DiscardExtraRows = masterOptions.HasFlag(RowOptions.DiscardExtraRows);
		m_PreventEmptyResults = masterOptions.HasFlag(RowOptions.PreventEmptyResults);
	}

	public override IReadOnlyList<string> DesiredColumns()
	{
		//We need to pick the constructor now so that we have the right columns in the SQL.
		//If we wait until materialization, we could have missing or extra columns.

		if (m_MasterConstructor == null && !s_MasterMetadata.Constructors.HasDefaultConstructor)
			m_MasterConstructor = MaterializerUtilities.InferConstructor(s_MasterMetadata);

		if (m_DetailConstructor == null && !s_DetailMetadata.Constructors.HasDefaultConstructor)
			m_DetailConstructor = MaterializerUtilities.InferConstructor(s_DetailMetadata);

		var masterColumns = (m_MasterConstructor == null) ? s_MasterMetadata.ColumnsFor : m_MasterConstructor.ParameterNames;
		var detailColumns = (m_DetailConstructor == null) ? s_DetailMetadata.ColumnsFor : m_DetailConstructor.ParameterNames;

		//Sanity checks

		if (m_IncludedColumns != null && m_ExcludedColumns != null)
			throw new InvalidOperationException("Cannot specify both included and excluded columns/properties.");

		if (m_ExcludedColumns != null && (m_MasterConstructor != null || m_DetailConstructor != null))
			throw new InvalidOperationException("Cannot specify excluded columns/properties with non-default constructors.");

		if (masterColumns.Length == 0)
			throw new MappingException($"Type {typeof(TMaster).Name} has no writable properties. Please use the InferConstructor option or the WithMasterConstructor method.");

		if (detailColumns.Length == 0)
			throw new MappingException($"Type {typeof(TDetail).Name} has no writable properties. Please use the InferConstructor option or the WithMasterConstructor method.");

		//Assembly the list

		var columnNames = new HashSet<string>(); //using this to filter out duplicates

		if (m_IncludedColumns != null)
		{
			columnNames.AddRange(m_IncludedColumns);
		}
		else //Use the previously found values
		{
			columnNames.AddRange(masterColumns);
			columnNames.AddRange(detailColumns);

			if (m_ExcludedColumns != null)
				foreach (var column in m_ExcludedColumns)
					columnNames.Remove(column);
		}

		columnNames.Add(m_MasterKeyColumn); //Force this to always be included.
		return columnNames.ToImmutableArray();
	}

	public override List<TMaster> Execute(object? state = null)
	{
		Table? table = null;
		Prepare().Execute(cmd =>
		{
			using (var reader = cmd.ExecuteReader(CommandBehavior))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, state);

		return GenerateObjects(table!);
	}

	public override async Task<List<TMaster>> ExecuteAsync(CancellationToken cancellationToken, object? state = null)
	{
		Table? table = null;
		await Prepare().ExecuteAsync(async cmd =>
		{
			using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior, cancellationToken).ConfigureAwait(false))
			{
				table = new Table(reader, Converter);
				return table.Rows.Count;
			}
		}, cancellationToken, state).ConfigureAwait(false);

		return GenerateObjects(table!);
	}

	/// <summary>
	/// Excludes the properties from the list of what will be populated in the object.
	/// </summary>
	/// <param name="propertiesToOmit">The properties to omit.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	void ExceptProperties(string[] propertiesToOmit)
	{
		if (propertiesToOmit == null || propertiesToOmit.Length == 0)
			return;

		var result = new HashSet<string>();

		foreach (var propertyName in propertiesToOmit)
		{
			//A given property may be in either or both classes
			if (s_MasterMetadata.Properties.TryGetValue(propertyName, out var property1))
			{
				if (property1.MappedColumnName != null)
					result.Add(property1.MappedColumnName);
			}
			if (s_DetailMetadata.Properties.TryGetValue(propertyName, out var property2))
			{
				if (property2.MappedColumnName != null)
					result.Add(property2.MappedColumnName);
			}
		}

		m_ExcludedColumns = result.ToList();
	}

	List<TMaster> GenerateObjects(Table table)
	{
		IEqualityComparer<object> comparer;
		var columnType = table.ColumnTypeMap[m_MasterKeyColumn];

		if (columnType == typeof(short))
			comparer = new Int16EqualityComparer();
		else if (columnType == typeof(int))
			comparer = new Int32EqualityComparer();
		else if (columnType == typeof(long))
			comparer = new Int64EqualityComparer();
		else if (columnType == typeof(Guid))
			comparer = new GuidEqualityComparer();
		else if (columnType == typeof(string))
			comparer = new StringEqualityComparer();
		else if (columnType == typeof(DateTime))
			comparer = new DateTimeEqualityComparer();
		else if (columnType == typeof(DateTimeOffset))
			comparer = new DateTimeOffsetEqualityComparer();
		else if (columnType == typeof(ulong))
			comparer = new UInt64EqualityComparer();
		else
			throw new NotSupportedException($"Key column of type '{columnType.Name}' is not supported for Master/Detail collections.");

		var groups = new Dictionary<object, List<Row>>(comparer);
		foreach (var row in table.Rows)
		{
			var key = row[m_MasterKeyColumn];
			if (key == null)
				throw new MissingDataException($"A null was found in the master key column '{m_MasterKeyColumn}'");

			if (!groups.TryGetValue(key, out var group))
			{
				group = new();
				groups.Add(key, group);
			}
			group.Add(row);
		}

		var result = new List<TMaster>();
		foreach (var group in groups.Values)
		{
			var master = MaterializerUtilities.ConstructObject<TMaster>(group[0], m_MasterConstructor, Converter);
			var target = m_Map(master);
			foreach (var row in group)
				target.Add(MaterializerUtilities.ConstructObject<TDetail>(row, m_DetailConstructor, Converter));
			result.Add(master);
		}

		return result;
	}

	/// <summary>
	/// Sets the detail object constructor using a signature.
	/// </summary>
	/// <param name="signature">The constructor signature.</param>
	/// <exception cref="MappingException">Cannot find a matching detail object constructor for the desired type</exception>
	void SetDetailConstructor(IReadOnlyList<Type>? signature)
	{
		if (signature == null)
		{
			m_DetailConstructor = null;
		}
		else
		{
			var constructor = s_DetailMetadata.Constructors.Find(signature);
			if (constructor == null)
			{
				var types = string.Join(", ", signature.Select(t => t.Name));
				throw new MappingException($"Cannot find a constructor on {typeof(Type).Name} with the types [{types}]");
			}
			m_DetailConstructor = constructor;
		}
	}

	/// <summary>
	/// Sets the master object constructor using a signature.
	/// </summary>
	/// <param name="signature">The constructor signature.</param>
	/// <exception cref="MappingException">Cannot find a matching master object constructor for the desired type</exception>
	void SetMasterConstructor(IReadOnlyList<Type>? signature)
	{
		if (signature == null)
		{
			m_MasterConstructor = null;
		}
		else
		{
			var constructor = s_MasterMetadata.Constructors.Find(signature);
			if (constructor == null)
			{
				var types = string.Join(", ", signature.Select(t => t.Name));
				throw new MappingException($"Cannot find a constructor on {typeof(Type).Name} with the types [{types}]");
			}
			m_MasterConstructor = constructor;
		}
	}

	/// <summary>
	/// Limits the list of properties to populate to just the indicated list.
	/// </summary>
	/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
	/// <returns>ILink&lt;TResult&gt;.</returns>
	void WithProperties(string[] propertiesToPopulate)
	{
		if (propertiesToPopulate == null || propertiesToPopulate.Length == 0)
			return;

		var result = new HashSet<string>();

		foreach (var propertyName in propertiesToPopulate)
		{
			//A given property may be in either or both classes
			if (s_MasterMetadata.Properties.TryGetValue(propertyName, out var property1))
			{
				if (property1.MappedColumnName != null)
					result.Add(property1.MappedColumnName);
			}
			if (s_DetailMetadata.Properties.TryGetValue(propertyName, out var property2))
			{
				if (property2.MappedColumnName != null)
					result.Add(property2.MappedColumnName);
			}
		}

		m_IncludedColumns = result.ToList();
	}
}
