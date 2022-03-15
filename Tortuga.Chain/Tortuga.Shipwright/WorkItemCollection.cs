using Microsoft.CodeAnalysis;

namespace Tortuga.Shipwright;

class WorkItemCollection : System.Collections.ObjectModel.KeyedCollection<INamedTypeSymbol, WorkItem>
{
	public WorkItemCollection() : base(SymbolEqualityComparer.Default) { }

	protected override INamedTypeSymbol GetKeyForItem(WorkItem item) => item.ContainerClass;
}
