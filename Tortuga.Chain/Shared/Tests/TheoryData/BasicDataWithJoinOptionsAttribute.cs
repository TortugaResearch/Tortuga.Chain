using System.Reflection;

namespace Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BasicDataWithJoinOptionsAttribute : DataAttribute
{
	public BasicDataWithJoinOptionsAttribute(DataSourceGroup dataSourceGroup) : base(dataSourceGroup)
	{
	}

	public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
	{
		foreach (var ds in DataSources)
			foreach (var dst in DataSourceTypeList)
				foreach (var option in JoinOptionsList)
					yield return new object[] { ds.Name, dst, option };
	}
}
