using System;
using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Metadata for a database table or view.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public class TableOrViewMetadata<TName, TDbType> : TableOrViewMetadata
        where TDbType : struct
    {
        readonly SqlBuilder<TDbType> m_Builder;
        readonly DatabaseMetadataCache<TName, TDbType> m_MetadataCache;
        IndexMetadataCollection<TName> m_Indexes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOrViewMetadata{TName, TDbType}"/> class.
        /// </summary>
        /// <param name="metadataCache">The metadata cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public TableOrViewMetadata(DatabaseMetadataCache<TName, TDbType> metadataCache, TName name, bool isTable, IList<ColumnMetadata<TDbType>> columns)
        {
            m_MetadataCache = metadataCache ?? throw new ArgumentNullException(nameof(metadataCache), $"{nameof(metadataCache)} is null.");
            IsTable = isTable;
            Name = name;
            base.Name = name.ToString();
            Columns = new ColumnMetadataCollection<TDbType>(name.ToString(), columns);
            base.Columns = Columns.GenericCollection;
            NonNullableColumns = new ColumnMetadataCollection(columns.Where(c => c.IsNullable == false));
            m_Builder = new SqlBuilder<TDbType>(Name.ToString(), Columns);
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
        /// Creates the SQL builder
        /// </summary>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode) => m_Builder.Clone(strictMode);

        /// <summary>
        /// Gets the indexes for this table or view.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Indexes are not supported by this data source</exception>
        public IndexMetadataCollection<TName> GetIndexes()
        {
            if (m_Indexes == null)
                m_Indexes = m_MetadataCache.GetIndexesForTable(Name);
            return m_Indexes;
        }
    }
}
