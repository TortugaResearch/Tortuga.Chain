using System;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// This is the base class for rules that affect a single column.
    /// </summary>
    /// <seealso cref="Rule" />
    abstract public class ColumnRule : Rule
    {
        protected ColumnRule(string columnName, OperationType appliesWhen) : base(appliesWhen)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

            ColumnName = columnName;

        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; }

        /// <summary>
        /// Generates the value.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="userValue">The user value.</param>
        /// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
        /// <returns></returns>
        public abstract object GenerateValue(object argumentValue, object userValue, object currentValue);
    }


}