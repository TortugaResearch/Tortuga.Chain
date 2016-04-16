# GetById Command Builder

This generates a SELECT statement against a table or view based on a scalar primary key.

## Arguments

If just the table name is provided, all rows are returned.

The parameter must be either a scalar value (string, int guid, etc.) or an IEnumerable of the same. 

## Limitations

This command builder does not work with compound primary keys.

## SQL Generation

This will generate a WHERE clause in the form of `PrimaryKey = @Param` or `PrimaryKey in (@Param1, @Param2, ...)`. 

## Internals

All databases work the same.

## Roadmap

Possible enhancement: use tuples to support compound primary keys. This would require an upgrade to the metadata provider, as we would need to know the exact order for the primary key.