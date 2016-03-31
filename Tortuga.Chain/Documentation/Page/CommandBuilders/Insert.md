# Insert Command Builder

The command performs an insert using the provided model. 

## Arguments

A parameter object is used to construct the INSERT statment. If a property is marked with the IgnoreOnInsert attribute, it will not participate in this step. This attribute is usually applied to defaulted columns such as CreatedDate.

Alternately, a parameter dictionary of type IReadonlyDictionary<string, object> may be used. Again, the primary keys will be read from database metadata.

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

