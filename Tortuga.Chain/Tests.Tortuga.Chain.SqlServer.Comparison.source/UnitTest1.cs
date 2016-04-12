using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class DapperReadmeVsChain

    {
        private static SqlServerDataSource s_SqlServerDataSource;

        static void DapperReadmeVsChain()
        {
            //This would be handled by your Dependency Injection framework
            s_SqlServerDataSource = SqlServerDataSource.CreateFromConfig("CodeFirstModels");
        }

        public class Dog
        {
            public int? Age { get; set; }
            public Guid Id { get; set; }
            public string Name { get; set; }
            public float? Weight { get; set; }

            public int IgnoredProperty { get { return 1; } }
        }

        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}
