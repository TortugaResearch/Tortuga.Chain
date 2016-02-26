using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// This class represents result materializers that read from a single column. 
    /// </summary>
    public abstract class SingleColumnMaterializer<TCommandType, TParameterType, TResultType> : Materializer<TCommandType, TParameterType, TResultType>
            where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        readonly string m_DesiredColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleColumnMaterializer{TCommandType, TParameterType, TResultType}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The command builder.</param>
        /// <param name="columnName">Name of the desired column.</param>
        protected SingleColumnMaterializer(DbCommandBuilder<TCommandType, TParameterType> commandBuilder, string columnName)
            : base(commandBuilder)
        {
            m_DesiredColumns = columnName;
        }


        /// <summary>
        /// Returns the list of columns the result materializer would like to have.
        /// </summary>
        public override IReadOnlyList<string> DesiredColumns()
        {
            if (m_DesiredColumns != null)
                return System.Collections.Immutable.ImmutableList.Create(m_DesiredColumns);
            else
                return base.DesiredColumns();
        }

    }
}
