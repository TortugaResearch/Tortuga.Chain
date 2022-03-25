using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Traits;



interface ICommandHelper<TObjectName, TDbType> : IDataSource
	where TObjectName : struct
	where TDbType : struct
{
	new DatabaseMetadataCache<TObjectName, TDbType> DatabaseMetadata { get; }
}