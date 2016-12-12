# Table Function Command Builder

This generates a SELECT statement against a table-valued function.

## Function Arguments

Function arguments are provided as part of the `dataSource.TableFunction(...)` method. All parameters must be accounted for.

## Sorting 

To add sorting, use the `.WithSorting(...)` method. This accepts a list of strings or SortExpressions (the latter is only needed for descending sorts). To prevent SQL injection attacks, column names are validated against the database. 

## Limits

To add limits, use the `.WithLimits(...)` method. The type of limits avaialble vary from database to database. Most provide Top, Offset/Fetch, and one or more forms of random sampling.

Sorting is often required when using Offset/Fetch. It is not allowed when using random sampling.

## Filters

Filters can be added (or replaced) using the `.WithFilter(...)` method. This uses the same filters (WHERE clause or filter object) that are available when calling `.From(...)`.

If just the table name is provided, all rows are returned.

If an object (model or Dictionary<string, object>) is provided, a WHERE clause is generated in the form of "Column1 = @Value1 AND Column2 = @Value2 [...]".
     
If a filter string is provided, it is used as the where clause. Optionally, an object can be provided as parameters for the query.

Do not use the same arguments for the filter expression as you use for the table parameters, as this may caused unintended behavior.

## Limitations

When using a filter string with filter parameter object, the SQL generator won't be able to determine which properties/keys are applicable. Instead, all of them will be sent to the database.

When using a filter string with filter parameter object, and one of the filtered columns is an ANSI string (`char`/`varchar`), you may experience performance degradation. To avoid this, pass in an array of `DbParameter` objects with the correct ` DbParameter.DbType` value. (Normally DbType is inferred by checking against the column’s type.) 

Some types of random sampling are not available with table-valued functions.

## SQL Generation

If no columns are desired by the materializer, 'SELECT 1' is returned instead.

## Internals

All databases work the same except for what types of limits they support.

## Roadmap

