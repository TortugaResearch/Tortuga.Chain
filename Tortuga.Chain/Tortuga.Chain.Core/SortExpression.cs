namespace Tortuga.Chain;

/// <summary>
/// Sort expressions are used for From and FromFunction command builders.
/// </summary>
/// <remarks>You can implicitly convert strings into sort expressions.</remarks>
public struct SortExpression : IEquatable<SortExpression>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SortExpression"/> class.
	/// </summary>
	/// <param name="columnName">Name of the column to be sorted by.</param>
	/// <param name="direction">if set to <c>true</c> [descending].</param>
	/// <exception cref="ArgumentException"></exception>
	/// <remarks>If direction == Expression, columnName is a raw expression instead of a column name.</remarks>
	public SortExpression(string columnName, SortDirection direction)
	{
		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		ColumnName = columnName;
		Direction = direction;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SortExpression"/> class.
	/// </summary>
	/// <param name="columnName">Name of the column to be sorted by.</param>
	/// <exception cref="ArgumentException"></exception>
	public SortExpression(string columnName)
	{
		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentException($"{nameof(columnName)} is null or empty.", nameof(columnName));

		ColumnName = columnName;
		Direction = SortDirection.Ascending;
	}

	/// <summary>
	/// Gets the name of the column to be sorted by. 
	/// </summary>
	/// <value>
	/// The name of the column.
	/// </value>
	/// <remarks>If Direction == Expression, this is a raw expression instead of a column name.</remarks>
	public string? ColumnName { get; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="SortExpression"/> is descending.
	/// </summary>
	/// <value>
	///   <c>true</c> if descending; otherwise, <c>false</c>.
	/// </value>
	public SortDirection Direction { get; }

	/// <summary>
	/// Perform an implicit conversion from <see cref="string"/> to <see cref="SortExpression"/> with Ascending as the sort direction.
	/// </summary>
	/// <param name="columnName">The columnName</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	[SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
	public static implicit operator SortExpression(string columnName) => new SortExpression(columnName);

	/// <summary>
	/// Implements the != operator.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator !=(SortExpression left, SortExpression right)
	{
		return !(left == right);
	}

	/// <summary>
	/// Implements the == operator.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator ==(SortExpression left, SortExpression right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
	public bool Equals(SortExpression other)
	{
		return string.Equals(ColumnName, other.ColumnName, StringComparison.OrdinalIgnoreCase) && Direction == other.Direction;
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
	/// </summary>
	/// <param name="obj">The object to compare with the current instance.</param>
	/// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
	public override bool Equals(object? obj)
	{
		var temp = obj as SortExpression?;
		if (temp == null)
			return false;
		return Equals(temp.Value);
	}

	/// <summary>
	/// Returns a hash code for this instance.
	/// </summary>
	/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
	public override int GetHashCode()
	{
		return (ColumnName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0) + (int)Direction;
	}
}
