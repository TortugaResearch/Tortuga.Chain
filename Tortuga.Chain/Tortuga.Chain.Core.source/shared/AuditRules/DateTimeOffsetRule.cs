using System;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// Applies the current DateTimeOffset value to the indicated column.
    /// </summary>
    /// <seealso cref="ColumnRule" />
    public class DateTimeOffsetRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRule" /> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
        /// <remarks>
        /// This will have no effect on hard deletes.
        /// </remarks>
        public DateTimeOffsetRule(string columnName, OperationTypes appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationTypes.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");
        }


        /// <summary>
        /// Generates the value to be used for the operation.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="userValue">The user value.</param>
        /// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
        /// <returns></returns>
        public override object GenerateValue(object argumentValue, object userValue, object currentValue) => DateTimeOffset.Now;
    }


}