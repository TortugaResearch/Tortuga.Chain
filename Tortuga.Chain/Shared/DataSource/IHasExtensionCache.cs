using System.Collections.Concurrent;

namespace Traits;

internal interface IHasExtensionCache
{
	ConcurrentDictionary<Type, object> ExtensionCache { get; }
}
