## Version 4.1

### Performance Enhancements

Use `SqlCommand.EnableOptimizedParameterBinding` in SQL Server MDS.

### Technical Debt

Added test case for command timeout.

Created `ExecutionToken.PopulateCommand` method. This eliminates a lot of copy & past text from the various `Execute`/`ExecuteAsync` methods.

## Version 4.0

### Cleanup NuGet package code

https://github.com/TortugaResearch/Chain/pull/447/files

Use the `<Version>` tag instead of the individual AssemblyVersion and FileVersion tags. This elimiantes the need for the `ReadPackageVersionFromOutputAssembly` target.


[#327 Enabled SQL Dependency in Tortuga.Chain.SqlServer.MDS Materializers and Appenders SQL Server](https://github.com/TortugaResearch/Chain/issues/327)

Perviously there was a bug preventing this feature from working with `Microsoft.Data.SqlClient`. Now that the bug is fixed we can enable the feature in Chain.

[#402 SQL Server Parameter Lengths Performance and Optimization SQL Builder SQL Server](https://github.com/TortugaResearch/Chain/issues/402)

When sending in variable length parameters (e.g. nVarChar), make sure the parameter length is based on the column's max length. This is to avoid creating a bunch of different plans, each with slightly different parameters.

[#406 Delete, TableAndView, and Strict Mode Command Builders](https://github.com/TortugaResearch/Chain/issues/406)

When using the TableAndView attribute, some column will be mapped to the view but not the table.

Those unmapped columns should be ignored when performing a Delete operation. We only care about the columns that feed into the primary key.

[#407 Records and Strict Mode Materializers and Appenders](https://github.com/TortugaResearch/Chain/issues/407)

The main problem was a bug in Tortuga.Anchor, which was treating protected properties as if they were public properties. Once fixed, we could check the `property.CanRead` field to avoid pulling in the hidden property called `EqualityContract` that all records expose.

[#408 TableAndView and GetByKey Command Builders](https://github.com/TortugaResearch/Chain/issues/408)

If you use the TableAndView attribute, then GetByKey doesn't work because it doesn't know what the primary key is.

When that happens, ask the table for it's primary key. 

[#411 Improve the default behavior when only one constructor exists.](https://github.com/TortugaResearch/Chain/issues/411)

If only one constructor exists, then default to using that constructor even if InferConstructor is not set.

[#415 Default to not using CommandBehavior.SequentialAccess Materializers and Appenders](https://github.com/TortugaResearch/Chain/issues/415)

Make using CommandBehavior.SequentialAccess an opt-in. According to Microsoft, "In most cases using the Default (non-sequential) access mode is the better choice, as [...] and you will get better performance using ReadAsync."

[#416 Tag unmapped data types Metadata PostgreSQL](https://github.com/TortugaResearch/Chain/issues/416)


Added new items to the list of PostgreSQL data types that are used internally and not exposed via the database driver.

This is going to be an ongoing issue.

[#418 PostgreSQL GetTableApproximateCount returns -1 for empty tables Command Builders PostgreSQL](https://github.com/TortugaResearch/Chain/issues/418)

A weird effect in PostgreSQL is that if the table has never had any rows, GetTableApproximateCount will return -1 instead of 0.

[#422 Add Truncate Command Builder Command Builders](https://github.com/TortugaResearch/Chain/issues/422)

This will only be exposed for databases that have a native truncate command. For other databases, use `DeleteAll` instead.

Note that in SQLite, `Truncate` and `DeleteAll` will have have the same effect. This is due to SQLite's internal optimization strategy.

[#423 Remove String Names](https://github.com/TortugaResearch/Chain/issues/423)


Object names (e.g. tables, views, procs) will need to be referenced via:

* Using the database specific ObjectName class
* Using a class that is mapped to a table
* Via the IDataSource interfaces (which will continue to accept strings)

The reason for this change is to simplify the data source API. Currently there are too many overloads that perform the same action.

It was originally added to make working with F# easier. But with the improvements to table name inference using generics, it is no longer a pressing need.

[#427 Use global usings](https://github.com/TortugaResearch/Chain/issues/427)

The purpose of this change was to reduce the copious amounts of `#if` statements in the shared files.

[#429 Realign database classes](https://github.com/TortugaResearch/Chain/issues/429)

[#430 Add DeleteAll command](https://github.com/TortugaResearch/Chain/issues/430)

[#432 Move DataSourceBase.ClassXDataSource.cs into base classes](https://github.com/TortugaResearch/Chain/issues/432)
