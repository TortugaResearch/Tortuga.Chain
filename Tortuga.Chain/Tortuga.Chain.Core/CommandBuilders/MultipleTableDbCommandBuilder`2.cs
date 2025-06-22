using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that can potentially return multiple result sets.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
public abstract class MultipleTableDbCommandBuilder<TCommand, TParameter> : MultipleRowDbCommandBuilder<TCommand, TParameter>, IMultipleTableDbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	protected MultipleTableDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
		: base(dataSource)
	{ }

	/// <summary>
	/// To the collection set.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st collection.</typeparam>
	/// <typeparam name="T2">The type of the 2nd collection.</typeparam>
	/// <returns></returns>
	public ILink<Tuple<List<T1>, List<T2>>> ToCollectionSet<T1, T2>()
		where T1 : class, new()
		where T2 : class, new()
	{
		return new CollectionSetMaterializer<TCommand, TParameter, T1, T2>(this);
	}

	/// <summary>
	/// To the collection set.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st collection.</typeparam>
	/// <typeparam name="T2">The type of the 2nd collection.</typeparam>
	/// <typeparam name="T3">The type of the 3rd collection.</typeparam>
	/// <returns></returns>
	public ILink<Tuple<List<T1>, List<T2>, List<T3>>> ToCollectionSet<T1, T2, T3>()
		where T1 : class, new()
		where T2 : class, new()
		where T3 : class, new()
	{
		return new CollectionSetMaterializer<TCommand, TParameter, T1, T2, T3>(this);
	}

	/// <summary>
	/// To the collection set.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st collection.</typeparam>
	/// <typeparam name="T2">The type of the 2nd collection.</typeparam>
	/// <typeparam name="T3">The type of the 3rd collection.</typeparam>
	/// <typeparam name="T4">The type of the 4th collection.</typeparam>
	/// <returns></returns>
	public ILink<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> ToCollectionSet<T1, T2, T3, T4>()
		where T1 : class, new()
		where T2 : class, new()
		where T3 : class, new()
		where T4 : class, new()
	{
		return new CollectionSetMaterializer<TCommand, TParameter, T1, T2, T3, T4>(this);
	}

	/// <summary>
	/// To the collection set.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st collection.</typeparam>
	/// <typeparam name="T2">The type of the 2nd collection.</typeparam>
	/// <typeparam name="T3">The type of the 3rd collection.</typeparam>
	/// <typeparam name="T4">The type of the 4th collection.</typeparam>
	/// <typeparam name="T5">The type of the 5th collection.</typeparam>
	/// <returns></returns>
	public ILink<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>> ToCollectionSet<T1, T2, T3, T4, T5>()
		where T1 : class, new()
		where T2 : class, new()
		where T3 : class, new()
		where T4 : class, new()
		where T5 : class, new()
	{
		return new CollectionSetMaterializer<TCommand, TParameter, T1, T2, T3, T4, T5>(this);
	}

	/// <summary>
	/// Indicates the results should be materialized as a DataSet.
	/// </summary>
	/// <param name="tableNames">The table names. This must either be empty or at least one name per result set returned by the database.</param>
	/// <remarks>
	/// There is a performance hit when not providing tableNames. The results will be loaded into a TableSet, then copied into a DataSet.
	/// </remarks>
	public ILink<DataSet> ToDataSet(params string[] tableNames) { return new DataSetMaterializer<TCommand, TParameter>(this, tableNames); }

	/// <summary>
	/// Indicates the results should be materialized as a DataSet.
	/// </summary>
	/// <remarks>
	/// There is a performance hit when not providing tableNames. The results will be loaded into a TableSet, then copied into a DataSet.
	/// </remarks>
	public ILink<DataSet> ToDataSet() { return new DataSetMaterializer<TCommand, TParameter>(this, null); }

	/// <summary>
	/// Indicates the results should be materialized as a set of tables.
	/// </summary>
	public ILink<TableSet> ToTableSet(params string[] tableNames) { return new TableSetMaterializer<TCommand, TParameter>(this, tableNames); }
}
