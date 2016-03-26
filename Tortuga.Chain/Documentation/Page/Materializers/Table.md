# Row, Table, and TableSet Materializers

These are modern equivalents to DataRow, DataTable, and DataSet.

## Options

* `.ToRow` supports the `RowOptions` enumeration.
* `.ToTableSet` expects a list of table names.

## SQL Generation

These materializers do not request any columns. See the associated command builder for the behavior.

## Internals

A Row is implemented as an immutable dictionary. Likewise, Table and TableSet are immutable.

## Roadmap

These objects, while faster than the legacy versions, can probably be improved even more if we switch to an array based internal implementation. We’re looking for improvements both in performance and, more importantly, memory consumption.

