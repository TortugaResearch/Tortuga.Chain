using Tortuga.Chain.SqlServer;

namespace Tests.Core;

[TestClass]
public class ObjectNameTests : TestBase
{
#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB

	[DataTestMethod]
	[DataRow("Employee", "employee")]
	[DataRow("HR.Employee", "hr.employee")]
	[DataRow("HR.Employee", "employee")]
	public void HashCodeTest(string nameA, string nameB)
	{
		var a = new SqlServerObjectName(nameA);
		var b = new SqlServerObjectName(nameB);
		Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), "Hash codes do not match");
	}

	[DataTestMethod]
	[DataRow("Employee", "employee", true)]
	[DataRow("HR.Employee", "hr.employee", true)]
	[DataRow("HR.Employee", "employee", false)]
	public void EqualityTest(string nameA, string nameB, bool expectedResult)
	{
		var a = new SqlServerObjectName(nameA);
		var b = new SqlServerObjectName(nameB);
		Assert.AreEqual(expectedResult, a.Equals(b));
		Assert.AreEqual(expectedResult, SqlServerObjectName.Equals(a, b));
	}

#endif
}
