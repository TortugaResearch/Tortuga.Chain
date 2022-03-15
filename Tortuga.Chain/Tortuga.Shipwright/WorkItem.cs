using Microsoft.CodeAnalysis;

namespace Tortuga.Shipwright;

class WorkItem
{
	public WorkItem(INamedTypeSymbol hostingClass)
	{
		HostingClass = hostingClass ?? throw new ArgumentNullException(nameof(hostingClass));
	}

	public INamedTypeSymbol HostingClass { get; }
	public HashSet<INamedTypeSymbol> TraitClasses { get; } = new(SymbolEqualityComparer.Default);
}

