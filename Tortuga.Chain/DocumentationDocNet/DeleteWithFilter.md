# Delete with Filter Command Builder

The `DeleteWithFilter` command performs set-based deletes using a where expression or filter object. 

## Arguments

This accepts a SQL expression, with optional parameter object, or a filter object.

## SQL Generation

If soft delete is enabled for this table, an update operation will be generated to set the deleted flag. 

## Internals

Implementation of the delete varies by database.

### SQL Server

SQL Server uses an UPDATE statement with an optional OUTPUT clause.

### SQLite

SQLite uses a separate DELETE and optional SELECT statement.

## Roadmap

