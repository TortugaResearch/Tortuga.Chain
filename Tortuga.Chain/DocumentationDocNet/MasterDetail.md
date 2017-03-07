# Master/Detail Records



## Loading from Multiple Recordsets

If you have a stored procedure that returns a pair of recordsets, you can use the `Join` appender to match master and child records.

```
dataSource.Procedure("GetCustomersWithOrders", SearchParameter).ToCollectionSet<Customer, Order>().Join(nameof(Customer.CustomerKey), nameof(Customer.Orders), options).Execute();
```

The Join appender takes two collections and maps the detail records to their parent records. 

The join specifier can be one of the following:

* A predicate function (useful for multi-key relationships)
* The name of a key. e.g. CustomerKey
* The name of a primary key and the name of foreign key. e.g. Id/CustomerId

You must also provide the collection property on the master object that will accept the detail objects. This can be done as a lambda expression, `c => x.Orders` or by property name.

## Internals

The mapping occurs entirely in memory.

## Roadmap

Take one result-set and split it into master and detail records.