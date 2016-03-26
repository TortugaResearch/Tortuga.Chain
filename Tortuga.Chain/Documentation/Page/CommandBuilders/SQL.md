# SQL Command Builder

This command executes arbitrary SQL. It makes no attempt to pre-validate said SQL.

## Arguments

An optional object (model or IDictionary<string, object>) may be provided when executing paramterized SQL.

## Limitations

When passing in paramters, the SQL generator won't be able to determine which properties/keys are applicable. Instead, all of them will be sent to the database.

## SQL Generation

N/A

## Internals

## Roadmap

