using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain;

namespace Tests.Repository
{

    public class Repository<TEntity, TKey> where TEntity : class, new()
    {

        protected string TableName { get; private set; }
        protected string PrimaryKey { get; private set; }

        protected IDictionary<string, object> GetKeyFilter(TKey id)
        {
            return new Dictionary<string, object>() { { PrimaryKey, id } };
        }

        protected IClass1DataSource Source { get; private set; }

        public Repository(IClass1DataSource source, string tableName)
        {
            Source = source;
            TableName = tableName;
            PrimaryKey = Source.DatabaseMetadata.GetTableOrView(tableName).Columns.Single(c => c.IsPrimaryKey).ClrName;
        }

        public TEntity Get(TKey id)
        {
            return Source.From(TableName, GetKeyFilter(id)).ToObject<TEntity>().Execute();
        }

        public IList<TEntity> GetAll()
        {
            return Source.From(TableName).ToCollection<TEntity>().Execute();
        }

        public TEntity Insert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().Execute();
        }

        public TEntity Insert(TEntity entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().Execute();
        }

        public TEntity Update(TEntity entity)
        {
            return Source.Update(TableName, entity).ToObject<TEntity>().Execute();
        }

        public void Delete(TKey id)
        {
            Source.Delete(TableName, GetKeyFilter(id)).Execute();
        }

        public TEntity Update(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Update(TableName, entity).ToObject<TEntity>().Execute();
        }

        public TEntity Upsert(TEntity entity)
        {
            return Source.Upsert(TableName, entity).ToObject<TEntity>().Execute();
        }

        public TEntity Upsert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Upsert(TableName, entity).ToObject<TEntity>().Execute();
        }

        public IList<TEntity> Query(string whereClause, object argumentValue)
        {
            return Source.From(TableName, whereClause, argumentValue).ToCollection<TEntity>().Execute();
        }

        public IList<TEntity> Query(object filterValue)
        {
            return Source.From(TableName, filterValue).ToCollection<TEntity>().Execute();
        }
    }

}

