# Upsert Command Builder

The command performs a insert or update using the provided model. 

## Arguments

The parameter object must contain the primary key(s) necessary to locate the record. The keys are normally read from the database, but you can override this behavior to use properties on the object that use the Key attribute.

Upsert will honor the IgnoreOnInsert and IgnoreOnUpdate attributes.

If you use the ChangedPropertiesOnly option, only properties marked as changed by `IPropertyChangeTracking.ChangedProperties()` will be used for the insert part of the operation. This interface is found in the `Tortuga.Anchor.ComponentModel` namespace.

Alternately, a parameter dictionary of type IReadonlyDictionary<string, object> may be used. Again, the primary keys will be read from database metadata.

UpsertOptions.IdentityInsert allows for overriding an identity or auto-number column.

## SQL Generation

If the materializer desires columns, this echos back the row. The UpsertOptions flag determines if the original or new values are returned.

## Limitations

This command is meant to operate on one row at a time. Set-based operations need to be performed using a SQL or Procedure command.

## Internals

Implementation of the upset varies by database.

### SQL Server

Upserts are performed via a MERGE command. This command is inheritently non-atomic and may throw an exception if there is a race condition. If this occurs, the application needs to decide whether or not to retry the operation.

### SQLite

This requires a 3 step process:

* Update the existing row (if it exists)
* Insert a new row, silently failing if it already exists
* Select the recently inserted/updated row. (If the correct materializer was selected.)

## Roadmap

