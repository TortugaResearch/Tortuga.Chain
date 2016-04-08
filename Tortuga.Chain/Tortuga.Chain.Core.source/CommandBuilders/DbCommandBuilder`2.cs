using System.ComponentModel;
using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using System;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class from which all other command builders are created.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command used.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class DbCommandBuilder<TCommand, TParameter> : DbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        private readonly DataSource<TCommand, TParameter> m_DataSource;

        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected DbCommandBuilder(DataSource<TCommand, TParameter> dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

            m_DataSource = dataSource;
            StrictMode = dataSource.StrictMode;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataSource<TCommand, TParameter> DataSource
        {
            get { return m_DataSource; }
        }


        /// <summary>
        /// Indicates this operation has no result set.
        /// </summary>
        /// <returns></returns>
        public override sealed ILink AsNonQuery() { return new NonQueryMaterializer<TCommand, TParameter>(this); }


        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public abstract ExecutionToken<TCommand, TParameter> Prepare(Materializer<TCommand, TParameter> materializer);
    }
}

