using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain;

namespace Tests.Repository
{
    public class RepositoryWithCaching<TEntity, TKey> where TEntity : class, new()
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

        protected string CacheKey(TEntity entity)
        {
            var id = (TKey)MetadataCache.GetMetadata(entity.GetType()).Properties[PrimaryKey].InvokeGet(entity);
            return CacheKey(id);
        }

        public TEntity Get(TKey id)
        {
            return Source.From(TableName, GetKeyFilter(id)).ToObject<TEntity>().ReadOrCache(CacheKey(id), policy: Policy).Execute();
        }

        public IList<TEntity> GetAll()
        {
            return Source.From(TableName).ToCollection<TEntity>().CacheAllItems((TEntity x) => CacheKey(x), policy: Policy).ReadOrCache(AllCacheKey, policy: Policy).Execute();
        }

        public TEntity Insert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().InvalidateCache(AllCacheKey).Cache((TEntity x) => CacheKey(x), policy: Policy).Execute();
        }

        public TEntity Insert(TEntity entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().InvalidateCache(AllCacheKey).Cache((TEntity x) => CacheKey(x), policy: Policy).Execute();
        }

        public TEntity Update(TEntity entity)
        {
            return Source.Update(TableName, entity).ToObject<TEntity>().Cache(CacheKey(entity)).InvalidateCache(AllCacheKey).Execute();
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
