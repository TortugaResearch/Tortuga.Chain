using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Formatters
{
    /// <summary>
    /// This is the base class for formatters that don't return a value. Most operation are not executed without first attaching a formatter subclass.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class Formatter<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        readonly DbCommandBuilder<TCommandType, TParameterType> m_CommandBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter{TCommandType, TParameterType}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated command builder.</param>
        /// <exception cref="ArgumentNullException">operation;operation is null.</exception>
        protected Formatter(DbCommandBuilder<TCommandType, TParameterType> commandBuilder)
        {
            if (commandBuilder == null)
                throw new ArgumentNullException(nameof(commandBuilder), $"{nameof(commandBuilder)} is null.");

            m_CommandBuilder = commandBuilder;
        }

        /// <summary>
        /// Gets the associated operation.
        /// </summary>
        /// <value>The command builder.</value>
        protected DbCommandBuilder<TCommandType, TParameterType> CommandBuilder
        {
            get { return m_CommandBuilder; }
        }

        /// <summary>
        /// Returns the command text (usually SQL) that would have been executed. 
        /// </summary>
        /// <returns>System.String.</returns>
        public string CommandText() => CommandBuilder.Prepare(this).CommandText;

        /// <summary>
        /// Returns the list of columns the formatter would like to have.
        /// </summary>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        public virtual IReadOnlyList<string> DesiredColumns()
        {
            return ImmutableList<string>.Empty;
        }

        /// <summary>
        /// Prepares this operation for execution.
        /// </summary>
        /// <returns>ExecutionToken&lt;TCommandType, TParameterType&gt;.</returns>
        protected ExecutionToken<TCommandType, TParameterType> Prepare() => CommandBuilder.Prepare(this);

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        protected void ExecuteCore(Func<TCommandType, int?> implementation, object state) => Prepare().Execute(implementation, state);

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        protected void ExecuteCore(Action<TCommandType> implementation, object state)
        {
            Prepare().Execute(cmd =>
            {
                implementation(cmd);
                return null;
            }, state);
        }


        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        protected Task ExecuteCoreAsync(Func<TCommandType, Task<int?>> implementation, CancellationToken cancellationToken, object state) => Prepare().ExecuteAsync(implementation, cancellationToken, state);

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        protected Task ExecuteCoreAsync(Func<TCommandType, Task> implementation, CancellationToken cancellationToken, object state)
        {
            return Prepare().ExecuteAsync(async cmd =>
            {
                await implementation(cmd);
                return null;
            }, cancellationToken, state);
        }
    }

}
