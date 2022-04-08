namespace Tortuga.Chain.AuditRules;

/// <summary>
/// Base class for all audit rules.
/// </summary>
abstract public class AuditRule
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AuditRule"/> class.
	/// </summary>
	/// <param name="appliesWhen">The rule applies when.</param>
	protected AuditRule(OperationTypes appliesWhen)
	{
		AppliesWhen = appliesWhen;
	}

	/// <summary>
	/// Indicates when the rule is applicable.
	/// </summary>
	/// <value>
	/// The rule applies when.
	/// </value>
	public OperationTypes AppliesWhen { get; }
}
