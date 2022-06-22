namespace Tortuga.Chain.AuditRules;

/// <summary>
/// Applies the indicated value to the column, overriding any previously applied value.
/// </summary>
/// <seealso cref="ColumnRule" />
public class ValueRule : ColumnRule
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UserDataRule" /> class.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="value">The value to apply.</param>
	/// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
	/// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
	/// <remarks>
	/// This will have no effect on hard deletes.
	/// </remarks>
	public ValueRule(string columnName, object value, OperationTypes appliesWhen) : base(columnName, appliesWhen)
	{
		if (appliesWhen.HasFlag(OperationTypes.Select))
			throw new ArgumentOutOfRangeException(nameof(appliesWhen), appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

		ValueFactory = (_, __, ___) => value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ValueRule"/> class.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <param name="valueFactory">The value factory.</param>
	/// <param name="appliesWhen">The applies when.</param>
	/// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
	public ValueRule(string columnName, ColumnValueGenerator valueFactory, OperationTypes appliesWhen) : base(columnName, appliesWhen)
	{
		if (appliesWhen.HasFlag(OperationTypes.Select))
			throw new ArgumentOutOfRangeException(nameof(appliesWhen), appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

		ValueFactory = valueFactory;
	}

	/// <summary>
	/// Gets the value factory.
	/// </summary>
	/// <value>
	/// The value factory.
	/// </value>
	public ColumnValueGenerator ValueFactory { get; }

	/// <summary>
	/// Generates the value to be used for the operation.
	/// </summary>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="userValue">The user value.</param>
	/// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
	/// <returns></returns>
	public override object GenerateValue(object? argumentValue, object? userValue, object? currentValue) => ValueFactory(argumentValue, userValue, currentValue);
}
