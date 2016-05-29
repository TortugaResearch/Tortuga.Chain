# Restricted Columns

Chain’s restricted column support allows you to limit read/write access to particular columns based on the user. These restrictions can be applied globally or to specific tables. 

You setup a restricted column the same way you would any other audit rule. Here is an example that prevents users from granting themselves admin privileges:

```csharp 
ExceptWhenPredicate IsAdminCheck = user => ((UserToken)user).IsAdmin;

DataSource = DataSource.WithRules(new RestrictColumn("Users", "IsAdmin", OperationTypes.Insert |  OperationTypes.Update, IsAdminCheck));
```

Whenever an operation is performed against the `Users` table, the `IsAdminCheck` function will be executed. If the check fails, the SQL generator will skip the IsAdmin column while performing insert/update operations.

There are cases where you may need to universally block access to a column. For example, say you find that the CreditCardNumber is being exposed via multiple tables and views. While you still want your payment resolution managers to have access to it, most users shouldn’t see it. We can enable using this syntax:

```csharp 
ExceptWhenPredicate IsManger = user => ((UserToken)user).HasRoles("PaymentResolutionManager");

DataSource = DataSource.WithRules(new RestrictColumn("CreditCardNumber", OperationTypes.Select, IsManger));
```

An interesting effect of this rule is that it only blocks reads, not writes. So non-managers can update a customer’s credit card number even though they can’t actually see it themselves after pressing the save button.

## Usage Notes

Once you setup a restricted column rule, you need to ensure that all data access is performed within the context of a user. For example:

```csharp 
var ds = DataSource.WithUser(currentUser);
//perform operations with ds    
```

This can be combined with a transaction:

```csharp 
using (var ds = DataSource.WithUser(currentUser).BeginTransaction())
{
    //perform operations with ds    
    ds.Commit();
}
```

## Implementation

Restricted columns are built on top of the Audit Rules functionality and participate in the SQL generation process. This means that it is possible to bypass the restriction using stored procedures.

Audit rules and restricted columns are not intended to be mixed. For example, if you have an audit rule for setting the UpdatedBy/LastUpdatedDate columns, do not put a restricted column rule on the same columns.




