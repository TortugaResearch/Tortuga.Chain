# DataRow, DataTable, and DataSet Materializers

These materializers support the legacy DataRow, DataTable, and DataSet types. They are slower than the Row, Table, and TableSet equivalents, but have the advantage of being bindable to data grids in Windows Forms and WebForms.

## Options

* `.ToDataRow` supports the `RowOptions` enumeration.
* `.ToDataSet` expects a list of table names.

## SQL Generation

These materializers do not request any columns. See the associated command builder for the behavior.
