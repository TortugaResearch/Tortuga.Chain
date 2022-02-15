namespace Tortuga.Chain
{
	/// <summary>
	/// Controls what happens when performing a model-based insert
	/// </summary>
	[Flags]
	public enum InsertOptions
	{
		/// <summary>
		/// Use the default behavior.
		/// </summary>
		None = 0,

		/// <summary>
		/// Override the identity/auto-number column.
		/// </summary>
		/// <remarks>This may require elevated privileges.</remarks>
		IdentityInsert = 4,

		/*
         * Might need this for PostgreSQL
        /// <summary>
        /// Do not reset the identity/auto-number column after performing an identity insert.
        /// </summary>
        /// <remarks>Use this when performing a series of identity inserts to improve performance. Then invoke ResetIdentity on the DataSource. This is a no-op when resetting the identity column is not necessary (Access, SQL Server, SQLite).</remarks>
        DoNotResetIdentityColumn = 8
        */
	}
}