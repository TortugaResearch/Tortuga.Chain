using Microsoft.CodeAnalysis;

namespace Tortuga.Shipwright;

class WorkItem
{
	public WorkItem(INamedTypeSymbol hostingClass)
	{
		ContainerClass = hostingClass ?? throw new ArgumentNullException(nameof(hostingClass));
	}

	public INamedTypeSymbol ContainerClass { get; }
	public HashSet<INamedTypeSymbol> TraitClasses { get; } = new(SymbolEqualityComparer.Default);
}

