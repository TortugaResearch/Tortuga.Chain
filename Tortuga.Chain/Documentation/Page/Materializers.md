# Materializers

Materializers represent the `SELECT` part of the SQL generation process. They can be used to request the result of an operation be formatted as a scalar value, a list, an object, a collection, or a traditional `DataTable`. The base classes are public so that additional materializers can be created in the same fashion as the built-in ones.

Materializers are always linked directly to command builders. They are usually expressed as `.ToXxx` where Xxx is the type of result desired. There is also a special `AsNonQuery()` command builder for when you don't need anything back (e.g. insert/update operations). Many materializers allow you to set options that further refine their behavior.

Generally speaking, the classes that implement materializers are non-public. Instead you get back an `ILink` or `ILink<T>` wrapper. This is necessary as the concrete implementation of the materializer may change depending on the options you select. It is best to think of an `ILink` in the same way you think of an `IEnumerable` in a LINQ expression.

## Sql

All materializers offer a `Sql` method. This returns the SQL that would have been executed.

Note that this is not the only way to access the SQL being generated and is primarily meant to be used when debugging. For other situations, it is typical to access the SQL via the logging events and with a Trace appender. 

## Internals

When a materializer is executed, it first calls `Prepare` on the command builder it is linked to in order to generate an execution token. Immediately afterwards, it fires an ExecutionTokenPrepared event that appenders can listen for. (If you request the SQL from a materializer, only this step is performed.)

Next, the materializer calls Execute (or ExecuteAsync) on the execution token, passing in a callback. This callback is given a `DBCommand` object on which the materializer can call `ExecuteNonQuery`, `ExecuteScalar`, or `ExecuteReader` as appropriate.

The callback is expected to return an integer which is either null or the number of rows affected by the operation. Here is an example `Execute` method with callback:

        public override DataTable Execute(object state = null)
        {
            DataTable dt = new DataTable();
            ExecuteCore(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    dt.Load(reader);
                    return dt.Rows.Count;
                }
            }, state);

            return dt;
        } 

### Limitations

A strict limitation on materializers is that if they open an `IDataReader` they must close it inside their callback routine. There is no facility for holding open the connection associated with the data reader.

