using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
sealed class SupportsDisableIndexesTrait<TParameter, TObjectName, TDbType> : ISupportsDisableIndexes
	where TObjectName : struct
	where TDbType : struct
	where TParameter : DbParameter
{
	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnDisableIndexes { get; set; } = null!;
	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnEnableIndexes { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TObjectName, TDbType> DataSource { get; set; } = null!;

	/// <summary>
	/// Disables all of the indexes on the indicated table..
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <remarks>For SQL Server, this will not disable the clustered index.</remarks>
	[Expose] public ILink<int?> DisableIndexes(TObjectName tableName) => OnDisableIndexes(tableName);

	/// <summary>
	/// Enables all of the indexes on the indicated table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>

	[Expose] public ILink<int?> EnableIndexes(TObjectName tableName) => OnEnableIndexes(tableName);

	/// <summary>
	/// Disables all of the indexes on the indicated table..
	/// </summary>
	/// <remarks>For SQL Server, this will not disable the clustered index.</remarks>
	[Expose]
	public ILink<int?> DisableIndexes<TObject>() where TObject : class
	{
		return OnDisableIndexes(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
	}

	/// <summary>
	/// Enables all of the indexes on the indicated table.
	/// </summary>
	[Expose]
	public ILink<int?> EnableIndexes<TObject>() where TObject : class
	{
		return OnEnableIndexes(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
	}

	ILink<int?> ISupportsDisableIndexes.DisableIndexes(string tableName) => OnDisableIndexes(DataSource.DatabaseMetadata.ParseObjectName(tableName));

	ILink<int?> ISupportsDisableIndexes.EnableIndexes<TObject>() => EnableIndexes<TObject>();

	ILink<int?> ISupportsDisableIndexes.EnableIndexes(string tableName) => OnEnableIndexes(DataSource.DatabaseMetadata.ParseObjectName(tableName));

	ILink<int?> ISupportsDisableIndexes.DisableIndexes<TObject>() => DisableIndexes<TObject>();
}
