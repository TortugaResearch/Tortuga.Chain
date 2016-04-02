#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Metadata
{
    [TestClass]
    public class MetadataTests : TestBase
    {
        [TestMethod]
        public void PreloadTables()
        {
            DataSource.DatabaseMetadata.PreloadTables();
        }

        [TestMethod]
        public void PreloadViews()
        {
            DataSource.DatabaseMetadata.PreloadViews();
        }
    }
}
