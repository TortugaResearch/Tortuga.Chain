using System;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    public class TypeRegistration<TDbType>
    {
        /// <summary>
        /// Initializes a new instance of the TypeRegistration class.
        /// </summary>
        /// <param name="databaseTypeName">Name of the database type.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="clrType">Type of the color.</param>
        public TypeRegistration(string databaseTypeName, TDbType databaseType, Type clrType)
        {
            DatabaseType = databaseType;
            DatabaseTypeName = databaseTypeName;
            ClrType = clrType;
        }

        /// <summary>
        /// Gets the type of the CLR type.
        /// </summary>
        /// <value>
        /// The type of the CLR type.
        /// </value>
        public Type ClrType { get; }

        /// <summary>
        /// Gets the type of the database column.
        /// </summary>
        /// <value>
        /// The type of the database column.
        /// </value>
        public TDbType DatabaseType { get; }

        /// <summary>
        /// Gets the name of the database column type.
        /// </summary>
        /// <value>
        /// The name of the database column type.
        /// </value>
        public string DatabaseTypeName { get; }
    }
}
