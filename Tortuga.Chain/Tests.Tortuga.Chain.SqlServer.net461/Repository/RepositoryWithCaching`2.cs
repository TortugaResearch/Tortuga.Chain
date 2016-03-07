using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain;

namespace Tests.Repository
{
    public class RepositoryWithCaching<TObject, TKey> where TObject : class, new()
    {

        protected string TableName { get; private set; }
        protected string PrimaryKey { get; private set; }

        protected IDictionary<string, object> GetKeyFilter(TKey id)
        {
            return new Dictionary<string, object>() { { PrimaryKey, id } };
        }

        protected IClass1DataSource Source { get; private set; }



        protected string AllCacheKey { get; private set; }
        public CacheItemPolicy Policy { get; private set; }

        public RepositoryWithCaching(IClass1DataSource source, string tableName, CacheItemPolicy policy = null)
        {
            Policy = policy;
            Source = source;
            TableName = tableName;
            PrimaryKey = Source.DatabaseMetadata.GetTableOrView(tableName).Columns.Single(c => c.IsPrimaryKey).ClrName;
            AllCacheKey = $"{TableName} ALL";
        }

        protected string CacheKey(TKey id)
        {
            return $"{TableName} {PrimaryKey}={id}";
        }

        protected string CacheKey(TObject entity)
        {
            var id = (TKey)MetadataCache.GetMetadata(entity.GetType()).Properties[PrimaryKey].InvokeGet(entity);
            return CacheKey(id);
        }

        public TObject Get(TKey id)
        {
            return Source.From(TableName, GetKeyFilter(id)).AsObject<TObject>().ReadOrCache(CacheKey(id), policy: Policy).Execute();
        }

        public IList<TObject> GetAll()
        {
            return Source.From(TableName).AsCollection<TObject>().CacheAllItems((TObject x) => CacheKey(x), policy: Policy).ReadOrCache(AllCacheKey, policy: Policy).Execute();
        }

        public TObject Insert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).AsObject<TObject>().InvalidateCache(AllCacheKey).Cache((TObject x) => CacheKey(x), policy: Policy).Execute();
        }

        public TObject Insert(TObject entity)
        {
            return Source.Insert(TableName, entity).AsObject<TObject>().InvalidateCache(AllCacheKey).Cache((TObject x) => CacheKey(x), policy: Policy).Execute();
        }

        public void Update(TObject entity)
        {
            Source.Update(TableName, entity).InvalidateCache(AllCacheKey).Execute();
            Source.WriteToCache(new CacheItem(CacheKey(entity), entity), Policy);
        }

        public void Delete(TKey id)
        {
            Source.Delete(TableName, GetKeyFilter(id)).InvalidateCache(CacheKey(id)).InvalidateCache(AllCacheKey).Execute();
        }

        internal void Update(IReadOnlyDictionary<string, object> entity)
        {
            Source.Update(TableName, entity).InvalidateCache(CacheKey((TKey)entity[PrimaryKey])).InvalidateCache(AllCacheKey).Execute();
        }
    }
}
