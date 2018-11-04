using System.Reflection;

[assembly: AssemblyTitle("Tortuga.Chain.Core")]
[assembly: AssemblyDescription("A fluent ORM for .NET.")]
[assembly: AssemblyCulture("")]
#if NETSTANDARD1_3 || NETSTANDARD2_0
[assembly: AssemblyVersion("2.0")]
#else
[assembly: AssemblyVersion("2.0.*")]
#endif