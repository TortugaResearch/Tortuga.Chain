using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers;

internal sealed class MasterDetailCollectionMaterializer<TCommand, TParameter, TMaster, TDetail> : Materializer<TCommand, TParameter, List<TMaster>>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TMaster : class
	where TDetail : class

{
	static readonly ImmutableArray<string> s_DesiredColumns = MetadataCache.GetMetadata<TMaster>().ColumnsFor.Union(MetadataCache.GetMetadata<TMaster>().ColumnsFor).ToImmutableArray();
	static readonly ClassMetadata s_DetailMetadata = MetadataCache.GetMetadata<TMaster>();
	static readonly ClassMetadata s_MasterMetadata = MetadataCache.GetMetadata<TMaster>();
	readonly DbCommandBuilder<TCommand, TParameter> commandBuilder;
	readonly CollectionOptions m_DetailOptions;
	readonly Func<TMaster, ICollection<TDetail>> m_Map;
	readonly string m_MasterKeyColumn;
	readonly CollectionOptions m_MasterOptions;

	public MasterDetailCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string masterKeyColumn, Func<TMaster, ICollection<TDetail>> map, CollectionOptions masterOptions, CollectionOptions detailOptions) : base(commandBuilder)
	{
		this.commandBuilder = commandBuilder;
		m_MasterKeyColumn = masterKeyColumn;
		m_Map = map;
		m_MasterOptions = masterOptions;
		m_DetailOptions = detailOptions;
		if (!s_DesiredColumns.Contains(masterKeyColumn))
			throw new MappingException($"Cannot find column '{masterKeyColumn}' in the list of columns for {typeof(TMaster).Name}");
	}

	public override IReadOnlyList<string> DesiredColumns() => s_DesiredColumns;

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

		//As a future enhancement, consider supporting constructor inference.
		ConstructorMetadata? masterConstructor = null;
		ConstructorMetadata? detailConstructor = null;

		//We are caching the default constructor for performance reasons.
		if (m_MasterOptions.HasFlag(CollectionOptions.InferConstructor))
			masterConstructor = MaterializerUtilities.InferConstructor(s_MasterMetadata);
		else
			masterConstructor = MaterializerUtilities.DefaultConstructor(s_MasterMetadata);

		if (m_DetailOptions.HasFlag(CollectionOptions.InferConstructor))
			detailConstructor = MaterializerUtilities.InferConstructor(s_DetailMetadata);
		else
			detailConstructor = MaterializerUtilities.DefaultConstructor(s_MasterMetadata);

		var result = new List<TMaster>();
		foreach (var group in groups.Values)
		{
			var master = MaterializerUtilities.ConstructObject<TMaster>(group[0], masterConstructor, Converter);
			var target = m_Map(master);
			foreach (var row in group)
				target.Add(MaterializerUtilities.ConstructObject<TDetail>(row, detailConstructor, Converter));
		}

		return result;
	}

	class DateTimeEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (DateTime)x! == (DateTime)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class DateTimeOffsetEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (DateTimeOffset)x! == (DateTimeOffset)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class GuidEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (Guid)x! == (Guid)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int16EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (short)x! == (short)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int32EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (int)x! == (int)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int64EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (long)x! == (long)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class StringEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => string.Equals((string)x!, (string)y!, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(object obj) => ((string)obj).GetHashCode(StringComparison.OrdinalIgnoreCase);
	}
}