# Delete Command Builder

The command performs a delete using the provided model. 

## Arguments

The parameter object must contain the primary key(s) necessary to locate the record. The keys are normally read from the database, but you can override this behavior to use properties on the object that use the Key attribute.

Alternately, a parameter dictionary of type IReadonlyDictionary<string, object> may be used. Again, the primary keys will be read from database metadata.

## SQL Generation

If the materializer desires columns, this will return the row as it looked before it was deleted.

## Limitations

This command is meant to operate on one row at a time. Set-based operations need to be performed using a SQL or Procedure command.

## Internals

Implementation of the delete varies by database.

### SQL Server

SQL Server uses an UPDATE statement with an optional OUTPUT clause.

### SQLite

SQLite uses a separate DELETE and optional SELECT statement.

## Roadmap

