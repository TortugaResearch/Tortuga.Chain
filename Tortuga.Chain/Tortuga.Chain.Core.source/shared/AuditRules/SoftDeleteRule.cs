using System;

namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// If a column matching this rule is found, then soft deletes will be applied instead of hard deletes.
    /// </summary>
    /// <seealso cref="AuditRule" />
    public sealed class SoftDeleteRule : ColumnRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftDeleteRule"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="deletedValue">The value that represents a deleted row.</param>
        /// <param name="appliesWhen">The rule can be applied to delete and/or select operations.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be Select or Delete</exception>
        public SoftDeleteRule(string columnName, object deletedValue, OperationTypes appliesWhen) : base(columnName, appliesWhen)
        {
            switch (appliesWhen)
            {
                case OperationTypes.Select:
                case OperationTypes.Delete:
                case OperationTypes.SelectOrDelete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be Select or Delete");
            }
            DeletedValue = deletedValue;
        }

        /// <summary>
        /// Gets the deleted value.
        /// </summary>
        /// <value>
        /// The deleted value.
        /// </value>
        public object DeletedValue { get; }

        /// <summary>
        /// Generates the value to be used for the operation.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="userValue">The user value.</param>
        /// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
        /// <returns></returns>
        public override object GenerateValue(object argumentValue, object userValue, object currentValue)
        {
            return DeletedValue;
        }
    }




}