using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Indicates the type of operation being performed.
/// </summary>
/// <remarks>Keep numbers in sync with AuditRules.OperationTypes.</remarks>
[SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "<Pending>")]
public enum OperationType
{
	/// <summary>
	/// Undefined operation, return the default table.
	/// </summary>
	All = 0,

	/* TASK-350: Reserved for future work
        /// <summary>
        /// The table, function, or stored procedure used for insert operations.
        /// </summary>
        Insert = 1,

        /// <summary>
        /// The table, function, or stored procedure used for update operations.
        /// </summary>
        Update = 2,

        /// <summary>
        /// The table, function, or stored procedure used for delete operations.
        /// </summary>
        Delete = 4,
        */

	/// <summary>
	/// The table or view select operations.
	/// </summary>
	Select = 8,
}
