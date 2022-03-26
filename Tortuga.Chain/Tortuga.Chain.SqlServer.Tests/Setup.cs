namespace Tests;

[TestClass]
public static class Setup
{
	[AssemblyCleanup]
	public static void AssemblyCleanup()
	{
	}

	[AssemblyInitialize]
	public static void AssemblyInit(TestContext context)
	{
		TestBase.SetupTestBase();
	}
}
