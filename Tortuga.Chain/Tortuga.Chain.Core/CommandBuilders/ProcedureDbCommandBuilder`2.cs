using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that represent stored procedures.
/// </summary>
public abstract class ProcedureDbCommandBuilder<TCommand, TParameter> : MultipleTableDbCommandBuilder<TCommand, TParameter>, IProcedureDbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.ProcedureDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	protected ProcedureDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
		: base(dataSource)
	{ }

	/// <summary>
	/// Captures the output parameters as a dictionary.
	/// </summary>
	/// <returns>The output parameters as a dictionary.</returns>
	/// <remarks>This will throw an exception if there are no output parameters.</remarks>
	public ILink<Dictionary<string, object?>> AsOutputs()
	{
		return new AsOutputsMaterializer<TCommand, TParameter>(this);
	}

	/// <summary>
	/// Captures the output parameters and use them to populate a new object.
	/// </summary>
	/// <typeparam name="TObject">The type that will hold the output parameters.</typeparam>
	/// <returns>The output parameters as an object.</returns>
	/// <remarks>This will throw an exception if there are no output parameters.</remarks>
	public ILink<TObject> AsOutputs<TObject>() where TObject : class, new()
	{
		return new AsOutputsMaterializer<TCommand, TParameter, TObject>(this);
	}
}
