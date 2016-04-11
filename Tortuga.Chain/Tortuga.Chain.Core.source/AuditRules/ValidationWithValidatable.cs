using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tortuga.Anchor.ComponentModel;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// When this rule is in effect, objects that implement IValidatable will be checked.
    /// </summary>
    /// <seealso cref="Rule" />
    public class ValidationWithValidatable : ValidationRule
    {
        public ValidationWithValidatable(OperationType appliesWhen) : base(appliesWhen)
        {
            if (appliesWhen.HasFlag(OperationType.Select) || appliesWhen.HasFlag(OperationType.Delete))
                throw new ArgumentOutOfRangeException("appliesWhen", appliesWhen, "appliesWhen may only be a combination of Insert or Update");
        }

        public override void CheckValue(object argumentValue)
        {
            var validation = argumentValue as IValidatable;
            if (validation == null)
                return;

            validation.Validate();
            if (!validation.HasErrors)
                return;

            throw new ValidationException($"Validation errors: "
                + Environment.NewLine
                + string.Join(Environment.NewLine + "\t", validation.GetAllErrors().Select(e => e.ToString())));
        }

    }
}


