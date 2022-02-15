using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.AuditRules
{
	/// <summary>
	/// This is a rule that overrides argument values with data on the user object.
	/// </summary>
	/// <seealso cref="ColumnRule" />
	/// <remarks>This is usually used for CreatedBy/LastModifiedBy style columns</remarks>
	public class UserDataRule : ColumnRule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserDataRule"/> class.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="appliesWhen">The rule can be applied to insert, update, and/or soft delete operations.</param>
		/// <exception cref="ArgumentOutOfRangeException">appliesWhen;appliesWhen may only be a combination of Insert, Update, or Delete</exception>
		/// <remarks>This will have no effect on hard deletes.</remarks>
		public UserDataRule(string columnName, string propertyName, OperationTypes appliesWhen) : base(columnName, appliesWhen)
		{
			if (appliesWhen.HasFlag(OperationTypes.Select))
				throw new ArgumentOutOfRangeException(nameof(appliesWhen), appliesWhen, "appliesWhen may only be a combination of Insert, Update, or Delete");

			PropertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		/// <value>
		/// The name of the property.
		/// </value>
		public string PropertyName { get; }

		/// <summary>
		/// Generates the value to be used for the operation.
		/// </summary>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="userValue">The user value.</param>
		/// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "userValue")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DataSource")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WithUser")]
		public override object? GenerateValue(object? argumentValue, object? userValue, object? currentValue)
		{
			if (userValue == null)
				throw new InvalidOperationException($"{nameof(userValue)} is null. Did you forget to call DataSource.WithUser?");

			return MetadataCache.GetMetadata(userValue.GetType()).Properties[PropertyName].InvokeGet(userValue);
		}
	}
}
