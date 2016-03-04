using System.Data;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain
{
    /// <summary>
    /// This class represents the actual preparation and execution of a command.
    /// </summary>

    public abstract class ExecutionToken
    {
        private readonly DataSource m_DataSource;
        private readonly CommandType m_CommandType;
        private readonly string m_CommandText;
        private readonly string m_OperationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionToken" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        protected ExecutionToken(DataSource dataSource, string operationName, string commandText, CommandType commandType)
        {
            m_DataSource = dataSource;
            m_OperationName = operationName;
            m_CommandText = commandText;
            m_CommandType = commandType;
        }

        /// <summary>
        /// Gets the command text, which is usually SQL.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get { return m_CommandText; }
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType
        {
            get { return m_CommandType; }
        }

        /// <summary>
        /// Gets the name of the operation being performed.
        /// </summary>
        public string OperationName
        {
            get { return m_OperationName; }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource
        {
            get { return m_DataSource; }
        }
    }
}
