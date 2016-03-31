# Command Builders

A command builder is used to generate SQL for a specific type of operation. SQL generation does not occur immediately, but rather waits until you've selected a materializer and, optionally, appenders.

The reason for the delay is that materializers often change the way the SQL is generated. For example, when performing an Insert operation you have the option of getting back nothing, just the newly inserted primary key, or an entire object with any defaulted/calculated columns.

Since each command builder works differently, we can't really go into details here.

## Internals

### Metadata Consumption

When a command builder is created, it may immediately fetch metadata from the data source. This means that you could potentially see blocking operation and/or database error before you actually try to execute a Chain. 

In practice this shouldn't be a problem, as it should only occur the first time a given table/view is referenced. But if you are concerned, you can call `dataSource.Metadata.PreloadTables` and `PreloadViews` during startup. The time it takes to run these operations varies with the size of the database schema, so it may negatively impact performance.

### Execution

When a chain is executed, the materializer calls `Prepare` on the command builder, which in turn creates an execution token. This token contains the SQL statement and parameters needed to build and execute a `System.Data.Common.DBCommand` object. The materializer and appenders can further modify the execution token from here, but the command builder is no longer in the process.


