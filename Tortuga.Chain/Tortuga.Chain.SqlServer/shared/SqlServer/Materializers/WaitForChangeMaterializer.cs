using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tortuga.Chain.SqlServer.Materializers
{
	internal static class WaitForChangeMaterializer
	{
		internal static Task GenerateTask<TCommandBuilder>(TCommandBuilder commandBuilder, CancellationToken cancellationToken, object? state)
			where TCommandBuilder : DbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
		{
			var materializer = new WaitForChangeMaterializer<TCommandBuilder>(commandBuilder);
			return materializer.GenerateTask(cancellationToken, state);
		}
	}
}

