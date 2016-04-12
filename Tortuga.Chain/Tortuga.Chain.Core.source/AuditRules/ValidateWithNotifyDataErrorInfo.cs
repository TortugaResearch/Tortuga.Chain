using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// When this rule is in effect, objects that implement INotifyDataErrorInfo will be checked.
    /// </summary>
    /// <seealso cref="Rule" />
    public class ValidateWithNotifyDataErrorInfo : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateWithNotifyDataErrorInfo"/> class.
        /// </summary>
        /// <param name="appliesWhen">The rule applies when.</param>
        /// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert or Update</exception>
        public ValidateWithNotifyDataErrorInfo(OperationTypes appliesWhen) : base(appliesWhen)
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
            var validation = argumentValue as INotifyDataErrorInfo;
            if (validation == null)
                return;

            CheckValueCore(validation);
        }

        /// <summary>
        /// Checks the value core.
        /// </summary>
        /// <param name="validation">The validation.</param>
        /// <exception cref="ValidationException"></exception>
        protected static void CheckValueCore(INotifyDataErrorInfo validation)
        {
            if (validation == null)
                throw new ArgumentNullException(nameof(validation), $"{nameof(validation)} is null.");

            if (validation.HasErrors)
                throw new ValidationException($"Validation errors: "
                    + string.Join(Environment.NewLine + "    ", validation.GetErrors(null).OfType<object>().Select(e => e.ToString())));
        }


    }
}
