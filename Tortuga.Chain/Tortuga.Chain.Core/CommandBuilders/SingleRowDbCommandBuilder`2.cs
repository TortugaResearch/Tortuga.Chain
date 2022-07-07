using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that can potentially return one row.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
public abstract class SingleRowDbCommandBuilder<TCommand, TParameter> : ScalarDbCommandBuilder<TCommand, TParameter>, ISingleRowDbCommandBuilder
where TCommand : DbCommand
where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	protected SingleRowDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
		: base(dataSource)
	{
	}

	/// <summary>
	/// Indicates the results should be materialized as a Row.
	/// </summary>
	public ILink<DataRow> ToDataRow(RowOptions rowOptions = RowOptions.None) => new DataRowMaterializer<TCommand, TParameter>(this, rowOptions);

	/// <summary>
	/// Indicates the results should be materialized as a Row.
	/// </summary>
	public ILink<DataRow?> ToDataRowOrNull(RowOptions rowOptions = RowOptions.None) => new DataRowOrNullMaterializer<TCommand, TParameter>(this, rowOptions);

	/// <summary>
	/// Materializes the result as a dynamic object
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public ILink<dynamic> ToDynamicObject(RowOptions rowOptions = RowOptions.None) => new DynamicObjectMaterializer<TCommand, TParameter>(this, rowOptions);

	/// <summary>
	/// Materializes the result as a dynamic object
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public ILink<dynamic?> ToDynamicObjectOrNull(RowOptions rowOptions = RowOptions.None) => new DynamicObjectOrNullMaterializer<TCommand, TParameter>(this, rowOptions);

	/// <summary>
	/// Materializes the result as a master object with detail records.
	/// </summary>
	/// <typeparam name="TMaster">The type of the master model.</typeparam>
	/// <typeparam name="TDetail">The type of the detail model.</typeparam>
	/// <param name="masterKeyColumn">The column used as the primary key for the master records.</param>
	/// <param name="map">This is used to identify the detail collection property on the master object.</param>
	/// <param name="masterOptions">Options for handling extraneous rows and constructor selection for the master object.</param>
	/// <param name="detailOptions">Options for handling constructor selection for the detail objects</param>
	/// <returns></returns>
	public ILink<TMaster> ToMasterDetailObject<TMaster, TDetail>(string masterKeyColumn, Func<TMaster, ICollection<TDetail>> map, RowOptions masterOptions = RowOptions.None, CollectionOptions detailOptions = CollectionOptions.None)
		where TMaster : class
		where TDetail : class
	{
		//We're taking advantage of the fact that the flags are in the same position.
		var masterCollectionOptions = (CollectionOptions)((int)masterOptions & (int)CollectionOptions.InferConstructor);

		return new MasterDetailCollectionMaterializer<TCommand, TParameter, TMaster, TDetail>(this, masterKeyColumn, map, masterCollectionOptions, detailOptions).Transform(x =>
		{
			if (x.Count == 0)
				throw new MissingDataException("No records were returned");
			if (x.Count > 0 && !masterOptions.HasFlag(RowOptions.DiscardExtraRows))
				throw new UnexpectedDataException("More records were returned than expected");
			return x[0];
		});
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <typeparam name="TObject">The type of the object returned.</typeparam>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
		where TObject : class
	{
		return new ObjectMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <typeparam name="TObject">The type of the object returned.</typeparam>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public IConstructibleMaterializer<TObject?> ToObjectOrNull<TObject>(RowOptions rowOptions = RowOptions.None)
		where TObject : class
	{
		return new ObjectOrNullMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
	}

	/// <summary>
	/// Indicates the results should be materialized as a Row.
	/// </summary>
	public ILink<Row> ToRow(RowOptions rowOptions = RowOptions.None) => new RowMaterializer<TCommand, TParameter>(this, rowOptions);

	/// <summary>
	/// Indicates the results should be materialized as a Row.
	/// </summary>
	public ILink<Row?> ToRowOrNull(RowOptions rowOptions = RowOptions.None) => new RowOrNullMaterializer<TCommand, TParameter>(this, rowOptions);
}