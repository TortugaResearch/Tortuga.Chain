using Microsoft.VisualStudio.TestTools.UnitTesting;

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
