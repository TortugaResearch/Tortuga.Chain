using System;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class MaterializerCompiledEventArgs.
    /// </summary>
    public class MaterializerCompiledEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterializerCompiledEventArgs"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="code">The code.</param>
        /// <param name="targetType">Type of the target.</param>
        public MaterializerCompiledEventArgs(DataSource dataSource, string sql, string code, Type targetType)
        {
            TargetType = targetType;
            Code = code;
            Sql = sql;
            DataSource = dataSource;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource { get; }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; }
    }

}
