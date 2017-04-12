# Update Set Command Builder

The `UpdateSet` command builder performs an update to a set of records. 

## Arguments

You may pass a SQL expression, with optional parameter object, to this command builder. This expression will be applied to each row being updated.

Alternately you may pass in an "update object". Each row will be updated to match the values in this object. 

## Filters

Most of the time you don't want to update every row. In this case, use `.WithFilter(...)` to add a WHERE clause to the update operation.

If you actually do want to update all rows, use `.All()`.

## SQL Generation


## Internals


## Roadmap

