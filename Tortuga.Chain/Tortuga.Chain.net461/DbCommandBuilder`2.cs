using System.Data.Common;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain
{
    /// <summary>
    /// This is the base class from which all other command builders are created.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class DbCommandBuilder<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        private readonly DataSource<TCommandType, TParameterType> m_DataSource;

        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected DbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public abstract ExecutionToken<TCommandType, TParameterType> Prepare(Formatter<TCommandType, TParameterType> formatter);

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource<TCommandType, TParameterType> DataSource
        {
            get { return m_DataSource; }
        }

    }
}