using System.Reflection;

[assembly: AssemblyTitle("Tortuga.Chain.SqlServer")]
[assembly: AssemblyDescription("Fluent ORM for .NET and SQL Server.")]

[assembly: AssemblyCulture("")]


#if NETSTANDARD1_3 || NETSTANDARD2_0
[assembly: AssemblyVersion("1.1")]
#else
[assembly: AssemblyVersion("1.1.*")]
#endif
