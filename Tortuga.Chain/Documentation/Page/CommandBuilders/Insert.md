# Insert Command Builder

The command performs an insert using the provided model. 

## Arguments

## SQL Generation

If the materializer desires columns, this echos back the newly inserted row.

## Limitations

This command is meant to operate on one row at a time.

## Internals

Implementation of the insert varies by database.

### SQL Server

SQL Server uses an INSERT statement with an optional OUTPUT clause.

### SQLite

SQLite uses a separate INSERT and optional SELECT statement.

## Roadmap

We should create set-based insert. The biggest question is how to handle the materializer, as we may not necessarily be able to match up the returned identity values with the original objects.

