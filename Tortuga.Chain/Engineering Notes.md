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

