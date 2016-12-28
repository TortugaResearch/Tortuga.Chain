# GetByKey, UpdateByKey, and DeleteByKey Command Builders

This generates a `SELECT`, `UPDATE`, or `DELETE` statement against a table using a key or list of keys.

## Arguments

* The `…ByKey` variants accept one or more keys.
* The `…ByKeyList` variants accept a list of keys.

Keys must be scalar values (string, int guid, etc.) 

## Arguments, `UpdateByKey` Only

The `newValues` object is used to generate the `SET` clause of the `UDPATE` statement in the same manner as the normal `Update` command builder.

## Sorting 

Rows are returned in a non-deterministic order. 

## Limitations

This feature only works on tables that have a scalar primary key. 

## SQL Generation

This will generate a WHERE clause in the form of `PrimaryKey = @Param` or `PrimaryKey in (@Param1, @Param2, ...)`. 

## Roadmap

Possible enhancement: use tuples to support compound primary keys. This would require an upgrade to the metadata provider, as we would need to know the exact order for the primary key.
