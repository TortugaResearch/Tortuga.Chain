using System.Collections.Concurrent;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain
{
	partial class SqlServerDataSource
	{
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain
{
	partial class OleDbSqlServerDataSource
	{
#elif SQL_SERVER_OLEDB

#elif SQLITE

namespace Tortuga.Chain
{
	partial class SQLiteDataSource
	{
#elif MYSQL

namespace Tortuga.Chain
{
	partial class MySqlDataSource
	{

#elif POSTGRESQL

namespace Tortuga.Chain
{
	partial class PostgreSqlDataSource
	{

#elif ACCESS

namespace Tortuga.Chain
{
	partial class AccessDataSource
	{

#endif
		internal ICacheAdapter m_Cache;
		internal ConcurrentDictionary<Type, object> m_ExtensionCache;

		/// <summary>
		/// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		public override ICacheAdapter Cache => m_Cache;

		/// <summary>
		/// This object can be used to access the database connection string.
		/// </summary>
		internal string ConnectionString => m_ConnectionBuilder.ConnectionString;

		/// <summary>
		/// The extension cache is used by extensions to store data source specific information.
		/// </summary>
		/// <value>
		/// The extension cache.
		/// </value>
		protected override ConcurrentDictionary<Type, object> ExtensionCache => m_ExtensionCache;


		/// <summary>
		/// Creates a new data source with the provided cache.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <returns></returns>
		public AbstractDataSource WithCache(ICacheAdapter cache)
		{
			var result = WithSettings(null);
			result.m_Cache = cache;
			return result;
		}

		/// <summary>
		/// Creates a new data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		public AbstractDataSource WithRules(params AuditRule[] additionalRules)
		{
			var result = WithSettings(null);
			result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
			return result;
		}

		/// <summary>
		/// Creates a new data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		public AbstractDataSource WithRules(IEnumerable<AuditRule> additionalRules)
		{
			var result = WithSettings(null);
			result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
			return result;
		}

		/// <summary>
		/// Creates a new data source with the indicated user.
		/// </summary>
		/// <param name="userValue">The user value.</param>
		/// <returns></returns>
		/// <remarks>
		/// This is used in conjunction with audit rules.
		/// </remarks>
		public AbstractDataSource WithUser(object? userValue)
		{
			var result = WithSettings(null);
			result.UserValue = userValue;
			return result;
		}
	}
}
