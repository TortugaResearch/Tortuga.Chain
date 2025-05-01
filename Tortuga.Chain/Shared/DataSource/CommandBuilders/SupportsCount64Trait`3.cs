using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.Aggregates;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("Performance", "CA1812")]
[SuppressMessage("Performance", "CA1822")]
sealed class SupportsCount64Trait<TCommand, TParameter, TLimit> : ISupportsCount64
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TLimit : struct //really an enum
{
	//[Partial("tableName")] public Func<TObjectName, ILink<int?>> OnDeleteAll { get; set; } = null!;

	[Container(RegisterInterface = false)]
	internal TableDbCommandBuilder<TCommand, TParameter, TLimit> CommandBuilder { get; set; } = null!;

	ILink<long> ISupportsCount64.AsCount64() => AsCount64();

	ILink<long> ISupportsCount64.AsCount64(string columnName, bool distinct) => AsCount64(columnName, distinct);

	/// <summary>
	/// Returns a 64 bit row count using a <c>SELECT Count(*)</c> style query.
	/// </summary>
	/// <returns></returns>
	[Expose]
	public ILink<long> AsCount64()
	{
		return CommandBuilder.AsAggregate(new AggregateColumn(AggregateType.Count64, "*", "RowCount")).ToInt64();
	}

	/// <summary>
	/// Returns a 64 bit row count for a given column. <c>SELECT Count(columnName)</c>
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="distinct">if set to <c>true</c> use <c>SELECT COUNT(DISTINCT columnName)</c>.</param>
	/// <returns></returns>
	[Expose]
	public ILink<long> AsCount64(string columnName, bool distinct = false)
	{
		var column = CommandBuilder.Columns[columnName];

		return CommandBuilder.AsAggregate(new AggregateColumn(distinct ? AggregateType.CountDistinct64 : AggregateType.Count64, column.SqlName, column.SqlName)).ToInt64();
	}
}
