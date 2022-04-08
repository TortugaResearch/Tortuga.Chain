using System.Data.Common;

namespace Tortuga.Chain;

static class DbCommandExtensions
{
	public static IEnumerable<DbParameter> Enumerate(this DbParameterCollection collection)
	{
		for (int i = 0; i < collection.Count; i++)
		{
			yield return collection[i];
		}
	}
}
