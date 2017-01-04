# Entity Framework Integration

Chain can be combined with Entity Framework (EF) using the `ChainForEntityFramework` extension.

To use `ChainForEntityFramework`, first include the code at the botom of this page in your project. 

You'll need to register a Chain data source with a DBConext. Or more accurately, you are registering the DBConext's connection string. Normally you would do this at startup using a DBConext that you then discard.

Once you have done that, you may invoke Chain methods from the DBConext as shown.

    DBConext context = [...]
    context.Chain().From(...)

If your `DBConext` is currently participating in a transaction, any operations that Chain performs will also particpate in the transaction.

Do not cache the results of `DBConext.Chain`. The context will occasionally close the connection and calling `.Chain()` will reopen it if necessary. 

## ChainForEntityFramework Class

```csharp
    public static class ChainForEntityFramework
    {
        static ConcurrentDictionary<string, IRootDataSource> s_DataSources = new ConcurrentDictionary<string, IRootDataSource>();

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">An example context. Really the connection string is what's being registered.</param>
        /// <param name="dataSource">The data source.</param>
        public static void RegisterDataSource<T>(this DbContext context, T dataSource) where T : IRootDataSource, IClass2DataSource
        {
            s_DataSources[context.Database.Connection.ConnectionString] = dataSource;
        }

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">An example context. Really the connection string is what's being registered.</param>
        /// <param name="dataSourceFactory">The data source factory.</param>
        public static void RegisterDataSource<T>(this DbContext context, Func<string, T> dataSourceFactory) where T : IRootDataSource, IClass2DataSource
        {
            s_DataSources[context.Database.Connection.ConnectionString] = dataSourceFactory(context.Database.Connection.ConnectionString);
        }

        public static IClass2DataSource Chain(this DbContext context)
        {
            var connection = context.Database.Connection;
            var transaction = context.Database.CurrentTransaction?.UnderlyingTransaction;

            //If context.SaveChanges is called, the connection will be immediately closed.
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            return (IClass2DataSource)s_DataSources[context.Database.Connection.ConnectionString].CreateOpenDataSource(connection, transaction);
        }

    }
```

