using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.ComponentModel;

namespace Tortuga.Chain.AuditRules;

/// <summary>
/// When this rule is in effect, objects that implement IValidatable will be checked.
/// </summary>
/// <seealso cref="AuditRule" />
[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validatable")]
public class ValidateWithValidatable : ValidationRule
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValidateWithValidatable"/> class.
	/// </summary>
	/// <param name="appliesWhen">The rule applies when.</param>
	/// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert or Update</exception>
	public ValidateWithValidatable(OperationTypes appliesWhen) : base(appliesWhen)
	{
		if (appliesWhen.HasFlag(OperationTypes.Select) || appliesWhen.HasFlag(OperationTypes.Delete))
			throw new ArgumentOutOfRangeException(nameof(appliesWhen), appliesWhen, "appliesWhen may only be a combination of Insert or Update");
	}

	/// <summary>
	/// Checks the value for validation errors.
	/// </summary>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ValidationException"></exception>
	public override void CheckValue(object argumentValue)
	{
		if (!(argumentValue is IValidatable validation))
			return;

		validation.Validate();
		if (!validation.HasErrors)
			return;

		throw new ValidationException($"Validation errors: "
			+ Environment.NewLine
			+ string.Join(Environment.NewLine + "\t", validation.GetAllErrors().Select(e => e.ToString())));
	}
}
