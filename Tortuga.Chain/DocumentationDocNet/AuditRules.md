# Audit Rules

Audit rules allow you to drastically reduce the amount of boilerplate code needed for common scenarios such as verifying a record is valid before saving it, setting the created by or updated by column, and honoring soft deletes. 

Note that all of these rules are applied at the data source level. You cannot selectively apply them to individual tables, though you can use multiple data sources with different rules. 

## Validation

Chain supports the following validation interfaces:

* `IDataErrorInfo`
* `INotifyDataErrorInfo`
* `IValidatable` (From Tortuga Anchor)

Validation will only occur if the object used for the insert/update operation implements the indicated interface. 

Example:

    dataSource = dataSource.WithRules(new ValidateWithDataErrorInfo (OperationType.InsertOrUpdate));

### Triggering Validation

Depending on your object model, you may need to manually trigger validation before IDataErrorInfo or INotifyDataErrorInfo report an error condition. 

    dataSource = dataSource.WithRules(new ValidateWithDataErrorInfo<ModelBase>(OperationType.InsertOrUpdate, x => x.Validate()));

### Custom Validation

If your object model has its own validation interface, you can subclass ` ValidationRule`.

## Created By/Updated By Support

Chain can automatically set fields such as “CreatedByKey” and “UpdatedByKey”. There are two steps necessary to do this. First, you need to create a data source with the appropriate rules. As with a normal data source, this should be cached at the application level.

    dataSource = dataSource.WithRules(
        new UserDataRule("CreatedByKey", "UserKey", OperationTypes.Insert),
        new UserDataRule("UpdatedByKey", "UserKey", OperationTypes.InsertOrUpdate));

When a request is initiated, you then create a contextual data source with the user object. There are no restrictions on what this object looks like, so long as it has the columns indicated by your audit rules.

    myDS = dataSource.WithUser(currentUser);

When using the contextual data source, the indicated rules will be automatically applied to insert, and update operations. For example:

    myDS.Update("Customer", customer).Execute();

This will automatically apply the user’s `UserKey` value to the `UpdatedByKey` column when performing the update operation. This replaces any value on `customer.UpdatedByKey` and will work even if `customer.UpdatedByKey` is marked as `[IgnoreOnUpdate]`.
    
## Created Date/Updated Date Support

To ensure that `CreatedDate` and `UpdatedDate` are correctly set without using constraints and triggers, you can use `DateTimeRule` or `DateTimeOffsetRule`. 

    dataSource = dataSource.WithRules(
        new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
        new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate));

These rules do not require a context data source.

## Soft Delete Support

Soft delete support is enabled by add a` SoftDeleteRule`. This rule can apply to delete and/or select operations. Usually the `SoftDeleteRule` will be combined for rules that set a deleted by and/or deleted date column. For example: 

    dataSource = dataSource.WithRules(
        new SoftDeleteRule("DeletedFlag", 1, OperationType.SelectOrDelete),
        new UserDataRule("DeletedByKey", "UserKey", OperationType.Delete),
        new DateTimeOffsetRule("DeletedDate", OperationType.Delete));

When soft deletes are in effect, calling `dataSource.Delete` will automatically switch from a hard to a soft delete if the correct column exists.

Likewise, the `dataSource.From` operation will automatically rewrite the WHERE clause to filter out deleted records.

## Transactions

A transactional data source inherits the rules and user value from the data source used to create it. If necessary, you can chain these operations together. For example:

    using (var trans = dataSource.WithUser(currentUser).BeginTransaction())
 
## Dependency Injection
If your DI framework supports it, you should create the contextual data source using the `dataSource.WithUser` function at the beginning of the request. You can then inject that data source instead of the application-wide version to ensure that audit rules are enforced.
 
