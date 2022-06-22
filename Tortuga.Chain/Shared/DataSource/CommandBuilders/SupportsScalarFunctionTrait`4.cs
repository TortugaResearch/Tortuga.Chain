using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsScalarFunctionTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsScalarFunction
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{
	[Partial("scalarFunctionName,argumentValue")]
	public Func<TObjectName, object?, ScalarDbCommandBuilder<TCommand, TParameter>> OnScalarFunction { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IScalarDbCommandBuilder ISupportsScalarFunction.ScalarFunction(string scalarFunctionName)
	{
		return OnScalarFunction(DataSource.DatabaseMetadata.ParseObjectName(scalarFunctionName), null);
	}

	IScalarDbCommandBuilder ISupportsScalarFunction.ScalarFunction(string scalarFunctionName, object functionArgumentValue)
	{
		return OnScalarFunction(DataSource.DatabaseMetadata.ParseObjectName(scalarFunctionName), functionArgumentValue);
	}

	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <returns></returns>
	[Expose]
	public ScalarDbCommandBuilder<TCommand, TParameter> ScalarFunction(TObjectName scalarFunctionName)
	{
		return OnScalarFunction(scalarFunctionName, null);
	}

	/// <summary>
	/// This is used to query a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	/// <returns></returns>
	[Expose]
	public ScalarDbCommandBuilder<TCommand, TParameter> ScalarFunction(TObjectName scalarFunctionName, object functionArgumentValue)
	{
		return OnScalarFunction(scalarFunctionName, functionArgumentValue);
	}
}

