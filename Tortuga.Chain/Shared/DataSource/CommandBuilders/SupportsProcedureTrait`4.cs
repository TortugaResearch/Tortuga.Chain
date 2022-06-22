using System.Data.Common;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class SupportsProcedureTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsProcedure
where TCommand : DbCommand
where TParameter : DbParameter
where TObjectName : struct
where TDbType : struct
{
	[Partial("procedureName,argumentValue")]
	public Func<TObjectName, object?, ProcedureDbCommandBuilder<TCommand, TParameter>> OnProcedure { get; set; } = null!;

	[Container(RegisterInterface = true)]
	internal ICommandHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

	IProcedureDbCommandBuilder ISupportsProcedure.Procedure(string procedureName)
	{
		return OnProcedure(DataSource.DatabaseMetadata.ParseObjectName(procedureName), null);
	}

	IProcedureDbCommandBuilder ISupportsProcedure.Procedure(string procedureName, object argumentValue)
	{
		return OnProcedure(DataSource.DatabaseMetadata.ParseObjectName(procedureName), argumentValue);
	}

	/// <summary>
	/// Loads a procedure definition
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <returns></returns>
	[Expose]
	public ProcedureDbCommandBuilder<TCommand, TParameter> Procedure(TObjectName procedureName)
	{
		return OnProcedure(procedureName, null);
	}

	/// <summary>
	/// Loads a procedure definition and populates it using the parameter object.
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <returns></returns>
	/// <remarks>
	/// The procedure's definition is loaded from the database and used to determine which properties on the parameter object to use.
	/// </remarks>
	[Expose]
	public ProcedureDbCommandBuilder<TCommand, TParameter> Procedure(TObjectName procedureName, object argumentValue)
	{
		return OnProcedure(procedureName, argumentValue);
	}
}



