using System;
using System.Threading;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Tests.ComparisonTests.AssemblyInit(null);
            Console.ReadLine();
            var c = new Tests.ComparisonTests();
            c.ChainCompiled_CrudTest();
        }
    }
}
