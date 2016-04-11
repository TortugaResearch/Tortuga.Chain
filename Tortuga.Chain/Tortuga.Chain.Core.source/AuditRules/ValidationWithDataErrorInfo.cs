using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// When this rule is in effect, objects that implement IDataErrorInfo will be checked.
    /// </summary>
    /// <seealso cref="Rule" />
    public class ValidationWithDataErrorInfo : ValidationRule
    {
        public ValidationWithDataErrorInfo(OperationType appliesWhen) : base(appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select) || appliesWhen.HasFlag(OperationType.Delete))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert or Update");
        }

        public override void CheckValue(object argumentValue)
        {
            var validation = argumentValue as IDataErrorInfo;
            if (validation != null)
                CheckValueCore(validation);
        }

        protected static void CheckValueCore(IDataErrorInfo validation)
        {
            var errors = validation.Error;

            if (string.IsNullOrEmpty(errors))
                throw new ValidationException($"Validation errors: " + errors);
        }
    }
}
