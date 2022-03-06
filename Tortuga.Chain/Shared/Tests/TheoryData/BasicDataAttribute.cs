using System.Reflection;

namespace Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BasicDataAttribute : DataAttribute
{
	public BasicDataAttribute(DataSourceGroup dataSourceGroup) : base(dataSourceGroup)
	{
	}

	public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
	{
		foreach (var ds in DataSources)
			foreach (var dst in DataSourceTypeList)
				yield return new object[] { ds.Name, dst };
	}
}
