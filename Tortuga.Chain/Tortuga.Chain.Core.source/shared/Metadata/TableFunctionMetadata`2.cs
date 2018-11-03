using System.Collections.Generic;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
        

    /// <summary>
    /// Metadata for a database table value function.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class TableFunctionMetadata<TName, TDbType> : TableFunctionMetadata
        where TDbType : struct
    {
        readonly SqlBuilder<TDbType> m_Builder;
         
        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="columns">The columns.</param>
        public TableFunctionMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters, IList<ColumnMetadata<TDbType>> columns)
        {
            Name = name;
            base.Name = name.ToString();
            Columns = new ColumnMetadataCollection<TDbType>(name.ToString(), columns);
            base.Columns = Columns.GenericCollection;
            Parameters = new ParameterMetadataCollection<TDbType>(name.ToString(), parameters);
            base.Parameters = Parameters.GenericCollection;

            m_Builder = new SqlBuilder<TDbType>(Name.ToString(), Columns, Parameters);
        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public new ColumnMetadataCollection<TDbType> Columns { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public new TName Name { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public new ParameterMetadataCollection<TDbType> Parameters { get; }

        /// <summary>
        /// Creates a SQL builder.
        /// </summary>
        /// <param name="strictMode">if set to <c>true</c> [strict mode].</param>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode) => m_Builder.Clone(strictMode);
    }
}
