using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This is the root base class for materializers. It is used when we need to strip away the generic type arguments.
    /// </summary>
    public abstract class Materializer
    {

        /// <summary>
        /// Return all columns.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IReadOnlyList<string> AllColumns = new ReadOnlyCollection<string>(new List<string>());

        /// <summary>
        /// The automatically select desired columns. If there are primary key column(s), return them. Otherwise look for an identity column. If that is missing too, raise an exception. 
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IReadOnlyList<string> AutoSelectDesiredColumns = new ReadOnlyCollection<string>(new List<string>());

        /// <summary>
        /// Return no columns. Used when the result of the operation is not desired (e.g. insert/update).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IReadOnlyList<string> NoColumns = new ReadOnlyCollection<string>(new List<string>());

        /// <summary>
        /// Occurs when an execution token has been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override command behavior.</remarks>
        public event EventHandler<ExecutionTokenPreparedEventArgs> ExecutionTokenPrepared;


        /// <summary>
        /// Occurs when execution token is about to be been prepared.
        /// </summary>
        /// <remarks>This is mostly used by appenders to override SQL generation.</remarks>
        public event EventHandler<ExecutionTokenPreparingEventArgs> ExecutionTokenPreparing;

        /// <summary>
        /// Returns the list of columns the materializer would like to have. 
        /// </summary>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        /// <remarks>If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.</remarks>
        public virtual IReadOnlyList<string> DesiredColumns()
        {
            return AutoSelectDesiredColumns;
        }
        /// <summary>
        /// Raises the <see cref="E:ExecutionTokenPrepared" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionTokenPreparedEventArgs"/> instance containing the event data.</param>
        protected void OnExecutionTokenPrepared(ExecutionTokenPreparedEventArgs e)
        {
            ExecutionTokenPrepared?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExecutionTokenPreparing" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExecutionTokenPreparingEventArgs"/> instance containing the event data.</param>
        protected void OnExecutionTokenPreparing(ExecutionTokenPreparingEventArgs e)
        {
            ExecutionTokenPreparing?.Invoke(this, e);
        }

    }

}
