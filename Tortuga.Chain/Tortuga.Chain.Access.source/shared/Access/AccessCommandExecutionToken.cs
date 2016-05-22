using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;


namespace Tortuga.Chain.Access
{
    /// <summary>
    /// Class AccessExecutionToken.
    /// </summary>
    public sealed class AccessCommandExecutionToken : CommandExecutionToken<OleDbCommand, OleDbParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionToken{TCommand, TParameter}" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="operationName">Name of the operation. This is used for logging.</param>
        /// <param name="commandText">The SQL to be executed.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        public AccessCommandExecutionToken(ICommandDataSource<OleDbCommand, OleDbParameter> dataSource, string operationName, string commandText, IReadOnlyList<OleDbParameter> parameters, CommandType commandType = CommandType.Text)
            : base(dataSource, operationName, commandText, parameters, commandType)
        {

        }

        /// <summary>
        /// This function is executed with the value returned by this execution token. 
        /// It is used to create the next execution token in the chain.
        /// </summary>
        internal Action<object> ForwardResult { get; set; }


        internal AccessCommandExecutionMode ExecutionMode { get; set; } = AccessCommandExecutionMode.Materializer;

        /// <summary>
        /// Gets or sets the command to be executed after the current execution token.
        /// </summary>
        /// <value>
        /// The chained command.
        /// </value>
        internal AccessCommandExecutionToken NextCommand { get; set; }

    }




}
