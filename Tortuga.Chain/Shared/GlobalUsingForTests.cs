global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

#if CLASS_2 || CLASS_3

global using System.Runtime.CompilerServices;

#endif

#if SQLITE

global using KeyType = System.Int64;

#elif MYSQL

global using KeyType = System.UInt64;

#else

global using KeyType = System.Int32;

#endif
