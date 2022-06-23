using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Traits;

interface ICommandHelper<TParameter, TObjectName, TDbType> : IDataSource
	where TObjectName : struct
	where TDbType : struct
	where TParameter : DbParameter
{
	new DatabaseMetadataCache<TParameter, TObjectName, TDbType> DatabaseMetadata { get; }
}
