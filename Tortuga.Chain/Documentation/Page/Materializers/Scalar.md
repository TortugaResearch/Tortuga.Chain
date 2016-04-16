# Scalar Materializers

Scalar materializers are avaialble for common .NET types such as string, numerics, date/time, etc. Nullable versions are avaialable.

## Options

The scalar materializers accepts an optional column name. This may be used by the command builder when generating a SELECT clause.

## SQL Generation

As mentioned above, this materailizer may request a specific column. If no column in indicated, then AutoSelectDesiredColumns is returned to the command builder.

## Internals

All scalar materializes use DbCommand.ExecuteScalar. This means that only the first column of the first row is evaluated. Everything else is silently ignored.

## Roadmap

