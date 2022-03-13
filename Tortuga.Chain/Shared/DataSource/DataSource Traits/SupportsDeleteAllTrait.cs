using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsDeleteAllTrait<TObjectName> : ISupportsDeleteAll where TObjectName : struct
{
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
		return OnDeleteAll(OnGetTableOrViewNameFromClass(typeof(TObject), OperationType.All));
	}

	ILink<int?> ISupportsDeleteAll.DeleteAll(string tableName) => OnDeleteAll(OnParseObjectName(tableName));


	ILink<int?> ISupportsDeleteAll.DeleteAll<TObject>() => DeleteAll<TObject>();

	[Partial("type, operationType")] public Func<Type, OperationType, TObjectName> OnGetTableOrViewNameFromClass { get; set; } = null!;
	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnDeleteAll { get; set; } = null!;
	[Partial("objectName")] public Func<string, TObjectName> OnParseObjectName { get; set; } = null!;


}


