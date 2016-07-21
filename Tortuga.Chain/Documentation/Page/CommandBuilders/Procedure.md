# Procedure Command Builder

This command is used to execute a stored procedure that may return one or more record sets.

## Arguments

If an object parameter is provided (model or IDictionary<string, object>), the procedure's paratmeters will be compared to those on the object. Where they match, the value will be passed in.

## SQL Generation

## SQL Server

This command will use standard `CommandType.StoredProcedure` semantics. 

## PostgreSQL

This command is only used for functions that return one or more `refcursor` objects.

This command will use standard `CommandType.StoredProcedure` semantics to get a list of cursors. Then for each cursor, `FETCH ALL IN "cursor_name"` will be executed.

## Internals

### PostgreSQL

When building the command, `PostgreSqlCommandExecutionToken.DereferenceCursors` is set to true. This causes the data sources' execute functions to use cursor dereferencing semantics.

Cursors cannot be dereferenced outside of a transaction. Chain will automatically create a transacton if one doesn't exist.

The `CommandTimeout` is misleading, as it applied twice: once to the function call and once to the call that dereferences the cursors. This effectively doubles the amount of time that a command can theoretically take to execute.

## Roadmap

