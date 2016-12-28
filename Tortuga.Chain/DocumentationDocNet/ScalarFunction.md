# Scalar Function Command Builder

This generates a SELECT statement against a scalar function.

## Function Arguments

Function arguments are provided as part of the `dataSource.ScalarFunction(...)` method. All parameters must be accounted for.

## Internals

Some database don't support scalar functions, but instead use table-valued functions with a single row/single column result.

## Roadmap

