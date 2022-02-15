using System.ComponentModel;

namespace Tortuga.Chain.AuditRules
{
	/// <summary>
	/// When this rule is in effect, objects of type T will be checked.
	/// </summary>
	/// <seealso cref="AuditRule" />
	public class ValidateWithNotifyDataErrorInfo<T> : ValidateWithNotifyDataErrorInfo
		where T : class, INotifyDataErrorInfo
	{
		readonly Action<T> m_ValidationMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateWithNotifyDataErrorInfo{T}" /> class.
		/// </summary>
		/// <param name="appliesWhen">The rule applies when.</param>
		/// <param name="validationMethod">The method on the object that triggers validation. Usually this will be something like x =&gt; x.Validate().</param>
		public ValidateWithNotifyDataErrorInfo(OperationTypes appliesWhen, Action<T> validationMethod) : base(appliesWhen)
		{
			m_ValidationMethod = validationMethod;
		}

		/// <summary>
		/// Checks the value for validation errors.
		/// </summary>
		/// <param name="argumentValue">The argument value.</param>
		public override void CheckValue(object argumentValue)
		{
			if (!(argumentValue is T validation))
				return;

			m_ValidationMethod(validation);
			CheckValueCore(validation);
		}
	}
}
