# Bulk Insert

* `ds.InsertBulk(tableName, tableTypeName, DataTable, Options, BatchSize)`
* `ds.InsertBulk(tableName, tableTypeName, DbDataReader, Options, BatchSize)`
* `ds.InsertBulk(tableName, tableTypeName, IEnumerable<TObject>, Options, BatchSize)`

Bulk inserts are the fastest way to populate a database. However, they have several limitions including being unable to return the newly created primary keys. Audit rules are not in effect when using a bulk insert.


## Streaming (SQL Server)

To enable streaming to SQL Server database, use the `.WithStreaming()` option. 

Streaming can be used in conjucntion with the `IEnumerable< TObject >` option, batch insert supports streaming objects into the database from a lazily initialized source.

When using this option, records are still batched according to the batch size option.

## Limitations

Audit rules are not in effect. If you to override values such as CreatedDate/CreatedBy, use `InsertBatch` instead.

## Examples


```csharp
var count = DataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
```

### With streaming

```csharp
var count = DataSource.InsertBulk.WithStreaming(EmployeeTableName, employeeSource).Execute();
```