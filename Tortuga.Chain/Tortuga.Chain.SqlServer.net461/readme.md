#Tortuga Chain for SQL Server

Tortuga Chain for SQL Server supports these command builders:

* `dataSource.Sql` Execute raw SQL, optionally using a parameter object.
* `dataSource.Procedure` Execute a stored procedure, optionally using a parameter object.
* `dataSource.Function` Select from a table value function, optionally using a parameter object and/or filter.
* `dataSource.From` Reading from tables and views using simple filters
* `dataSource.Insert` Use an object to insert a single record.
* `dataSource.Update` Use an object to update a single record.
* `dataSource.InsertOrUpdate` Use an object to perform an “upsert”.
 * `dataSource.Delete` User an object to delete a single record. 

##SQL Dependency

SQL Dependency allows your application to monitor the database for changes. When a change occurs, a message is sent from the database to the application, which can then express it as an event or awaitable Task.
