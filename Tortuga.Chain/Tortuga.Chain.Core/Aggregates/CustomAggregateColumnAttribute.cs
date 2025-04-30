namespace Tortuga.Chain.Aggregates
{
	/// <summary>
	/// This attribute indicates that the propery should be mapped to a custom aggregate expression.
	/// Implements the <see cref="Attribute" />
	/// </summary>
	/// <seealso cref="Attribute" />
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	sealed public class CustomAggregateColumnAttribute : BaseAggregateAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
		/// </summary>
		/// <param name="selectExpression">The SQL expression to use.</param>
		/// <param name="groupBy">Indicates whether the expression is also used for grouping.</param>
		/// <param name="order">The order is important for group by columns. If tied or null, uses the order the properties appear in reflection.</param>
		public CustomAggregateColumnAttribute(string selectExpression, bool groupBy, int order)
		{
			SelectExpression = selectExpression;
			GroupBy = groupBy;
			Order = order;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupByColumnAttribute"/> class.
		/// </summary>
		/// <param name="selectExpression">The SQL expression to use.</param>
		/// <param name="groupBy">Indicates whether the expression is also used for grouping.</param>
		public CustomAggregateColumnAttribute(string selectExpression, bool groupBy = false)
		{
			SelectExpression = selectExpression;
			GroupBy = groupBy;
		}

		/// <summary>
		/// Gets the name of the source column to be provided to the aggregate function.
		/// </summary>
		public string SelectExpression { get; }
	}
}
