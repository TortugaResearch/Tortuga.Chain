using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
    public class PostgreSqlTransactionalDataSource : PostgreSqlDataSourceBase, IDisposable
    {
        internal PostgreSqlTransactionalDataSource(DataSourceSettings settings) : base(settings)
        {
            throw new NotImplementedException();
        }

        public override PostgreSqlMetadataCache DatabaseMetadata
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected override void Execute(ExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, Func<NpgsqlCommand, int?> implementation, object state)
        {
            throw new NotImplementedException();
        }

        protected override Task ExecuteAsync(ExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, Func<NpgsqlCommand, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }
}   