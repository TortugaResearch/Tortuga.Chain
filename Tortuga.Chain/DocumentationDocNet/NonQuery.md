# Non-Query Materializers

There are two Non-Query materializers. `AsNonQuery` has no return value, while `AsRowsAffected` returns the number of rows affected. 

## Limitations

The rows affected count is not necessarily accurate. For example, if a stored procedure uses `SET NOCOUNT ON` then there will be nothing to report.

## SQL Generation

These materializers respond with NoColumns when participating in SQL generation.

## Internals

These materialziers use DbCommand.ExecuteNonQuery.

## Roadmap

