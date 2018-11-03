#if !IDataErrorInfo_Missing
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// When this rule is in effect, objects that implement IDataErrorInfo will be checked.
    /// </summary>
    /// <seealso cref="AuditRule" />
    public class ValidateWithDataErrorInfo : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateWithDataErrorInfo"/> class.
        /// </summary>
        /// <param name="appliesWhen">The rule applies when.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert or Update</exception>
        public ValidateWithDataErrorInfo(OperationTypes appliesWhen) : base(appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationTypes.Select) || appliesWhen.HasFlag(OperationTypes.Delete))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert or Update");
        }

        /// <summary>
        /// Checks the value for validation errors.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        public override void CheckValue(object argumentValue)
        {
            if (argumentValue is IDataErrorInfo validation)
                CheckValueCore(validation);
        }

        /// <summary>
        /// Checks the value core.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <exception cref="ValidationException"></exception>
        protected static void CheckValueCore(IDataErrorInfo validation)
        {
            if (validation == null)
                throw new ArgumentNullException(nameof(validation), $"{nameof(validation)} is null.");

            var errors = validation.Error;

            if (string.IsNullOrEmpty(errors))
                throw new ValidationException($"Validation errors: " + errors);
        }
    }
}
#endif