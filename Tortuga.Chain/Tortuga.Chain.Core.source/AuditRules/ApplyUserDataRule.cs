using System;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.AuditRules
{


    /// <summary>
    /// This is a rule that overrides argument values with data on the user object. 
    /// </summary>
    /// <seealso cref="ColumnRule" />
    /// <remarks>This is usually used for CreatedBy/LastModifiedBy style columns</remarks>
    public class ApplyUserDataRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyUserDataRule"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
        /// <remarks>This will have no effect on hard deletes.</remarks>
        public ApplyUserDataRule(string columnName, string propertyName, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            if (userValue == null)
                throw new InvalidOperationException($"{nameof(userValue)} is null. Did you forget to call DataSource.WithUser?");

            return MetadataCache.GetMetadata(userValue.GetType()).Properties[PropertyName].InvokeGet(userValue);
        }
    }





}