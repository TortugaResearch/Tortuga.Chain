using CSScriptLib;
using System.Collections.Concurrent;

namespace Tortuga.Chain.Materializers;

sealed internal class CompilerCache
{
	readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> m_Cache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

	public MethodDelegate<TObject>? GetBuilder<TObject>(string sql)
		where TObject : new()
	{
		var cache2 = m_Cache.GetOrAdd(typeof(TObject), x => new ConcurrentDictionary<string, object>());
		cache2.TryGetValue(sql, out var result);
		return (MethodDelegate<TObject>?)result;
	}

	internal void StoreBuilder<TObject>(string sql, MethodDelegate<TObject> builder) where TObject : new()
	{
		var cache2 = m_Cache.GetOrAdd(typeof(TObject), x => new ConcurrentDictionary<string, object>());
		cache2.TryAdd(sql, builder);
	}
}
