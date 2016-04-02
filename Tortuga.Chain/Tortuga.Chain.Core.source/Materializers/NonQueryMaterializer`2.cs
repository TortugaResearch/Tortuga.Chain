using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This class indicates the associated operation should be executed without returning a result set.
    /// </summary>
    public class NonQueryMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter>, ILink
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="commandBuilder">The associated command builder.</param>
        public NonQueryMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
            : base(commandBuilder)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public void Execute(object state = null)
        {
            ExecuteCore(cmd => cmd.ExecuteNonQuery(), state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public Task ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            return ExecuteCoreAsync(async cmd => await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false), cancellationToken, state);
        }

        /// <summary>
        /// Gets the data source that is associated with this materilizer or appender.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource DataSource
        {
            get { return CommandBuilder.DataSource; }
        }


        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns>
        /// IReadOnlyList&lt;System.String&gt;.
        /// </returns>
        /// <remarks>
        /// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
        /// </remarks>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return NoColumns;
        }
    }

}
