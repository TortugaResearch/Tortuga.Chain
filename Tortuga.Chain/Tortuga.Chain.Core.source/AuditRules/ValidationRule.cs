using System.ComponentModel.DataAnnotations;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// Base class for validation style rules.
    /// </summary>
    /// <seealso cref="Rule" />
    public abstract class ValidationRule : Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="appliesWhen">The rule applies when.</param>
        protected ValidationRule(OperationTypes appliesWhen) : base(appliesWhen)
        {

        }

        /// <summary>
        /// Checks the value for validation errors. 
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <exception cref="ValidationException">A ValidationException will be thrown if the object contains validation errors.</exception>
        public abstract void CheckValue(object argumentValue);

    }
}
