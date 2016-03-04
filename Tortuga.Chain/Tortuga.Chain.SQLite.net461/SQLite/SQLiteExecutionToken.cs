using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.SQLite
{
    public class SQLiteExecutionToken : ExecutionToken<SQLiteCommand, SQLiteParameter>
    {
        private readonly LockType m_LockType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionToken{TCommandType, TParameterType}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation. This is used for logging.</param>
        /// <param name="commandText">The SQL to be executed.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        public SQLiteExecutionToken(DataSource<SQLiteCommand, SQLiteParameter> dataSource, string operationName, string commandText, IReadOnlyList<SQLiteParameter> parameters, CommandType commandType = CommandType.Text, LockType lockType = LockType.Write)
            : base(dataSource, operationName, commandText, parameters, commandType)
        {
            m_LockType = lockType;

        }
        /// <summary>
        /// Indicats whether or not the operation may perform writes.
        /// </summary>
        /// <value>The mode.</value>
        public LockType LockType
        {
            get { return m_LockType; }
        }

    }
}
