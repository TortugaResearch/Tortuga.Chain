using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsTableFunctionTrait<TCommand, TParameter, TObjectName, TDbType, TLimitOption> : ISupportsTableFunction
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
where TLimitOption : struct
{
	[Partial("tableFunctionName,functionArgumentValue")]
	public Func<TObjectName, object?, TableDbCommandBuilder<TCommand, TParameter, TLimitOption>> OnTableFunction { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	ITableDbCommandBuilder ISupportsTableFunction.TableFunction(string functionName)
	{
		return OnTableFunction(DataSource.DatabaseMetadata.ParseObjectName(functionName), null);
	}

	ITableDbCommandBuilder ISupportsTableFunction.TableFunction(string functionName, object functionArgumentValue)
	{
		return OnTableFunction(DataSource.DatabaseMetadata.ParseObjectName(functionName), functionArgumentValue);
	}

	/// <summary>
	/// This is used to query a table valued function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <returns></returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> TableFunction(TObjectName tableFunctionName)
	{
		return OnTableFunction(tableFunctionName, null);
	}

	/// <summary>
	/// This is used to query a table valued function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	/// <returns></returns>
	[Expose]
	public TableDbCommandBuilder<TCommand, TParameter, TLimitOption> TableFunction(TObjectName tableFunctionName, object functionArgumentValue)
	{
		return OnTableFunction(tableFunctionName, functionArgumentValue);
	}
}



