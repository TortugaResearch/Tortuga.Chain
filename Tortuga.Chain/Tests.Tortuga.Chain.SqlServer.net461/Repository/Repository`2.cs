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
            return Source.From(TableName, GetKeyFilter(id)).AsObject<TEntity>().Execute();
        }

        public IList<TEntity> GetAll()
        {
            return Source.From(TableName).AsCollection<TEntity>().Execute();
        }

        public TEntity Insert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).AsObject<TEntity>().Execute();
        }

        public TEntity Insert(TEntity entity)
        {
            return Source.Insert(TableName, entity).AsObject<TEntity>().Execute();
        }

        public void Update(TEntity entity)
        {
            Source.Update(TableName, entity).Execute();
        }

        public void Delete(TKey id)
        {
            Source.Delete(TableName, GetKeyFilter(id)).Execute();
        }

        public void Update(IReadOnlyDictionary<string, object> entity)
        {
            Source.Update(TableName, entity).Execute();
        }

        public IList<Employee> Query(string whereClause, object argumentValue)
        {
            return Source.From(TableName, whereClause, argumentValue).AsCollection<Employee>().Execute();
        }

        public IList<Employee> Query(object filterValue)
        {
            return Source.From(TableName, filterValue).AsCollection<Employee>().Execute();
        }
    }

}

