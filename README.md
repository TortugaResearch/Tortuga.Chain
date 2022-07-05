# Tortuga Chain

A Fluent ORM for .NET

## Documentation

* [Documentation Website](https://tortugaresearch.github.io/Tortuga.Chain)
* [API Reference](https://tortugaresearch.github.io/Tortuga.Chain/API/Index.html)
* [Chain Wiki](https://github.com/docevaad/Chain/wiki)
* [Chain vs Dapper](https://github.com/docevaad/Chain/wiki/A-Chain-comparison-to-Dapper)
* The [change log](Tortuga.Chain/Changelog.md) starts with version 4.0. 

## Getting Started

To get started with Chain, you need to create a data source. This can be done using a connection string or a `SqlConnectionStringBuilder`. Optionally, you can also name your data source. (This has no functional effect, but does assist in logging.)

```csharp 
dataSource = new Tortuga.Chain.SqlServerDataSource("Adventure DB", "Server=.;Database=AdventureWorks2014;Trusted_Connection=True;");
```

Or from your app.config file:

```csharp 
dataSource = Tortuga.Chain.SqlServerDataSource.CreateFromConfig("AdventureDB");
```

Your data source should be treated as a singleton object; you only need one per unique connection string. This is important because your data source will cache information about your database.

We recommend calling dataSource.Test() when your application starts up. This verifies that you can actually connect to the database.

## Connection Management

A major difference between Chain and other ORMs is that you don't need to manage connections or data contexts. A Chain data source is designed to be completely thread safe and will handle connection lifetime for you automatically.

## Transactions

Transactions still need to contained within a `using` statement and explicitly committed. You can create one by calling `dataSource.BeginTransaction`.

## Command Chains

Command chains are the primary way of working with Tortuga. Each link in the chain is used to inform the previous link about what actions are desired. Here is a basic example:

```csharp 
dataSource.Procedure("uspGetEmployeeManagers", new {@BusinessEntityID = 100}).ToCollection<Manager>().Execute();
```

Breaking this down, we have:

* The data source
* The command being performed
* How the results of the command should be returned
* If the operation should be executed synchronously or asynchronously

### Commands

The list of available commands depends on the data source. Most data sources support 

* Raw sql
* Table/View queries
* Insert, Update, and Delete operations (some also include 'upserts')

Advanced ones may also include

* Stored procedures and/or Table Value Functions
* Batch insert, a.k.a. bulk copy

Most commands accept a parameter object. The parameter object can be a normal class, a dictionary of type `IDictionary<string, object>`, or a list of appropriate DbParameter objects.

Chain command builders honor .NET's `NotMapped` and `Column` attributes.

### Materializers

Materializers are an optional link, you only need them if you want something back from the database.

An interesting feature of the materializer is that it participates in SQL generation. For example, if you use the `ToObject<T>` or `ToCollection<T>` materializer, then it will read the list of properties on class T. That list of properties will be used to generate the SELECT clause, ensuring that you don't pull back more information than you actually need. This in turn means that indexes are used more efficiently and performance is improved.

Materializers call into several categories:

* Scalar: `ToInt`, `ToIntOrNull`, `ToString`
* Row: `ToRow`, `ToDataRow`, `ToObject`
* Table: `ToTable`, `ToDataTable`, `ToCollection`
* Multiple Tables: `ToTableSet`, `ToDataSet`

For better performance, you can use the compiled materializer extension:

* Row: `.Compile().ToObject<TObject>()`
* Table: `.Compile().ToCollection<TObject>()`, `.Compile().ToCollection<TList, TObject>()`

This requires the `Tortuga.Chain.CompiledMaterializers` package, which includes CS-Script as a dependency. 

### CRUD Operations

By combining commands and materializers, you can perform all of the basic CRUD operations. Here are some examples.

#### Create

```csharp 
var vehicleKey = dataSource.Insert("Vehicle", new { VehicleID = "65476XC54E", Make = "Cadillac", Model = "Fleetwood Series 60", Year = 1955 }).ToInt32().Execute();
```

#### Read

```csharp
var car = dataSource.GetById("Vehicle", vehicleKey).ToObject<Vehicle>().Execute();
var cars = dataSource.From("Vehicle", new { Make = "Cadillac" }).ToCollection<Vehicle>().Execute();
```

#### Update

```csharp
dataSource.Update("Vehicle", new { VehicleKey = vehicleKey, Year = 1957 }).Execute();
```

#### Delete

```csharp
dataSource.Delete("Vehicle", new { VehicleKey = vehicleKey }).Execute();
```

### Appenders

Appenders are links that can change the rules before, during, or after execution.  An appender can be added after a materializer or another appender.

Caching appenders include:

* `Cache`: Writes to the cache, overwriting any previous value. (Use with Update and Procedure operations.)
* `ReadOrCache`: If it can read from the cache, the database operation is aborted. Otherwise the value is cached. 
* `CacheAllItems`: Cache each item in the result list individually. Useful when using a GetAll style operation.
* `InvalidateCache`: Removes a cache entry. Use with any operation that modifies a record.

Here is an example of CRUD operations using caching.

```csharp
var car = dataSource.GetById("Vehicle", vehicleKey).ToObject<Vehicle>().ReadOrCache("Vehicle " + vehicleKey).Execute();
car = dataSource.Update("Vehicle", new { VehicleKey = vehicleKey, Year = 1957 }).ToObject<Vehicle>().Cache("Vehicle " + vehicleKey).Execute();
dataSource.Delete("Vehicle", new { VehicleKey = vehicleKey }).InvalidateCache("Vehicle " + vehicleKey.Execute();
```

If using SQL Server, you can also use `WithChangeNotification`. This uses SQL Dependency to listen for changes to the table(s) you queried.

When debugging applications, it is often nice to dump the SQL somewhere. This is where the tracing appenders come into play.

* `WithTracing`: Writes to an arbitrary TextWriter style stream.
* `WithTracingToConsole`: Writes to the Console window
* `WithTracingToDebug`: Writes to the Debug window

You can also override DBCommand settings such as the command timeout. For example:

```csharp 
ds.Procedure("ExpensiveReport").ToDataSet().SetTimeout(TimeSpan.FromHours(3)).Execute()
```

### Execution Modes

The final link in any chain is the execution mode. There are two basic options:

* `Execute()`
* `ExecuteAsync()`

Both options accept a `state` parameter. This has no direct effect, but can be used to facilitate logging. `ExecuteAsync` also accepts an optional cancellation token.

