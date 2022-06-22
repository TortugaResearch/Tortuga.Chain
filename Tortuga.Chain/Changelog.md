## Version 4.0

### Architecture

Rather than using #if in shared files, we are now using a trait based system that simulates multiple inheritance. [Tortuga.Shipwright](https://github.com/TortugaResearch/Shipwright) provides this capability using a C# Source Generator. Here is an example,

```CSharp
[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateTrait<OleDbCommand, OleDbParameter, AccessObjectName, OleDbType>))]
[UseTrait(typeof(SupportsDeleteTrait<OleDbCommand, OleDbParameter, AccessObjectName, OleDbType>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<OleDbCommand, OleDbParameter>))]
[UseTrait(typeof(SupportsUpdateByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsFromTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
[UseTrait(typeof(SupportsGetByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]

partial class AccessDataSourceBase : ICrudDataSource
```

As you can see, the common functionality is added via the `UseTrait` attribute. In cases where additional logic is needed, the trait can make calls to partial methods in the database-specific class.


### Features with Breaking Changes

[#429 Realign database classes](https://github.com/TortugaResearch/Chain/issues/429)

The `IClassXDataSource` interfaces are gone. Now we have interfaces based on specific feature sets such as `ISupportsInsert` or `ISupportsScalarFunction`. Rollups such as `ICrudDataSource` are used to combine related feature interfaces.

### Other Features 

[#327 Enabled SQL Dependency in Tortuga.Chain.SqlServer.MDS Materializers and Appenders SQL Server](https://github.com/TortugaResearch/Chain/issues/327)

Previously there was a bug preventing this feature from working with `Microsoft.Data.SqlClient`. Now that the bug is fixed, we can enable the feature in Chain.

[#411 Improve the default behavior when only one constructor exists.](https://github.com/TortugaResearch/Chain/issues/411)

If only one constructor exists, then default to using that constructor even if InferConstructor is not set.

[#408 TableAndView and GetByKey Command Builders](https://github.com/TortugaResearch/Chain/issues/408)

If you use the TableAndView attribute, then GetByKey doesn't work because it doesn't know what the primary key is.

When that happens, ask the table for its primary key. 

[#422 Add Truncate Command Builder Command Builders](https://github.com/TortugaResearch/Chain/issues/422)

This will only be exposed for databases that have a native truncate command. For other databases, use `DeleteAll` instead.

Note that in SQLite, `Truncate` and `DeleteAll` will have have the same effect. This is due to SQLite's internal optimization strategy.

[#430 Add DeleteAll command](https://github.com/TortugaResearch/Chain/issues/430)

As noted above, calling `DeleteAll` on a SQLite database will actually perform a truncate.


### Bug Fixes

[#406 Delete, TableAndView, and Strict Mode Command Builders](https://github.com/TortugaResearch/Chain/issues/406)

When using the TableAndView attribute, some column will be mapped to the view but not the table.

Those unmapped columns should be ignored when performing a Delete operation. We only care about the columns that feed into the primary key.

[#407 Records and Strict Mode Materializers and Appenders](https://github.com/TortugaResearch/Chain/issues/407)

The main problem was a bug in Tortuga.Anchor, which was treating protected properties as if they were public properties. Once fixed, we could check the `property.CanRead` field to avoid pulling in the hidden property called `EqualityContract` that all records expose.

[#418 PostgreSQL GetTableApproximateCount returns -1 for empty tables Command Builders PostgreSQL](https://github.com/TortugaResearch/Chain/issues/418)

A weird effect in PostgreSQL is that if the table has never had any rows, GetTableApproximateCount will return -1 instead of 0.


### Performance Enhancements

[#402 SQL Server Parameter Lengths Performance and Optimization SQL Builder SQL Server](https://github.com/TortugaResearch/Chain/issues/402)

When sending in variable length parameters (e.g. nVarChar), make sure the parameter length is based on the column's max length. This is to avoid creating a bunch of different plans, each with slightly different parameters.

[#415 Default to not using CommandBehavior.SequentialAccess Materializers and Appenders](https://github.com/TortugaResearch/Chain/issues/415)

Make using CommandBehavior.SequentialAccess an opt-in. According to Microsoft, "In most cases using the Default (non-sequential) access mode is the better choice, as [...] and you will get better performance using ReadAsync."

### Technical Debt

[Cleanup NuGet package code](https://github.com/TortugaResearch/Chain/pull/447/files)

Use the `<Version>` tag instead of the individual AssemblyVersion and FileVersion tags. This elimiantes the need for the `ReadPackageVersionFromOutputAssembly` target.

[#416 Tag unmapped data types Metadata PostgreSQL](https://github.com/TortugaResearch/Chain/issues/416)


Added new items to the list of PostgreSQL data types that are used internally and not exposed via the database driver.

This is going to be an ongoing issue.

[#427 Use global usings](https://github.com/TortugaResearch/Chain/issues/427)

The purpose of this change was to reduce the copious amounts of `#if` statements in the shared files.

### Other Breaking Changes

[#423 Remove String Names](https://github.com/TortugaResearch/Chain/issues/423)

Object names (e.g. tables, views, procs) will need to be referenced via:

* Using the database specific `ObjectName` class
* Using a class that is mapped to a table
* Via the `IDataSource` interfaces (which will continue to accept strings)

The reason for this change is to simplify the data source API. Currently there are too many overloads that perform the same action.

It was originally added to make working with F# easier. But with the improvements to table name inference using generics, it is no longer a pressing need.

