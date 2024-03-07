using System.Globalization;
using System.Reflection;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB

using TypedLimitOption = Tortuga.Chain.SqlServerLimitOption;

#elif SQLITE
using TypedLimitOption = Tortuga.Chain.SQLiteLimitOption;
#elif POSTGRESQL
using  TypedLimitOption = Tortuga.Chain.PostgreSqlLimitOption;
#elif ACCESS
using  TypedLimitOption = Tortuga.Chain.AccessLimitOption;
#elif MYSQL

using TypedLimitOption = Tortuga.Chain.MySqlLimitOption;

#endif

namespace Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class DataAttribute : Attribute, ITestDataSource
{
	static DataSourceType[] s_DataSourceTypeList = new DataSourceType[] { DataSourceType.Normal,
													 DataSourceType.Open,
													 DataSourceType.Transactional,
													 DataSourceType.Strict };

	protected DataSourceType[] DataSourceTypeList = s_DataSourceTypeList;

	protected static JoinOptions[] JoinOptionsList = new JoinOptions[]{ JoinOptions.None,
				JoinOptions.IgnoreUnmatchedChildren,
				JoinOptions.MultipleParents,
				JoinOptions.Parallel,
				JoinOptions.MultipleParents | JoinOptions.Parallel };

	protected IEnumerable<DataSource> DataSources;

	protected static TypedLimitOption[] LimitOptionList = (TypedLimitOption[])Enum.GetValues(typeof(TypedLimitOption));

	DataSourceGroup m_DataSourceGroup;

	public DataAttribute(DataSourceGroup dataSourceGroup)
	{
		TestBase.SetupTestBase();

		m_DataSourceGroup = dataSourceGroup;
		DataSources = m_DataSourceGroup switch
		{
			DataSourceGroup.All => TestBase.s_DataSources.Values,
			DataSourceGroup.AllNormalOnly => TestBase.s_DataSources.Values,
			DataSourceGroup.Primary => new[] { TestBase.s_PrimaryDataSource },
			_ => throw new ArgumentOutOfRangeException(nameof(dataSourceGroup))
		};

		if (m_DataSourceGroup == DataSourceGroup.AllNormalOnly)
			DataSourceTypeList = new DataSourceType[] { DataSourceType.Normal };
	}

	public abstract IEnumerable<object[]> GetData(MethodInfo methodInfo);

	public string GetDisplayName(MethodInfo methodInfo, object[] data)
	{
		if (data != null)
			return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));

		return null;
	}
}
