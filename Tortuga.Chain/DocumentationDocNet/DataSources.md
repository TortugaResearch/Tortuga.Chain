# Data Sources

At the beginning of every chain is a `DataSource`. A normal data source represents an abstract connection to the database. The key word here is 'abstract'; this is not an actual connection but rather all of the information needed to create and manage one.

Normally an application will only have one instance of a data source per connection string. This is important, as the data source needs to cache information about your database and application. 

## Basic Configuration

Most of the time, all of the configuration needed to create a data source can be found with a normal ADO.NET style connection string. The connection string can be passed to the constructor as a `String` or the appropriate subclass of `DbConnectionStringBuilder`. Optionally you can also provide a name for the data source. The name has no effect, but may be useful for logging.

On the .NET Framework, data sources also provide a factory method called `CreateFromConfig` for reading the connection string from the app.config file. (This is not available on UWP.)

### Advanced Configuration

For some databases, you may override connection settings beyond those offered by the connection string. For example, for SQL Server you can alter `dataSource.Settings.ArithAbort` and `dataSource.Settings.XactAbort`. These overrides are applied each time a connection is opened by the data source.

The `Settings` object is database specific and will not necessarily be available for all types of data sources.

## Data Source Classifications

Chain is primarily designed to take advantage of the capabilities of a database. Rather than catering to the "lowest common denominator" like many ORMs, we'd rather give you full access to everything your database is capable of supporting.

However, we realize that some projects actually do need database portability. For those project we offer a set of data source classifications, which are expressed in terms of interfaces.

### ICrudDataSource

A CRUD data source provides the necessary functionality to support a repository pattern using CRUD style operations. Briefly these include:

* Database metadata (tables, views, columns, etc.)
* From
* Insert: Single row inserts
* Update: Single row updates
* Upsert: Automatic insert or update as appropriate
* Delete: Single row deletes
* Sql: Raw SQL statements with optional parameters.

Separate pages will be offered to document each of these operations.

### IAdvancedCrudDataSource

These data sources add:

* Truncate
* Upsert



## Internals

### Command Execution

The database contains an Execute and ExecuteCore method for processing execution tokens. Within these methods the connection is opened and the DBCommand is created. Materializers provide a call-back through which the command is executed and the results are processed.

Commands are executed by the materializer callback because neither the execution token nor the data source know whether you need `ExecuteNonQuery`, `ExecuteScalar`, or `ExecuteReader`. This also gives the materializer the option to set execution mode flags such as `Sequential`.

### Metadata

The data source contains metadata about database such as a list of tables and views. While available to the application, this information is primary used by the command builders to generate SQL.

In order to achieve reasonable performance, the data source caches the metadata. This means that the data source can become out of sync with the database if the schema changes while the application is running. Currently there is no way to "reset" the cache, so if that happens you'll need to create a new instance of the data source.

### Extension Data

Some Chain extensions such as compiled materializers need to cache data source specific information. For these, there is the `DataSource.GetExtensionData` method. Each extension should only store one object here. Any additional information should hang off that object rather than creating multiple keys.

### Data Caching

Data caching is performed through the data source object. Originally the intention was to use the `System.Runtime.Caching` framework, which would have allowed developers to plug in any caching framework they want. However, it turns out that framework has some design flaws and we're going to need to create our own caching interface and matching adapters.

In the meantime, we'll be using .NET's MemoryCache but are not exposing it. That way there won't be a breaking change once we settle on a caching interface.

### Universal Windows Platform Limitations

UWP does not support the `System.Runtime.Caching` framework, which in turn means that Chain for UWP doesn't support it either. Again, this will change once we develop our own caching interface and start writing adapters for the more popular caching libraries and servers.
