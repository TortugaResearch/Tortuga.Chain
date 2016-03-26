# Appenders

In the API, appenders are exposed as `ILink` or `ILink<T>` just like materializers. The difference is that while a materializer hooks onto a command builder, an appender hooks onto other links. This means you can string together as many appenders as you need to get the desired effect.

The mostly commonly used appenders deal with caching. You can also find appenders for tracing SQL calls, modifying command timeouts, and listening to change notification from the database (see SQL Dependency Appenders).

## Internals

Appends can be loosely grouped into these categories:

* Before execution
* After execution
* Instead of execution

Appenders can fit into more than one category.

### Before execution

A before execution appender is usually involved with the command building process in some way. Here we see one that alters the connection timeout:

        protected override void OnCommandBuilt(CommandBuiltEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e", "e is null.");
            e.Command.CommandTimeout = (int)m_Timeout.TotalSeconds;
        }

The tracing appenders also respond to this event, but they merely read the command text instead of modifying it. 

Another type of before execution appender is InvalidateCache, which you can see here:

        public override void Execute(object state = null)
        {
            DataSource.InvalidateCache(m_CacheKey);

            PreviousLink.Execute(state);
        }

Note that the ExecuteAsync method will also have to be overridden with nearly identical logic.

### After execution

After execution appenders tend to work on the cache. Here is one that caches the result of an operation.

        public override TResult Execute(object state = null)
        {
            var result = PreviousLink.Execute(state);
            DataSource.WriteToCache(new CacheItem(m_CacheKey ?? m_CacheKeyFunction(result), result, null), m_Policy);
            return result;
        }

### Instead of execution

The quintessential instead of appender is `ReadOrCache`. As the name implies, this will read a value from the cache, and if not found executes the query and caches the resulting value. Here is an example:

        public override TResult Execute(object state = null)
        {
            TResult result;
            if (PreviousLink.DataSource.TryReadFromCache(m_CacheKey, out result))
                return result;

            result = PreviousLink.Execute(state);

            DataSource.WriteToCache(new CacheItem(m_CacheKey, result, null), m_Policy);

            return result;
        }

As you can see, this appender has an effect that occurs both before and optionally after execution.

### Creating an Appender

Using the example above and the `Appender`, `Appender<TResult>`, or `Appender<Tin, TOut>` base class, you can implement your own appender. If your appender is database specific, you may need to cast the execution into the correct subtype.


