namespace Tortuga.Chain.DataSources;
/// <summary>
/// Used to mark data sources that support a full set of basic CRUD operations.
/// </summary>
/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
public interface ICrudDataSource :
	IDataSource,
	ISupportsSqlQueries,
	ISupportsDelete,
	ISupportsDeleteByKey,
	ISupportsDeleteByKeyList,
	ISupportsDeleteSet,
	ISupportsDeleteAll,
	ISupportsFrom,
	ISupportsGetByKey,
	ISupportsGetByKeyList,
	ISupportsInsert,
	ISupportsUpdate,
	ISupportsUpdateByKey,
	ISupportsUpdateByKeyList,
	ISupportsUpdateSet
{ }
