using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;


[Trait]
class SupportsTruncateTrait<TObjectName> : ISupportsTruncate where TObjectName : struct
{
	[Partial("type, operationType")] public Func<Type, OperationType, TObjectName> OnGetTableOrViewNameFromClass { get; set; } = null!;

	[Partial("objectName")] public Func<string, TObjectName> OnParseObjectName { get; set; } = null!;

	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnTruncate { get; set; } = null!;

	/// <summary>Truncates the specified table.</summary>
	/// <param name="tableName">Name of the table to Truncate.</param>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	[Expose] public ILink<int?> Truncate(TObjectName tableName) => OnTruncate(tableName);

	/// <summary>Truncates the specified table.</summary>
	/// <typeparam name="TObject">This class used to determine which table to Truncate</typeparam>
	/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
	[Expose]
	public ILink<int?> Truncate<TObject>() where TObject : class
	{
		return OnTruncate(OnGetTableOrViewNameFromClass(typeof(TObject), OperationType.All));
	}

	ILink<int?> ISupportsTruncate.Truncate(string tableName) => OnTruncate(OnParseObjectName(tableName));


	ILink<int?> ISupportsTruncate.Truncate<TObject>() => Truncate<TObject>();
}


