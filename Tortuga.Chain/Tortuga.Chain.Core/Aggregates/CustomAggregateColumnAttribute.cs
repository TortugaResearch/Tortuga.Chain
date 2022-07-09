namespace Tortuga.Chain.Aggregates
{
	/// <summary>
	/// This attribute indicates that the propery should be mapped to a custom aggregate expression.
	/// Implements the <see cref="Attribute" />
	/// </summary>
	/// <seealso cref="Attribute" />
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CustomAggregateColumnAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
		/// </summary>
		/// <param name="selectExpression">The SQL expression to use.</param>
		/// <param name="groupBy">Indicates whether the expression is also used for grouping.</param>
		/// <param name="groupingOrder">The order to apply the group by columns. If null, uses the order the properties appear in reflection.</param>
		public CustomAggregateColumnAttribute(string selectExpression, bool groupBy = false, int? groupingOrder = null)
		{
			SelectExpression = selectExpression;
			GroupBy = groupBy;
			GroupingOrder = groupingOrder ?? int.MaxValue;
		}

		/// <summary>
		/// If true, group by this column.
		/// </summary>
		public bool GroupBy { get; }

		/// <summary>
		/// The order to apply the group by columns. If tied, uses the order the properties appear in reflection.
		/// </summary>
		/// <value>The grouping order.</value>
		/// <remarks>Defaults to Int32.MaxValue</remarks>
		public int GroupingOrder { get; }

		/// <summary>
		/// Gets the name of the source column to be provided to the aggregate function.
		/// </summary>
		public string SelectExpression { get; }
	}
}
