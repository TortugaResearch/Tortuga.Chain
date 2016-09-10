using System.Collections.Generic;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Repository
{
    public class EmployeeCachingRepository
    {

        private const string TableName = "HR.Employee";
        private const string AllCacheKey = "HR.Employee ALL";

        public IClass1DataSource Source { get; private set; }
        public CachePolicy Policy { get; private set; }

        public EmployeeCachingRepository(IClass1DataSource source, CachePolicy policy = null)
        {
            Source = source;
            Policy = policy;
        }

        protected string CacheKey(int id)
        {
            return $"HR.Employee EmployeeKey={id}";
        }

        protected string CacheKey(Employee entity)
        {
            return CacheKey(entity.EmployeeKey.Value);
        }

        public Employee Get(int id)
        {
            return Source.GetByKey(TableName, id).ToObject<Employee>().ReadOrCache(CacheKey(id), policy: Policy).Execute();
        }

        public IList<Employee> GetAll()
        {
            return Source.From(TableName).ToCollection<Employee>().CacheAllItems((Employee x) => CacheKey(x), policy: Policy).ReadOrCache(AllCacheKey, policy: Policy).Execute();
        }

        public Employee Insert(Employee entity)
        {
            return Source.Insert(TableName, entity).ToObject<Employee>().InvalidateCache(AllCacheKey).Cache((Employee x) => CacheKey(x), policy: Policy).Execute();
        }

        public Employee Update(Employee entity)
        {
            return Source.Update(TableName, entity).ToObject<Employee>().Cache(CacheKey(entity)).InvalidateCache(AllCacheKey).Execute();
        }

        public void Delete(int id)
        {
            Source.DeleteByKey(TableName, id).InvalidateCache(CacheKey(id)).InvalidateCache(AllCacheKey).Execute();
        }
    }
}
