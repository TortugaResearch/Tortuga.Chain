using System;

namespace Tortuga.Chain.AuditRules
{


    /// <summary>
    /// Applies the indicated value to the column, overriding any previously applied value.
    /// </summary>
    /// <seealso cref="ColumnRule" />
    public class ApplyValueRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyUserDataRule" /> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value to apply.</param>
        /// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
        /// <remarks>
        /// This will have no effect on hard deletes.
        /// </remarks>
        public ApplyValueRule(string columnName, object value, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

            ValueFactory = (av, uv, ov) => value;
        }

        public ApplyValueRule(string columnName, ColumnValueGenerator valueFactory, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

            ValueFactory = valueFactory;
        }

        public ColumnValueGenerator ValueFactory { get; }

        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            return ValueFactory(argumentValue, userValue, currentValue);
        }
    }




}