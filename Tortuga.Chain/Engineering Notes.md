## Generics

When multiple generic parameters are needed, they should always be in this order.


```
<TConnection, TTransaction, TCommand, TParameter, TObjectName, TDbType>
	where TConnection : DbConnection
	where TTransaction : DbTransaction
	where TCommand : DbCommand
	where TParameter : DbParameter
	where TObjectName : struct
	where TDbType : struct
```

When used...

```
<AbstractConnection, AbstractTransaction, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>
```

## Aggregates

Standard aggregate functions are listed in `AggregateType`.

To convert the enum to a database specific function, use `DatabaseMetadataCache.GetAggregateFunction`.

If the database doesn't support a given aggregation, override `GetAggregateFunction` and throw a `NotSupportedException`.
