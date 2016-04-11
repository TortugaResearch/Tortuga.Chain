using System;

namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// If a column matching this rule is found, then soft deletes will be applied instead of hard deletes.
    /// </summary>
    /// <seealso cref="Rule" />
    public sealed class SoftDeleteRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftDeleteRule"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="deletedValue">The value that represents a deleted row.</param>
        /// <param name="appliesWhen">The rule can be applied to delete and/or select operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be Select or Delete</exception>
        public SoftDeleteRule(string columnName, object deletedValue, OperationType appliesWhen) : base(columnName, appliesWhen)
        {
            switch (appliesWhen)
            {
                case OperationType.Select:
                case OperationType.Delete:
                case OperationType.SelectOrDelete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be Select or Delete");
            }
            DeletedValue = deletedValue;
        }

        public object DeletedValue { get; }

        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            return DeletedValue;
        }
    }




}