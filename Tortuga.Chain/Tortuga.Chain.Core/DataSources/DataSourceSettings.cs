namespace Tortuga.Chain.DataSources
{
	/// <summary>
	/// This class is used to modify settings that are not represented by the connection string.
	/// </summary>
	public class DataSourceSettings
	{
		/// <summary>
		/// Gets or sets the default command timeout.
		/// </summary>
		/// <value>The default command timeout.</value>
		/// <remarks>Leave null to inherit settings from the parent data source.</remarks>
		public TimeSpan? DefaultCommandTimeout { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether strict mode is enabled.
		/// </summary>
		/// <remarks>Strict mode requires all properties that don't represent columns to be marked with the NotMapped attribute.
		/// Leave null to inherit settings from the parent data source.</remarks>
		public bool? StrictMode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to suppress global events.
		/// </summary>
		/// <value>If <c>true</c>, this data source will not honor global event handlers.</value>
		/// <remarks>Leave null to inherit settings from the parent data source.</remarks>
		public bool? SuppressGlobalEvents { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether to use CommandBehavior.SequentialAccess.
		/// </summary>
		/// <value>If <c>true</c>, this data source will not honor global event handlers.</value>
		/// <remarks>Leave null to inherit settings from the parent data source.
		/// Disable for general database access. Enable when working with very large objects. 
		/// For more information see https://docs.microsoft.com/en-us/archive/blogs/adonet/using-sqldatareaders-new-async-methods-in-net-4-5
		/// </remarks>
		public bool? SequentialAccessMode { get; set; }
	}
}
