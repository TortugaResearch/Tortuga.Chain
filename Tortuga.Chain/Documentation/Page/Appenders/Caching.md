# Caching Appenders

* `.Cache(...)`
* `.CacheAllItems(...)`
* `.InvalidateCache(...)`
* `.ReadOrCache(...)`

## Cache Keys

Cache keys can be provided as a string or a function. In the latter case, the function accepts the result of the chain and returns a string.

`.CacheAllItems(...)` is a special case. It only operates on lists of objects and generates a cache key for each object seperately.

## Example of a Caching Repository

In this example you can see the interplay between caching individual records and caching collections using `CacheAllItems`.


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
    

## Internals

The caching appenders use the data source to access a cache. Currently that data source is hard-coded to use .NET's built-in `MemoryCache`.

## Roadmap

The current plan is to allow the caching framework to be swapped out. 
