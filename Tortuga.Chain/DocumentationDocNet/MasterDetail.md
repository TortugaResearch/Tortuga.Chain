# Master/Detail Records


## Loading from Multiple Recordsets

If you have a stored procedure that returns a pair of recordsets, you can use the `Join` appender to match master and child records.

```
dataSource.Procedure("GetCustomersWithOrders", SearchParameter).ToCollectionSet<Customer, Order>().Join(nameof(Customer.CustomerKey), nameof(Customer.Orders)).Execute();
```

The Join appender takes two collections and maps the detail records to their parent records. 

The join specifier can be one of the following:

* A predicate function (useful for multi-key relationships)
* The name of a key. e.g. CustomerKey
* The name of a primary key and the name of foreign key. e.g. Id/CustomerId

You must also provide the collection property on the master object that will accept the detail objects. This can be done as a lambda expression, `c => x.Orders` or by property name.

## Options

By default, each detail record is matched to one and only one master record. If it can't be matched, an error occurs. Other options include:

* `MultipleParents`: Each detail record can be matched to multiple parent records.
* `IgnoreUnmatchedChildren`: Silently discard unmatched detail records.
* `Parallel`: Use PLINQ to perform the join in parallel. 


## Internals

The mapping occurs entirely in memory.

## Roadmap

Take one result-set and split it into master and detail records.