using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// Metadata for a database table or view.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class TableOrViewMetadata<TName, TDbType> : ITableOrViewMetadata
        where TDbType : struct
    {

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public TableOrViewMetadata(TName name, bool isTable, IList<ColumnMetadata<TDbType>> columns)
        {
            IsTable = isTable;
            Name = name;
            Columns = new ReadOnlyCollection<ColumnMetadata<TDbType>>(columns);
        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata<TDbType>> Columns { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is table or a view.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a table; otherwise, <c>false</c>.
        /// </value>
        public bool IsTable { get; }

        IReadOnlyList<IColumnMetadata> ITableOrViewMetadata.Columns
        {
            get { return Columns; }
        }

        string ITableOrViewMetadata.Name
        {
            get { return Name.ToString(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name { get; }

        /// <summary>
        /// Creates the SQL builder.
        /// </summary>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode)
        {
            return new SqlBuilder<TDbType>(Name.ToString(), Columns, strictMode);
        }


    }

}
