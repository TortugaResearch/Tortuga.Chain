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
    public class ValidationWithNotifyDataErrorInfo : ValidationRule
    {
        public ValidationWithNotifyDataErrorInfo(OperationType appliesWhen) : base(appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select) || appliesWhen.HasFlag(OperationType.Delete))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert or Update");
        }

        public override void CheckValue(object argumentValue)
        {
            var validation = argumentValue as INotifyDataErrorInfo;
            if (validation == null)
                return;

            CheckValueCore(validation);
        }

        protected static void CheckValueCore(INotifyDataErrorInfo validation)
        {
            if (validation.HasErrors)
                throw new ValidationException($"Validation errors: "
                    + string.Join(Environment.NewLine + "    ", validation.GetErrors(null).OfType<object>().Select(e => e.ToString())));
        }


    }
}
