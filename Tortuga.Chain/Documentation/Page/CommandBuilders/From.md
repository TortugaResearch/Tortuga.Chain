# From Command Builder

This generates a SELECT statement against a table or view.

## Arguments

If just the table name is provided, all rows are returned.

If an object (model or Dictionary<string, object>) is provided, a WHERE clause is generated in the form of "Column1 = @Value1 AND Column2 = @Value2 [...]".
     
If a filter string is provided, it is used as the where clause. Optionally, an object can be provided as parameters for the query.

## Limitations

When using a filter string with filter parameter object, the SQL generator won't be able to determine which properties/keys are applicable. Instead, all of them will be sent to the database.

## SQL Generation

If no columns are desired by the materializer, 'SELECT 1' is returned instead.

## Internals

All databases work the same.

## Roadmap

