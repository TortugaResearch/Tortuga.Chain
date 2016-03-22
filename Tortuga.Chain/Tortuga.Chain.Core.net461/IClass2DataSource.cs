using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;

#if !WINDOWS_UWP
using System.Data;
#endif


namespace Tortuga.Chain
{
    /// <summary>
    /// A class 2 datasource includes stored procedures, table-value functions, and bulk insert.
    /// </summary>
    /// <seealso cref="IClass1DataSource" />
    public interface IClass2DataSource : IClass1DataSource
    {
        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        IMultipleTableDbCommandBuilder Procedure(string procedureName);

        /// <summary>
        /// Executes the indicated procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns></returns>
        IMultipleTableDbCommandBuilder Procedure(string procedureName, object argumentValue);

        /// <summary>
        /// Selects from the indicated table-value function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="functionArgumentValue">The function argument value.</param>
        /// <returns></returns>
        IMultipleRowDbCommandBuilder TableFunction(string functionName, object functionArgumentValue);

        /// <summary>
        /// Selects from the indicated table-value function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="functionArgumentValue">The function argument value.</param>
        /// <param name="filterValue">The filter value is used to generate a simple AND style WHERE clause.</param>
        /// <returns></returns>
        IMultipleRowDbCommandBuilder TableFunction(string functionName, object functionArgumentValue, object filterValue);

        /// <summary>
        /// Selects from the indicated table-value function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="functionArgumentValue">The function argument value.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns></returns>
        IMultipleRowDbCommandBuilder TableFunction(string functionName, object functionArgumentValue, string whereClause);


        /// <summary>
        /// Selects from the indicated table-value function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="functionArgumentValue">The function argument value.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="whereClauseArgumentValue">The argument value to apply to the where clause. Every property in the argument value must have a matching parameter in the WHERE clause and none may be same as those used in the function argument value.</param>
        /// <returns></returns>
        IMultipleRowDbCommandBuilder TableFunction(string functionName, object functionArgumentValue, string whereClause, object whereClauseArgumentValue);

        /// <summary>
        /// Performs a bulk insert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">Name of the target table.</param>
        /// <param name="values">The values to be inserted.</param>
        /// <returns></returns>
        ILink BulkInsert<T>(string tableName, IEnumerable<T> values);

#if !WINDOWS_UWP
        /// <summary>
        /// Performs a bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the target table.</param>
        /// <param name="values">The values to be inserted.</param>
        /// <returns></returns>
        ILink BulkInsert(string tableName, DataTable values);

        /// <summary>
        /// Performs a bulk insert.
        /// </summary>
        /// <param name="tableName">Name of the target table.</param>
        /// <param name="values">The values to be inserted.</param>
        /// <returns></returns>
        ILink BulkInsert(string tableName, IDataReader values);
#endif

    }
}
