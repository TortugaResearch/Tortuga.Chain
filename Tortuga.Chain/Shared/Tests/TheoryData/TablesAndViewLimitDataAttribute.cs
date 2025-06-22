using System.Reflection;

#if SQL_SERVER_MDS || SQL_SERVER_OLEDB || POSTGRESQL

using Tortuga.Chain;

#endif

namespace Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TablesAndViewLimitDataAttribute : DataAttribute
{
	public TablesAndViewLimitDataAttribute(DataSourceGroup dataSourceGroup) : base(dataSourceGroup)
	{
	}

	public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
	{
		foreach (var ds in DataSources)
		{
			ds.DatabaseMetadata.Preload();

			foreach (var table in ds.DatabaseMetadata.GetTablesAndViews())
				foreach (var dst in DataSourceTypeList)
					foreach (var limitType in LimitOptionList)
					{
#if SQL_SERVER_MDS || SQL_SERVER_OLEDB
						//Cannot use table sample with views
						if (!table.IsTable && limitType == SqlServerLimitOption.TableSampleSystemPercentage)
							continue;
						if (!table.IsTable && limitType == SqlServerLimitOption.TableSampleSystemRows)
							continue;
#elif POSTGRESQL
							//Cannot use table sample with views
							if (!table.IsTable && limitType == PostgreSqlLimitOption.TableSampleBernoulliPercentage)
								continue;
							if (!table.IsTable && limitType == PostgreSqlLimitOption.TableSampleSystemPercentage)
								continue;
#endif
						yield return new object[] { ds.Name, dst, table.Name, limitType };
					}
		}
	}
}
