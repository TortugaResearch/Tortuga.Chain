using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tortuga.Chain.Metadata
{



    /// <summary>
    /// This class represents user defined types.
    /// </summary>
    /// <typeparam name="TName">The type of the t name.</typeparam>
    /// <typeparam name="TDbType">The type of the t database type.</typeparam>
    public sealed class UserDefinedTypeMetadata<TName, TDbType> : IUserDefinedTypeMetadata
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
        public bool IsTableType { get; }

        IReadOnlyList<IColumnMetadata> IUserDefinedTypeMetadata.Columns
        {
            get { return Columns; }
        }

        string IUserDefinedTypeMetadata.Name
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
    }

}
