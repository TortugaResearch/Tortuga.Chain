# List Materializers

List materializers are avaialble for common .NET types such as string, numerics, date/time, etc. 

## Options

The list materializers accepts an optional column name. This may be used by the command builder when generating a SELECT clause.

* DiscardNulls: This flag will discard null values when populating the list. 
* IgnoreExtraColumns: Normally an error is raised if multiple columns are returned. This flag will ignore them instead.
* FlattenExtraColumns: If multiple columns are returned, this flag will cause all columns to be included in the resulting list. 

FlattenExtraColumns operates left to right, then top to bottom. For example:

    Resultset: 
        [1, 2, 3]
        [4, 5, 6]
    List:
        1, 2, 3, 4, 5, 6

## SQL Generation

As mentioned above, this materailizer may request a specific column. If no column in indicated, then AutoSelectDesiredColumns is returned to the command builder.

## Internals


## Roadmap

You can pass a  columnName  parameter to list based materializers. This parameter can be used for SQL generation, but is ignored when parsing the result set. As a result, we can't do interesting things like only read one column from a stored procedure that returns 3 columns. Task #24.
