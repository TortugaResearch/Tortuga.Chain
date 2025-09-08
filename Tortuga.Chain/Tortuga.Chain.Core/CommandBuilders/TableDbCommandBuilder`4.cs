﻿using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for table style command builders such as FromTableOrView and FromTableValueFunction.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TLimit">The type of the limit option.</typeparam>
/// <typeparam name="TObject">The type of the object to be constructed.</typeparam>
/// <seealso cref="CommandBuilders.MultipleRowDbCommandBuilder{TCommand, TParameter}" />
/// <seealso cref="ITableDbCommandBuilder" />
public abstract class TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> : TableDbCommandBuilder<TCommand, TParameter, TLimit>, ITableDbCommandBuilder<TObject>
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TLimit : struct //really an enum
	where TObject : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TableDbCommandBuilder{TCommand, TParameter, TLimit}"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	protected TableDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource) : base(dataSource)
	{
	}

	/// <summary>
	/// Performs an aggregation on the table using the provided object.
	/// </summary>
	/// <returns>ObjectMultipleRow&lt;TCommand, TParameter, TObject&gt;.</returns>
	public ObjectMultipleRow<TCommand, TParameter, TObject> AsAggregate()
	{
		return AsAggregate<TObject>();
	}

	/// <summary>
	/// Materializes the result as a list of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<List<TObject>> ToCollection(CollectionOptions collectionOptions = CollectionOptions.None)
		=> ToCollection<TObject>(collectionOptions);

	/// <summary>
	/// Materializes the result as a dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyColumn">The key column.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToDictionary<TKey, TObject>(keyColumn, dictionaryOptions);

	/// <summary>
	/// Materializes the result as a dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyFunction">The key function.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToDictionary<TKey, TObject>(keyFunction, dictionaryOptions);

	/// <summary>
	/// Materializes the result as an immutable array of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableArray&lt;TObject&gt;&gt;.</returns>
	/// <exception cref="MappingException"></exception>
	/// <remarks>In theory this will offer better performance than ToImmutableList if you only intend to read the result.</remarks>
	public IConstructibleMaterializer<ImmutableArray<TObject>> ToImmutableArray(CollectionOptions collectionOptions = CollectionOptions.None)
		=> ToImmutableArray<TObject>(collectionOptions);

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyFunction">The key function.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToImmutableDictionary<TKey, TObject>(keyFunction, dictionaryOptions);

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyFunction">The key function.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<FrozenDictionary<TKey, TObject>> ToFrozenDictionary<TKey>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToFrozenDictionary<TKey, TObject>(keyFunction, dictionaryOptions);

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyColumn">The key column.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToImmutableDictionary<TKey, TObject>(keyColumn, dictionaryOptions);

	/// <summary>
	/// Materializes the result as a immutable dictionary of objects.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="keyColumn">The key column.</param>
	/// <param name="dictionaryOptions">The dictionary options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<FrozenDictionary<TKey, TObject>> ToFrozenDictionary<TKey>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
		where TKey : notnull
		=> ToFrozenDictionary<TKey, TObject>(keyColumn, dictionaryOptions);

	/// <summary>
	/// Materializes the result as an immutable list of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableList&lt;TObject&gt;&gt;.</returns>
	/// <exception cref="MappingException"></exception>
	/// <remarks>In theory this will offer better performance than ToImmutableArray if you intend to further modify the result.</remarks>
	public IConstructibleMaterializer<ImmutableList<TObject>> ToImmutableList(CollectionOptions collectionOptions = CollectionOptions.None)
		=> ToImmutableList<TObject>(collectionOptions);

	/// <summary>
	/// Materializes the result as a list of master/detail records.
	/// </summary>
	/// <typeparam name="TDetail">The type of the detail model.</typeparam>
	/// <param name="masterKeyColumn">The column used as the primary key for the master records.</param>
	/// <param name="map">This is used to identify the detail collection property on the master object.</param>
	/// <param name="masterOptions">Options for handling extraneous rows and constructor selection for the master object.</param>
	/// <param name="detailOptions">Options for handling constructor selection for the detail objects</param>
	/// <returns></returns>
	public IMasterDetailMaterializer<List<TObject>> ToMasterDetailCollection<TDetail>(string masterKeyColumn, Func<TObject, ICollection<TDetail>> map, CollectionOptions masterOptions = CollectionOptions.None, CollectionOptions detailOptions = CollectionOptions.None)
		where TDetail : class
	{
		return ToMasterDetailCollection<TObject, TDetail>(masterKeyColumn, map, masterOptions, detailOptions);
	}

	/// <summary>
	/// Materializes the result as a master object with detail records.
	/// </summary>
	/// <typeparam name="TDetail">The type of the detail model.</typeparam>
	/// <param name="masterKeyColumn">The column used as the primary key for the master records.</param>
	/// <param name="map">This is used to identify the detail collection property on the master object.</param>
	/// <param name="masterOptions">Options for handling extraneous rows and constructor selection for the master object.</param>
	/// <param name="detailOptions">Options for handling constructor selection for the detail objects</param>
	/// <returns></returns>
	public IMasterDetailMaterializer<TObject> ToMasterDetailObject<TDetail>(string masterKeyColumn, Func<TObject, ICollection<TDetail>> map, RowOptions masterOptions = RowOptions.None, CollectionOptions detailOptions = CollectionOptions.None)
		where TDetail : class
	{
		return ToMasterDetailObject<TObject, TDetail>(masterKeyColumn, map, masterOptions, detailOptions);
	}

	/// <summary>
	/// Materializes the result as a master object with detail records.
	/// </summary>
	/// <typeparam name="TDetail">The type of the detail model.</typeparam>
	/// <param name="masterKeyColumn">The column used as the primary key for the master records.</param>
	/// <param name="map">This is used to identify the detail collection property on the master object.</param>
	/// <param name="masterOptions">Options for handling extraneous rows and constructor selection for the master object.</param>
	/// <param name="detailOptions">Options for handling constructor selection for the detail objects</param>
	/// <returns></returns>
	public IMasterDetailMaterializer<TObject?> ToMasterDetailObjectOrNull<TDetail>(string masterKeyColumn, Func<TObject, ICollection<TDetail>> map, RowOptions masterOptions = RowOptions.None, CollectionOptions detailOptions = CollectionOptions.None)
		where TDetail : class
	{
		return ToMasterDetailObjectOrNull<TObject, TDetail>(masterKeyColumn, map, masterOptions, detailOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<TObject> ToObject(RowOptions rowOptions = RowOptions.None)
		=> ToObject<TObject>(rowOptions);

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<TObject?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None)
		=> ToObjectOrNull<TObject>(rowOptions);

	/************************************************************************************************/

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override sealed ColumnMetadata? TryGetColumn(string columnName)
	{
		return OnGetTable().Columns.TryGetColumn(columnName);
	}

	/// <summary>
	/// Returns a list of columns.
	/// </summary>
	/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
	/// <remarks>This is used by materializers to skip exclude columns.</remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetColumns() => OnGetTable().Columns;

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public sealed override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => OnGetTable().NonNullableColumns;

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithFilter(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		=> OnWithFilterTyped(filterValue, filterOptions);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithFilter(string whereClause)
		=> WithFilter(whereClause, null);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithFilter(string whereClause, object? argumentValue)
		=> OnWithFilterTyped(whereClause, argumentValue);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithFilter(object filterValue, FilterOptions filterOptions) => WithFilter(filterValue, filterOptions);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithFilter(string whereClause) => WithFilter(whereClause);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithFilter(string whereClause, object? argumentValue) => WithFilter(whereClause, argumentValue);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithLimits(int? skip, int? take) => OnWithLimitsTyped(skip, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithLimits(int? take) => OnWithLimitsTyped(null, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithLimits(int? take, TLimit limitOptions) => OnWithLimitsTyped(null, take, limitOptions, null);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithLimits(int? take, TLimit limitOptions, int seed) => OnWithLimitsTyped(null, take, limitOptions, seed);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithLimits(int take, LimitOptions limitOptions, int? seed) => OnWithLimitsTyped(null, take, limitOptions, seed);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithLimits(int skip, int take) => OnWithLimitsTyped(skip, take, DefaultLimitOption, null);

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithSorting(IEnumerable<SortExpression> sortExpressions) => OnWithSortingTyped(sortExpressions);

	/// <summary>
	/// Adds sorting to the command builder
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithSorting(params SortExpression[] sortExpressions) => WithSorting((IEnumerable<SortExpression>)sortExpressions);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithSorting(IEnumerable<SortExpression> sortExpressions) => WithSorting(sortExpressions);

	ITableDbCommandBuilder<TObject> ITableDbCommandBuilder<TObject>.WithSorting(params SortExpression[] sortExpressions) => WithSorting((IEnumerable<SortExpression>)sortExpressions);

	/// <summary>
	/// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
	/// </summary>
	/// <returns></returns>
	protected abstract TableOrViewMetadata OnGetTable();

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="whereClause">The where clause.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> OnWithFilterTyped(string whereClause, object? argumentValue)
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithFilter(whereClause, argumentValue);

	/// <summary>
	/// Adds (or replaces) the filter on this command builder.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> OnWithFilterTyped(object filterValue, FilterOptions filterOptions = FilterOptions.None)
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithFilter(filterValue, filterOptions);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> OnWithLimitsTyped(int? skip, int? take, TLimit limitOptions, int? seed)
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithLimits(skip, take, limitOptions, seed);

	/// <summary>
	/// Adds limits to the command builder.
	/// </summary>
	/// <param name="skip">The number of rows to skip.</param>
	/// <param name="take">Number of rows to take.</param>
	/// <param name="limitOptions">The limit options.</param>
	/// <param name="seed">The seed for repeatable reads. Only applies to random sampling</param>
	/// <returns></returns>
	TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> OnWithLimitsTyped(int? skip, int? take, LimitOptions limitOptions, int? seed)
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithLimits(skip, take, limitOptions, seed);

	/// <summary>
	/// Adds sorting to the command builder.
	/// </summary>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <returns></returns>
	TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> OnWithSortingTyped(IEnumerable<SortExpression> sortExpressions)
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithSorting(sortExpressions);

	/// <summary>
	/// Applies a distinct operator to the command builder.
	/// </summary>
	/// <returns></returns>
	new public TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject> WithDistinct()
		=> (TableDbCommandBuilder<TCommand, TParameter, TLimit, TObject>)OnWithDistinct();

}
