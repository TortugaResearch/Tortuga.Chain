This namespace is for the base classes that all data sources theoritically use. It also contains a catalog of features, expressed as interfaces, that a given data source may expose.

The `ISupports` interfaces may have reduced capabilities compared to the data source's public interface. 

Some interfaces such as `ICrudDataSource` and `IAdvancedCrudDataSource` bundle together sets of feature interfaces. They are meant to make supporting multiple databases in the same application easier.


