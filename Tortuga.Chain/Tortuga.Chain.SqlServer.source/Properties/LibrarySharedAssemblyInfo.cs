using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Tortuga.Chain.SqlServer")]
[assembly: AssemblyDescription("Fluent ORM for .NET and SQL Server.")]
[assembly: AssemblyCulture("")]
#if NETSTANDARD2_0
[assembly: AssemblyVersion("2.1")]
#else
[assembly: AssemblyVersion("2.1.*")]
#endif

[assembly: InternalsVisibleTo("Tortuga.Chain.SqlServer.OleDb")]