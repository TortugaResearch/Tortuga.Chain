#if RUNTIME_CACHE_MISSING

using System;
using System.Threading.Tasks;
using Tortuga.Anchor.Collections;

namespace Tortuga.Chain.Core
{
    /// <summary>
    /// This is not a production-grade cache. It is just a placeholder until we have a real one for UAP apps.
    /// </summary>
    /// <seealso cref="ICacheAdapter" />
    internal class SimpleCache : ICacheAdapter
    {
        readonly WeakReferenceCollection<Tuple<string, object>> m_Cache = new WeakReferenceCollection<Tuple<string, object>>();
        readonly object SyncRoot = new object();

        public void Clear()
        {
            lock (SyncRoot)
            {
                m_Cache.Clear();
            }
        }

        public Task ClearAsync()
        {
            Clear();
            return Task.CompletedTask;
        }

        public void Invalidate(string cacheKey)
        {
            lock (SyncRoot)
            {
                foreach (var item in m_Cache)
                    if (item.Item1 == cacheKey)
                    {
                        m_Cache.Remove(item);
                        return;
                    }
            }
        }

        public Task InvalidateAsync(string cacheKey)
        {
            Invalidate(cacheKey);
            return Task.CompletedTask;
        }

        public bool TryRead<T>(string cacheKey, out T result)
        {
            lock (SyncRoot)
            {
                foreach (var item in m_Cache)
                    if (item.Item1 == cacheKey)
                    {
                        result = (T)item.Item2;
                        return true;
                    }
            }
            result = default(T);
            return false;
        }

        public Task<CacheReadResult<T>> TryReadAsync<T>(string cacheKey)
        {
            lock (SyncRoot)
            {
                foreach (var item in m_Cache)
                    if (item.Item1 == cacheKey)
                        return Task.FromResult(new CacheReadResult<T>(true, (T)item.Item2));
            }
            return Task.FromResult(new CacheReadResult<T>(false, default(T)));
        }

        public void Write(string cacheKey, object value, CachePolicy policy)
        {
            lock (SyncRoot)
            {
                foreach (var item in m_Cache)
                    if (item.Item1 == cacheKey)
                    {
                        m_Cache.Remove(item);
                        break;
                    }

                m_Cache.Add(new Tuple<string, object>(cacheKey, value));
            }

        }

        public Task WriteAsync(string cacheKey, object value, CachePolicy policy)
        {
            Write(cacheKey, value, policy);
            return Task.CompletedTask;
        }
    }
}
#endif