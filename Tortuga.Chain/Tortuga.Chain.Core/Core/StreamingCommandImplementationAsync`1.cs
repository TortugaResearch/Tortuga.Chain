using System.Data.Common;

namespace Tortuga.Chain.Core;

/// <summary>
/// The implementation of an operation from a CommandBuilder.
/// </summary>
/// <typeparam name="TCommand">The type of the t command.</typeparam>
/// <param name="command">The command.</param>
public delegate Task StreamingCommandImplementationAsync<TCommand>(TCommand command)
		  where TCommand : DbCommand;
