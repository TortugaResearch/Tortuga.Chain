using System;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// Applies the current DateTimeOffset value to the indicated column.
    /// </summary>
    /// <seealso cref="ColumnRule" />
    public class ApplyDateTimeOffsetRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyUserDataRule" /> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
        /// <remarks>
        /// This will have no effect on hard deletes.
        /// </remarks>
        public ApplyDateTimeOffsetRule(string columnName, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

        }


        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            return DateTimeOffset.Now;
        }
    }


}