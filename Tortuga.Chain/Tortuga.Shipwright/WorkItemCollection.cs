using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;

namespace Tortuga.Shipwright;

class WorkItemCollection : KeyedCollection<INamedTypeSymbol, WorkItem>
{
	public WorkItemCollection() : base(SymbolEqualityComparer.Default) { }

	protected override INamedTypeSymbol GetKeyForItem(WorkItem item) => item.ContainerClass;
}
