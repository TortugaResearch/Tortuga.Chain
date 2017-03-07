# NHibernate Integration

Chain can be combined with NHibernate using the `ChainForNHibernate` extension.

To use `ChainForNHibernate`, first include the code at the botom of this page in your project. 

You'll need to register a Chain data source for each ISessionFactory. Or more accurately, you are registering the ISessionFactory's connection string. Normally you would do this at startup just after creating the ISessionFactory itself.

Once you have done that, you may invoke Chain methods from the ISession as shown.

    ISesion session = [...]
    session.Chain().From(...)

If your `ISession` is currently participating in a transaction, any operations that Chain performs will also particpate in the transaction.

You may wish to call `ISession.Flush()` before invoking Chain commands if the order of operations is important. Chain will not implicitly call `Flush` for you.

You can improve memory usage slighly by caching the results of `ISession.Chain` for the lifetime of the ISession object. The difference should be minimal, as most of the internal state such as the metadata cache is shared with the root data source that you registered with the `ISessionFactory`. 

## ChainForNHibernate Class

```csharp
    public static class ChainForNHibernate
    {
        static ConcurrentDictionary<string, IRootDataSource> s_DataSources = new ConcurrentDictionary<string, IRootDataSource>();

        public static void RegisterDataSource<T>(this ISessionFactory sessionFactory, T dataSource) where T : IRootDataSource, IClass2DataSource
        {
            using (var session = sessionFactory.OpenSession())
            {
                s_DataSources[session.Connection.ConnectionString] = dataSource;
            }
        }

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionFactory">The session factory. The input parameter is a connection string extracted from the session factory.</param>
        /// <param name="dataSourceFactory">The data source factory.</param>
        public static void RegisterDataSource<T>(this ISessionFactory sessionFactory, Func<string, T> dataSourceFactory) where T : IRootDataSource, IClass2DataSource
        {
            using (var session = sessionFactory.OpenSession())
            {
                s_DataSources[session.Connection.ConnectionString] = dataSourceFactory(session.Connection.ConnectionString);
            }
        }

        public static IClass2DataSource Chain(this ISession session)
        {

            //These casts won't be necessary with Chain Version 1.1.
            DbConnection connection = (DbConnection)session.Connection;
            DbTransaction transaction = (DbTransaction)GetTransaction(session);

            return (IClass2DataSource)s_DataSources[session.Connection.ConnectionString].CreateOpenDataSource(connection, transaction);
        }

        //http://ayende.com/blog/1583/i-hate-this-code

        private static IDbTransaction GetTransaction(ISession session)

        {
            using (var command = session.Connection.CreateCommand())
            {
                session.Transaction.Enlist(command);
                return command.Transaction;
            }
        }

    }
```

