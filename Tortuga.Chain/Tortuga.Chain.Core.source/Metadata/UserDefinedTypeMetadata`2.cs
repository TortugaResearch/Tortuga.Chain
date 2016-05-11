using System.Collections.Generic;

namespace Tortuga.Chain.Metadata
{



    /// <summary>
    /// This class represents user defined types.
    /// </summary>
    /// <typeparam name="TName">The type of the t name.</typeparam>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    public sealed class UserDefinedTypeMetadata<TName, TDbType> : UserDefinedTypeMetadata
        where TDbType : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefinedTypeMetadata{TName, TDbType}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTableType">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public UserDefinedTypeMetadata(TName name, bool isTableType, IList<ColumnMetadata<TDbType>> columns)
        {
            IsTableType = isTableType;
            Name = name;
            base.Name = name.ToString();
            Columns = new ColumnMetadataCollection<TDbType>(name.ToString(), columns);
            base.Columns = Columns.GenericCollection;
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
    }

}
