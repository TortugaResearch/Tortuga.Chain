using System.Text;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Aggregation;

/// <summary>
/// Class AggregationColumnCollection.
/// </summary>
public class AggregationColumnCollection : System.Collections.ObjectModel.Collection<AggregationColumn>
{
	/// <summary>
	/// If this returns true, there are columns that need to go into a GROUP BY clause.
	/// </summary>
	/// <value><c>true</c> if this instance has group by; otherwise, <c>false</c>.</value>
	public bool HasGroupBy => this.Any(c => c.GroupBy);

	/// <summary>
	/// If this returns false, there are columns that need to go into a SELECT clause.
	/// </summary>
	public bool IsEmpty => Count == 0;

	/// <summary>
	/// Builds the GROUP BY clause.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually 'GROUP BY '.</param>
	/// <param name="dataSource">The data source.</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	/// <remarks>This does not include the phrase 'SELECT'</remarks>
	public void BuildGroupByClause(StringBuilder sql, string? header, IDataSource dataSource, string? footer)
	{
		if (!HasGroupBy)
			throw new InvalidOperationException($"{nameof(HasGroupBy)} is false. There are no GROUP BY columns to process.");

		sql.Append(header + string.Join(", ", this.Where(c => c.GroupBy).Select(c => c.ToGroupBySql(dataSource.DatabaseMetadata))) + footer);
	}

	/// <summary>
	/// Builds the SELECT clause.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually 'SELECT '.</param>
	/// <param name="dataSource">The data source.</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	/// <remarks>This does not include the phrase 'SELECT'</remarks>
	public void BuildSelectClause(StringBuilder sql, string? header, IDataSource dataSource, string? footer)
	{
		if (IsEmpty)
			throw new InvalidOperationException($"{nameof(IsEmpty)} is true. There are no columns to process.");

		sql.Append(header + string.Join(", ", this.Select(c => c.ToSelectSql(dataSource.DatabaseMetadata))) + footer);
	}
}
