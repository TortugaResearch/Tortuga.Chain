using System;

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


    }

}
