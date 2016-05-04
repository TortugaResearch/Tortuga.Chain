using Tortuga.Chain.CommandBuilders;

#if !WINDOWS_UWP
#endif


namespace Tortuga.Chain
{
    /// <summary>
    /// A class 2 datasource includes stored procedures, table-value functions, and bulk insert.
    /// </summary>
    /// <seealso cref="IClass1DataSource" />
    /// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.</remarks>
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
        //        /// Performs a bulk insert.
        //        /// </summary>
        //        /// <typeparam name="T"></typeparam>
        //        /// <param name="tableName">Name of the target table.</param>
        //        /// <param name="values">The values to be inserted.</param>
        //        /// <returns></returns>
        //        ILink BulkInsert<T>(string tableName, IEnumerable<T> values);

        //#if !WINDOWS_UWP
        //        /// <summary>
        //        /// Performs a bulk insert.
        //        /// </summary>
        //        /// <param name="tableName">Name of the target table.</param>
        //        /// <param name="values">The values to be inserted.</param>
        //        /// <returns></returns>
        //        ILink InsertBulk(string tableName, DataTable values);

        //        /// <summary>
        //        /// Performs a bulk insert.
        //        /// </summary>
        //        /// <param name="tableName">Name of the target table.</param>
        //        /// <param name="values">The values to be inserted.</param>
        //        /// <returns></returns>
        //        ILink InsertBulk(string tableName, IDataReader values);
        //#endif

    }
}
