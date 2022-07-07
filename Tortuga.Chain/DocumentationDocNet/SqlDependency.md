# Change Notification Appenders

**SQL Server Only**

The `WithChangeNotification` appender is implemented on top of SQL Dependency. It is essential that you read and understand the requirements for using SQL Dependency before attempting to use this appender.

https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldependency.aspx 

## Arguments

This appender accepts an `OnChangeEventHandler` delegate, which will be fired one time if the database detects an associated change. Once it fires, the event will have to be reattached.

## Internals

## Roadmap

Automatic cache invalidation is planned for the future. https://github.com/TortugaResearch/Tortuga.Chain/issues/23
