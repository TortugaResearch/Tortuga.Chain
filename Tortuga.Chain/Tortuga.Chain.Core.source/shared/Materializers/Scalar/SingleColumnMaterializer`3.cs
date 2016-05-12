using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This class represents result materializers that read a Scalar value. 
    /// </summary>
    public abstract class ScalarMaterializer<TCommand, TParameter, TResult> : Materializer<TCommand, TParameter, TResult>
            where TCommand : DbCommand
        where TParameter : DbParameter
    {
        readonly string m_DesiredColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleColumnMaterializer{TCommand, TParameter, TResult}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="columnName">Name of the desired column.</param>
        protected ScalarMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, string columnName)
            : base(commandBuilder)
        {
            m_DesiredColumn = columnName;
        }


        /// <summary>
        /// Returns the list of columns the result materializer would like to have.
        /// </summary>
        public override IReadOnlyList<string> DesiredColumns()
        {
            if (m_DesiredColumn != null)
                return ImmutableArray.Create(m_DesiredColumn);
            else
                return base.DesiredColumns();
        }

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        protected void ExecuteCore(CommandImplementation<TCommand> implementation, object state)
        {
            Prepare().Execute(implementation, state);
        }

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        protected void ExecuteCore(Action<TCommand> implementation, object state)
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
        protected Task ExecuteCoreAsync(CommandImplementationAsync<TCommand> implementation, CancellationToken cancellationToken, object state)
        {
            return Prepare().ExecuteAsync(implementation, cancellationToken, state);
        }

        /// <summary>
        /// Helper method for executing the operation.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        protected Task ExecuteCoreAsync(Func<TCommand, Task> implementation, CancellationToken cancellationToken, object state)
        {
            return Prepare().ExecuteAsync(async cmd =>
            {
                await implementation(cmd);
                return null;
            }, cancellationToken, state);
        }
    }
}
