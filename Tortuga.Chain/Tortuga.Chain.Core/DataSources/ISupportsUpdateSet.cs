using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// Used to mark data sources that support the UpdateSet command.
	/// </summary>
	public interface ISupportsUpdateSet
	{

		/// <summary>
		/// Update multiple records using an update expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="updateExpression">The update expression.</param>
		/// <param name="options">The update options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		IUpdateSetDbCommandBuilder UpdateSet(string tableName, string updateExpression, UpdateOptions options = UpdateOptions.None);

		/// <summary>
		/// Update multiple records using an update expression.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="updateExpression">The update expression.</param>
		/// <param name="updateArgumentValue">The argument for the update expression.</param>
		/// <param name="options">The update options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		IUpdateSetDbCommandBuilder UpdateSet(string tableName, string updateExpression, object updateArgumentValue, UpdateOptions options = UpdateOptions.None);

		/// <summary>
		/// Update multiple records using an update value.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="options">The options.</param>
		/// <remarks>Use .WithFilter to apply a WHERE clause.</remarks>
		IUpdateSetDbCommandBuilder UpdateSet(string tableName, object newValues, UpdateOptions options = UpdateOptions.None);
	}
}
