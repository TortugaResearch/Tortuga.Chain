﻿using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
sealed class SupportsTruncateTrait<TParameter, TObjectName, TDbType> : ISupportsTruncate
	where TObjectName : struct
	where TDbType : struct
	where TParameter : DbParameter
{
	[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnTruncate { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TObjectName, TDbType> DataSource { get; set; } = null!;

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
		return OnTruncate(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name);
	}

	ILink<int?> ISupportsTruncate.Truncate(string tableName) => OnTruncate(DataSource.DatabaseMetadata.ParseObjectName(tableName));

	ILink<int?> ISupportsTruncate.Truncate<TObject>() => Truncate<TObject>();
}
