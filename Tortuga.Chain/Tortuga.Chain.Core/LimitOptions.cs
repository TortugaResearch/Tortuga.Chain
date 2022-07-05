namespace Tortuga.Chain;

/// <summary>
/// Types of limits that can be applied to a table, view, or table-value function query.
/// </summary>
/// <remarks>Databases are expected to provide their own enumeration that represents a subset of these options.</remarks>
[Flags]
public enum LimitOptions
{
	/// <summary>
	/// No limits were applied.
	/// </summary>
	None = 0,

	/// <summary>
	/// Returns the indicated number of rows with optional offset
	/// </summary>
	Rows = 1,

	/// <summary>
	/// Returns the indicated percentage of rows. May be applied to TableSample
	/// </summary>
	Percentage = 2,

	/// <summary>
	/// Adds WithTies behavior to Rows or Percentage
	/// </summary>
	WithTies = 4,

	/// <summary>
	/// Returns the top N rows. When there is a tie for the Nth record, this will cause it to be returned.
	/// </summary>
	RowsWithTies = Rows | WithTies,

	/// <summary>
	/// Returns the top N percentage of rows. When there is a tie for the Nth record, this will cause it to be returned.
	/// </summary>
	PercentageWithTies = Percentage | WithTies,

	/// <summary>
	/// Randomly select a set of rows. Combine this with Rows or Percentage and optionally a Table Sample algorithm.
	/// </summary>
	RandomSample = 8,

	/// <summary>
	/// Randomly sample the indicated number of rows
	/// </summary>
	RandomSampleRows = RandomSample | Rows,

	/// <summary>
	/// Randomly sample the indicated percentage of rows
	/// </summary>
	RandomSamplePercentage = RandomSample | Percentage,

	/// <summary>
	/// Adds the Table Sample System algorithm to a random sample.
	/// </summary>
	TableSampleSystem = 16,

	/// <summary>
	/// Randomly sample N rows using the Table Sample System algorithm.
	/// </summary>
	TableSampleSystemRows = RandomSample | TableSampleSystem | Rows,

	/// <summary>
	/// Randomly sample N percentage of rows using the Table Sample System algorithm.
	/// </summary>
	TableSampleSystemPercentage = RandomSample | TableSampleSystem | Percentage,

	/// <summary>
	/// Adds the Table Sample Bernoulli algorithm behavior to a random sample.
	/// </summary>
	TableSampleBernoulli = 32,

	/// <summary>
	/// Randomly sample N rows using the Table Sample Bernoulli algorithm.
	/// </summary>
	TableSampleBernoulliRows = RandomSample | TableSampleBernoulli | Rows,

	/// <summary>
	/// Randomly sample N percentage of rows using the Table Sample Bernoulli algorithm.
	/// </summary>
	TableSampleBernoulliPercentage = RandomSample | TableSampleBernoulli | Percentage,
}
