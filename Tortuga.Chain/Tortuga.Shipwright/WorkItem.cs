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

//class TraitWorkItem
//{
//	public TraitWorkItem(INamedTypeSymbol traitClass)
//	{
//		TraitClass = traitClass ?? throw new ArgumentNullException(nameof(traitClass));

//		var interfaces = TraitClass.AllInterfaces
//	}
//	public INamedTypeSymbol TraitClass { get; }


//}