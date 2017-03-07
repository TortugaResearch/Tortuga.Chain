# Transactions

Basic transactions are created by calling `dataSource.BeginTransaction()`. This creates a transactional data source which can then be used just like a normal database.

Transactional data sources contain an open connection and transaction object, so they should be managed with a `using` statement just like any other limited resource.

```csharp
using (var trans = dataSource.BeginTransaction())
{
    var newKey = trans.Insert("Customer", customer).AsInt32().Execute();
    address.CustomerKey = newKey;
    trans.Insert("CustomerAddress", address).Execute();
    trans.Commit();
}
```

## Attaching to Active Connections and Transactions

There may be times when you need to attach a data source to an existing connection or transaction. This usually happens when mixing Chain with another ORM.

To do this, simply call `CreateOpenDataSource` on a normal data source and pass in the connection object. Optionally, you can also include a transaction object.

```csharp
var openDataSource = dataSource.CreateOpenDataSource( activeConnection, activeTransaction);
```

An open data source does not "own" the connection or transaction associated with it. This means that the open data source cannot close the connection or commit the transaction itself. 



