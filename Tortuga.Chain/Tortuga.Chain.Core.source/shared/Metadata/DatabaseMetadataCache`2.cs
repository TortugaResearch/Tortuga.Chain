using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// An abstract database metadata cache
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public abstract class DatabaseMetadataCache<TName, TDbType> : IDatabaseMetadataCache
        where TDbType : struct
    {
        /// <summary>
        /// This dictionary is used to register customer database types. It is used by the ToClrType method and possibly parameter generation.
        /// </summary>
        /// <remarks>This is populated by the RegisterType method.</remarks>
        ConcurrentDictionary<string, TypeRegistration<TDbType>> m_RegisteredTypes = new ConcurrentDictionary<string, TypeRegistration<TDbType>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the metadata for a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public virtual ScalarFunctionMetadata<TName, TDbType> GetScalarFunction(TName scalarFunctionName)
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        /// <summary>
        /// Gets the scalar functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<ScalarFunctionMetadata<TName, TDbType>> GetScalarFunctions()
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns></returns>
        public virtual StoredProcedureMetadata<TName, TDbType> GetStoredProcedure(TName procedureName)
        {
            throw new NotSupportedException("Stored procedures are not supported by this data source");
        }

        StoredProcedureMetadata IDatabaseMetadataCache.GetStoredProcedure(string procedureName)
        {
            return GetStoredProcedure(ParseObjectName(procedureName));
        }

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<StoredProcedureMetadata<TName, TDbType>> GetStoredProcedures()
        {
            throw new NotSupportedException("Stored procedures are not supported by this data source");
        }

        IReadOnlyCollection<StoredProcedureMetadata> IDatabaseMetadataCache.GetStoredProcedures()
        {
            return GetStoredProcedures();
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        public virtual TableFunctionMetadata<TName, TDbType> GetTableFunction(TName tableFunctionName)
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        TableFunctionMetadata IDatabaseMetadataCache.GetTableFunction(string tableFunctionName)
        {
            return GetTableFunction(ParseObjectName(tableFunctionName));
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<TableFunctionMetadata<TName, TDbType>> GetTableFunctions()
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        IReadOnlyCollection<TableFunctionMetadata> IDatabaseMetadataCache.GetTableFunctions() => GetTableFunctions();

        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract TableOrViewMetadata<TName, TDbType> GetTableOrView(TName tableName);

        TableOrViewMetadata IDatabaseMetadataCache.GetTableOrView(string tableName) => GetTableOrView(ParseObjectName(tableName));

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public abstract TableOrViewMetadata<TName, TDbType> GetTableOrViewFromClass<TObject>() where TObject : class;

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        TableOrViewMetadata IDatabaseMetadataCache.GetTableOrViewFromClass<TObject>() => GetTableOrViewFromClass<TObject>();

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IReadOnlyCollection<TableOrViewMetadata<TName, TDbType>> GetTablesAndViews();

        IReadOnlyCollection<TableOrViewMetadata> IDatabaseMetadataCache.GetTablesAndViews() => GetTablesAndViews();

        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual UserDefinedTypeMetadata<TName, TDbType> GetUserDefinedType(TName typeName)
        {
            throw new NotSupportedException("User defined types are not supported by this data source");
        }

        UserDefinedTypeMetadata IDatabaseMetadataCache.GetUserDefinedType(string typeName) => GetUserDefinedType(ParseObjectName(typeName));

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual IReadOnlyCollection<UserDefinedTypeMetadata<TName, TDbType>> GetUserDefinedTypes()
        {
            throw new NotSupportedException("Table value functions are not supported by this data source");
        }

        IReadOnlyCollection<UserDefinedTypeMetadata> IDatabaseMetadataCache.GetUserDefinedTypes() => GetUserDefinedTypes();

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public abstract void Preload();

        /// <summary>
        /// Registers a database type and its CLR equivalent.
        /// </summary>
        /// <param name="databaseTypeName">Name of the database type.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="clrType">Type of the color.</param>
        public void RegisterType(string databaseTypeName, TDbType databaseType, Type clrType)
        {
            m_RegisteredTypes[databaseTypeName] = new TypeRegistration<TDbType>(databaseTypeName, databaseType, clrType);
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns the CLR type that matches the indicated database column type.
        /// </summary>
        /// <param name="databaseTypeName">Name of the database column type.</param>
        /// <param name="isNullable">If nullable, Nullable versions of primitive types are returned.</param>
        /// <param name="maxLength">Optional length. Used to distinguish between a char and string.</param>
        /// <param name="isUnsigned">Indicates whether or not the column is unsigned. Only applicable to some databases.</param>
        /// <returns>
        /// A CLR type or NULL if the type is unknown.
        /// </returns>
        /// <remarks>Use RegisterType to add a missing mapping or override an existing one.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public Type ToClrType(string databaseTypeName, bool isNullable, int? maxLength, bool? isUnsigned = null)
        {
            if (TryGetRegisteredType(databaseTypeName, out var registeredType))
            {
                return registeredType.ClrType;
            }

            var dbType = TypeNameToDbType(databaseTypeName, isUnsigned);

            if (!dbType.HasValue)
                return ToClrType(dbType.Value, isNullable, maxLength);

            return null;
        }

        /// <summary>
        /// Removes a database type registration and its CLR equivalent. If a builtin type, this restores it to its default behavior.
        /// </summary>
        /// <param name="databaseTypeName">Name of the database type.</param>
        /// <remarks>True if the type was successfully unregistered. False if the type was not found.</remarks>
        public bool UnregisterType(string databaseTypeName)
        {
            return m_RegisteredTypes.TryRemove(databaseTypeName, out var _);
        }

        /// <summary>
        /// Converts a value to a string suitable for use in a SQL statement.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Optional database column type.</param>
        /// <returns></returns>
        /// <remarks>Override this to support custom escaping logic.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dbType")]
        public virtual string ValueToSqlValue(object value, TDbType? dbType)
        {
            switch (value)
            {
                case DBNull _:
                case System.Reflection.Missing _:
                case null:
                    return "NULL";

                case byte _:
                case sbyte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case decimal _:
                case float _:
                case double _:
                    return value.ToString();

                case string s:
                    return "'" + s.Replace("'", "''") + "'";

                case DateTime d:
                    return "'" + d.ToString("O") + "'"; //ISO 8601

                case DateTimeOffset d:
                    return "'" + d.ToString("O") + "'"; //ISO 8601

                case TimeSpan ts:
                    return "'" + ts.ToString("hh:mm:ss.fffffff") + "'"; //ISO 8601

                default:
                    if (dbType.HasValue)
                        throw new NotSupportedException($"Converting a value of type {value.GetType().Name} into a string of type {dbType.ToString()} is not supported. Try filing a bug report.");
                    else
                        throw new NotSupportedException($"Converting a value of type {value.GetType().Name} is not supported. Try supplying a dbType or filing a bug report.");
            }
        }

        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name"></param>
        protected abstract TName ParseObjectName(string name);

        /// <summary>
        /// Returns the CLR type that matches the indicated database column type.
        /// </summary>
        /// <param name="dbType">Type of the database column.</param>
        /// <param name="isNullable">If nullable, Nullable versions of primitive types are returned.</param>
        /// <param name="maxLength">Optional length. Used to distinguish between a char and string. Defaults to string.</param>
        /// <returns>
        /// A CLR type or NULL if the type is unknown.
        /// </returns>
        /// <remarks>This does not take into consideration registered types.</remarks>
        protected abstract Type ToClrType(TDbType dbType, bool isNullable, int? maxLength);

        /// <summary>
        /// Tries the registered column type from a database column type name.
        /// </summary>
        /// <param name="databaseTypeName">Name of the database type.</param>
        /// <param name="registeredType">Type of the registered.</param>
        /// <returns></returns>
        protected bool TryGetRegisteredType(string databaseTypeName, out TypeRegistration<TDbType> registeredType)
        {
            return m_RegisteredTypes.TryGetValue(databaseTypeName, out registeredType);
        }

        /// <summary>
        /// Determines the database column type from the column type name.
        /// </summary>
        /// <param name="typeName">Name of the database column type.</param>
        /// <param name="isUnsigned">Indicates whether or not the column is unsigned. Only applicable to some databases.</param>
        /// <returns></returns>
        /// <remarks>This does not honor registered types. This is only used for the database's hard-coded list of native types.</remarks>
        protected abstract TDbType? TypeNameToDbType(string typeName, bool? isUnsigned);
    }
}
