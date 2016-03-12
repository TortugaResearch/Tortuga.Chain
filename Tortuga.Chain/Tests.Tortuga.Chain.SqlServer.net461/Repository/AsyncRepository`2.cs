using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tortuga.Chain;

namespace Tests.Repository
{

    public class AsyncRepository<TEntity, TKey> where TEntity : class, new()
    {

        protected string TableName { get; private set; }
        protected string PrimaryKey { get; private set; }

        protected IDictionary<string, object> GetKeyFilter(TKey id)
        {
            return new Dictionary<string, object>() { { PrimaryKey, id } };
        }

        protected IClass1DataSource Source { get; private set; }

        public AsyncRepository(IClass1DataSource source, string tableName)
        {
            Source = source;
            TableName = tableName;
            PrimaryKey = Source.DatabaseMetadata.GetTableOrView(tableName).Columns.Single(c => c.IsPrimaryKey).ClrName;
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            return Source.From(TableName, GetKeyFilter(id)).ToObject<TEntity>().ExecuteAsync();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return Source.From(TableName).ToCollection<TEntity>().ExecuteAsync();
        }

        public Task<TEntity> InsertAsync(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().ExecuteAsync();
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            return Source.Insert(TableName, entity).ToObject<TEntity>().ExecuteAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            return Source.Update(TableName, entity).ExecuteAsync();
        }

        public Task DeleteAsync(TKey id)
        {
            return Source.Delete(TableName, GetKeyFilter(id)).ExecuteAsync();
        }

        public Task UpdateAsync(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Update(TableName, entity).ExecuteAsync();
        }

        public Task<List<Employee>> QueryAsync(string whereClause, object argumentValue)
        {
            return Source.From(TableName, whereClause, argumentValue).ToCollection<Employee>().ExecuteAsync();
        }

        public Task<List<Employee>> QueryAsync(object filterValue)
        {
            return Source.From(TableName, filterValue).ToCollection<Employee>().ExecuteAsync();
        }
    }

}

