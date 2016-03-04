using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain;

namespace Tests.Repository
{
    public class Repository<TObject, TKey> where TObject : class, new()
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

        public TObject Get(TKey id)
        {
            return Source.From(TableName, GetKeyFilter(id)).AsObject<TObject>().Execute();
        }

        public IList<TObject> GetAll()
        {
            return Source.From(TableName).AsCollection<TObject>().Execute();
        }

        public TObject Insert(IReadOnlyDictionary<string, object> entity)
        {
            return Source.Insert(TableName, entity).AsObject<TObject>().Execute();
        }

        public TObject Insert(TObject entity)
        {
            return Source.Insert(TableName, entity).AsObject<TObject>().Execute();
        }

        public void Update(TObject entity)
        {
            Source.Update(TableName, entity).Execute();
        }

        public void Delete(TKey id)
        {
            Source.Delete(TableName, GetKeyFilter(id)).Execute();
        }

        internal void Update(IReadOnlyDictionary<string, object> entity)
        {
            Source.Update(TableName, entity).Execute();
        }
    }

}

