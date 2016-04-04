namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// This is used to read out column names during SQL generation.
    /// </summary>
    public struct ColumnNamePair
    {
        public ColumnNamePair(string quotedSqlName, string sqlVariableName)
        {
            QuotedSqlName = quotedSqlName;
            SqlVariableName = sqlVariableName;
        }

        /// <summary>
        /// Gets or sets column name as quoted SQL.
        /// </summary>
        public string QuotedSqlName { get; set; }

        /// <summary>
        /// Gets or sets column name as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; set; }
    }
}
