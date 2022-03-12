using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// This indicates that the data source supports raw SQL queries. 
	/// </summary>
	public interface ISupportsSqlQueries
	{
		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		IMultipleTableDbCommandBuilder Sql(string sqlStatement, object argumentValue);
	}
}
