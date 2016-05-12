using System.Data.SqlClient;
using Tortuga.Chain.SqlServer.Appenders;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerAppenders.
    /// </summary>
    public static class SqlServerAppenders
    {
        /// <summary>
        /// Attaches a SQL Server dependency change listener to this operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result type.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="eventHandler">The event handler to fire when the underlying data changes.</param>
        /// <returns>Tortuga.Chain.Core.ILink&lt;TResult&gt;.</returns>
        /// <remarks>This will only work for operations against non-transactional SQL Server data sources that also comform to the rules about using SQL Dependency.</remarks>
        public static ILink<TResult> WithChangeNotification<TResult>(this ILink<TResult> previousLink, OnChangeEventHandler eventHandler)
        {
            return new NotifyChangeAppender<TResult>(previousLink, eventHandler);
        }
    }
}
