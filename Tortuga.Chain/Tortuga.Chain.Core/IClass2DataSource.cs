using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain
{
	/// <summary>
	/// A class 2 data source that includes stored procedures and functions.
	/// </summary>
	/// <seealso cref="IClass1DataSource" />
	/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
	public interface IClass2DataSource : IClass1DataSource
	{
		/// <summary>
		/// Executes the indicated procedure.
		/// </summary>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <returns></returns>
		IProcedureDbCommandBuilder Procedure(string procedureName);

		/// <summary>
		/// Executes the indicated procedure.
		/// </summary>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <returns></returns>
		IProcedureDbCommandBuilder Procedure(string procedureName, object argumentValue);

		/// <summary>
		/// This is used to query a scalar function.
		/// </summary>
		/// <param name="scalarFunctionName">Name of the scalar function.</param>
		/// <returns></returns>
		IScalarDbCommandBuilder ScalarFunction(string scalarFunctionName);

		/// <summary>
		/// This is used to query a scalar function.
		/// </summary>
		/// <param name="scalarFunctionName">Name of the scalar function.</param>
		/// <param name="functionArgumentValue">The function arguments.</param>
		/// <returns></returns>
		IScalarDbCommandBuilder ScalarFunction(string scalarFunctionName, object functionArgumentValue);

		/// <summary>
		/// Selects from the indicated table-value function.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <returns></returns>
		ITableDbCommandBuilder TableFunction(string functionName);

		/// <summary>
		/// Selects from the indicated table-value function.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="functionArgumentValue">The function argument value.</param>
		/// <returns></returns>
		ITableDbCommandBuilder TableFunction(string functionName, object functionArgumentValue);

		//        /// <summary>
		//        /// Perform a bulk insert.
		//        /// </summary>
		//        /// <typeparam name="T"></typeparam>
		//        /// <param name="tableName">Name of the target table.</param>
		//        /// <param name="values">The values to be inserted.</param>
		//        /// <returns></returns>
		//        ILink BulkInsert<T>(string tableName, IEnumerable<T> values);

		//        /// <summary>
		//        /// Perform a bulk insert.
		//        /// </summary>
		//        /// <param name="tableName">Name of the target table.</param>
		//        /// <param name="values">The values to be inserted.</param>
		//        /// <returns></returns>
		//        ILink InsertBulk(string tableName, DataTable values);

		//        /// <summary>
		//        /// Perform a bulk insert.
		//        /// </summary>
		//        /// <param name="tableName">Name of the target table.</param>
		//        /// <param name="values">The values to be inserted.</param>
		//        /// <returns></returns>
		//        ILink InsertBulk(string tableName, IDataReader values);
	}
}
