using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsDeleteAllTrait<TParameter, TObjectName, TDbType> : ISupportsDeleteAll
	where TObjectName : struct
	where TDbType : struct
	where TParameter : DbParameter
{
	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnDeleteAll { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	/// <summary>Deletes all records in the specified table.</summary>
	/// <param name="tableName">Name of the table to clear.</param>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	[Expose] public ILink<int?> DeleteAll(TObjectName tableName) => OnDeleteAll(tableName);

	/// <summary>Deletes all records in the specified table.</summary>
	/// <typeparam name="TObject">This class used to determine which table to clear</typeparam>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	[Expose]
	public ILink<int?> DeleteAll<TObject>() where TObject : class
	{
		return OnDeleteAll(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
	}

	ILink<int?> ISupportsDeleteAll.DeleteAll(string tableName) => OnDeleteAll(DataSource.DatabaseMetadata.ParseObjectName(tableName));

	ILink<int?> ISupportsDeleteAll.DeleteAll<TObject>() => DeleteAll<TObject>();
}
