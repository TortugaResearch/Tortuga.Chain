namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Limit options supported by Postgre SQL.
    /// </summary>
    /// <remarks>This is a strict subset of LimitOptions</remarks>
    public enum PostgreSqlLimitOption
    {
        /// <summary>
        /// No limits were applied.
        /// </summary>
        None = LimitOptions.None,

        /// <summary>
        /// Returns the indicated number of rows with optional offset
        /// </summary>
        Rows = LimitOptions.Rows,

        /// <summary>
        /// Randomly sample N percentage of rows using the Table Sample System algorithm.
        /// </summary>
        TableSampleSystemPercentage = LimitOptions.TableSampleSystemPercentage,

        /// <summary>
        /// Randomly sample N percentage of rows using the Table Sample Bernoulli algorithm.
        /// </summary>
        TableSampleSystemBernoulliPercentage = LimitOptions.TableSampleSystemPercentage,
    }
}
