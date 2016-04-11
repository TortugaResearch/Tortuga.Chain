using System;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// Applies the current DateTime value to the indicated column.
    /// </summary>
    /// <seealso cref="ColumnRule" />
    public class ApplyDateTimeRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyUserDataRule" /> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="kind">The kind.</param>
        /// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
        /// <remarks>
        /// This will have no effect on hard deletes.
        /// </remarks>
        public ApplyDateTimeRule(string columnName, DateTimeKind kind, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");
            if (kind == DateTimeKind.Unspecified)
                throw new ArgumentOutOfRangeException("kind");
            Kind = kind;
        }

        public DateTimeKind Kind { get; }

        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            switch (Kind)
            {
                case DateTimeKind.Local:
                    return DateTime.Now;
                case DateTimeKind.Utc:
                    return DateTime.UtcNow;
                default:
                    throw new InvalidOperationException("kind is set incorrectly");
            }
        }
    }
}