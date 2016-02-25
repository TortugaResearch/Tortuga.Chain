#Tortuga Chain
A fluent micro-ORM for .NET

##Getting Started

To get started with Chain, you need to create a data source. This can be done using a connection string or a `SqlConnectionStringBuilder`. Optionally, you can also name your data source. (This has no functional effect, but does assist in logging.)

    dataSource = new Tortuga.Chain.SqlServerDataSource("Adventure DB", "Server=.;Database=AdventureWorks2014;Trusted_Connection=True;");

Your data source should be treated as a singleton object; you only need one per unique connection string. This is important because your data source will cache information about your database.

We recommend calling dataSource.Test() when your application starts up. This verifies that you can actually connect to the database.

##Connection Management

A major difference between Chain and other ORMs is that you don't need to manage connections or data contexts. A Chain data source is designed to be completely thread safe and will handle connection lifetime for you automatically.

##Transactions

Transactions still need to contained within a `using` statement and explicitly committed. You can create one by calling `dataSource.BeginTransaction`.

##Command Chains

Command chains are the primary way of working with Tortuga. Each link in the chain is used to inform the previous link about what actions are desired. Here is a basic example:

    dataSource.Procedure("uspGetEmployeeManagers", new {@BusinessEntityID = 100}).AsCollection<Manager>().Execute();

Breaking this down, we have:

* The data source
* The command being performed
* How the results of the command should be returned
* If the operation should be executed synchronously or asynchronously

###Commands

The list of available commands depends on the data source. Most data sources support 

* Raw sql
* Table/View queries
* Insert, Update, and Delete operations (some also include 'upserts')

Advanced ones may also include

* Stored procedures and/or Table Value Functions
* Batch insert, a.k.a. bulk copy

Most commands accept a parameter object. The parameter object can be a normal class, a dictionary of type `IDictionary<string, object>`, or a list of appropriate DbParameter objects.

Chain command builders honor .NET's `NotMapped` and `Column` attributes.

###Materializers

Materializers are an optional link, you only need them if you want something back from the database.

An interesting feature of the materializer is that it participates in SQL generation. For example, if you use the `AsObject<T>` or `AsCollection<T>` materializer, then it will read the list of properties on class T. That list of properties will be used to generate the SELECT clause, ensuring that you don't pull back more information than you actually need. This in turn means that indexes are used more efficiently and performance is improved.

Materializers call into several categories:

* Scalar: `AsInt`, `AsIntOrNull`, `AsString`
* Row: `AsRow`, `AsDataRow`, `AsObject`
* Table: `AsTable`, `AsDataTable`, `AsCollection`
* Multiple Tables: `AsTableSet`, `AsDataSet`

###Execution Modes

The final link in any chain is the execution mode. There are two basic options:

* `Execute()`
* `ExecuteAsync()`

Both options accept a `state` parameter. This has no direct effect, but can be used to facilitate logging. `ExecuteAsync` also accepts an optional cancellation token.


