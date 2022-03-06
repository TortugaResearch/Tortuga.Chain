using System.Reflection;

namespace Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TablesAndViewColumnsDataAttribute : DataAttribute
{
	public TablesAndViewColumnsDataAttribute(DataSourceGroup dataSourceGroup) : base(dataSourceGroup)
	{
	}

	public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
	{
		foreach (var ds in DataSources)
		{
			ds.DatabaseMetadata.Preload();

			foreach (var table in ds.DatabaseMetadata.GetTablesAndViews())
				foreach (var dst in DataSourceTypeList)
					foreach (var column in table.Columns)
						yield return new object[] { ds.Name, dst, table.Name, column.SqlName };
		}
	}
}
