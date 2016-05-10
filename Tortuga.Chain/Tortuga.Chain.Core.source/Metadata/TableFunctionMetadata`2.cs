using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a database table value function.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class TableFunctionMetadata<TName, TDbType> : ITableFunctionMetadata
        where TDbType : struct
    {

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        public TableFunctionMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters, IList<ColumnMetadata<TDbType>> columns)
        {
            Name = name;
            Columns = new ColumnMetadataCollection<TDbType>(name.ToString(), columns);
            Parameters = new ReadOnlyCollection<ParameterMetadata<TDbType>>(parameters);
        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ColumnMetadataCollection<TDbType> Columns { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public ReadOnlyCollection<ParameterMetadata<TDbType>> Parameters { get; }

        /// <summary>
        /// Creates a SQL builder.
        /// </summary>
        /// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode)
        {
            return new SqlBuilder<TDbType>(Name.ToString(), Columns, Parameters, strictMode);
        }
    }
}
