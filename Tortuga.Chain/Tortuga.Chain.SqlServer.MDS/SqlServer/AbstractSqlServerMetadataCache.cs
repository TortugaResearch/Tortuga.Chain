using System.Collections.Concurrent;
using System.Data.Common;
using Tortuga.Anchor;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer;

#if SQL_SERVER_MDS

/// <summary>Class AbstractSqlServerMetadataCache.</summary>
public abstract class AbstractSqlServerMetadataCache : DatabaseMetadataCache<SqlServerObjectName, AbstractDbType>
#elif SQL_SERVER_OLEDB

/// <summary>Class AbstractSqlServerMetadataCache.</summary>
public abstract class AbstractSqlServerMetadataCache : OleDbDatabaseMetadataCache<SqlServerObjectName>
#endif

{
	/// <summary>
	/// Gets the metadata for a table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <returns></returns>
	public override sealed TableOrViewMetadata<SqlServerObjectName, AbstractDbType> GetTableOrView(SqlServerObjectName tableName)
	{
		return OnGetTableOrView(tableName);
	}

	//C# doesn't allow us to change the return type so we're using this as a thunk.
	internal abstract TableOrViewMetadata<SqlServerObjectName, AbstractDbType> OnGetTableOrView(SqlServerObjectName tableName);
}

#if SQL_SERVER_MDS

partial class SqlServerMetadataCache : AbstractSqlServerMetadataCache
#elif SQL_SERVER_OLEDB

partial class OleDbSqlServerMetadataCache : AbstractSqlServerMetadataCache
#endif
{
	internal readonly DbConnectionStringBuilder m_ConnectionBuilder;

	internal readonly ConcurrentDictionary<SqlServerObjectName, ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType>> m_ScalarFunctions = new();
	internal readonly ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, AbstractDbType>> m_StoredProcedures = new();

	internal readonly ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, AbstractDbType>> m_TableFunctions = new();

	internal readonly ConcurrentDictionary<SqlServerObjectName, SqlServerTableOrViewMetadata<AbstractDbType>> m_Tables = new();

	internal readonly ConcurrentDictionary<Type, TableOrViewMetadata<SqlServerObjectName, AbstractDbType>> m_TypeTableMap = new();

	internal readonly ConcurrentDictionary<SqlServerObjectName, UserDefinedTableTypeMetadata<SqlServerObjectName, AbstractDbType>> m_UserDefinedTableTypes = new();

	readonly ConcurrentDictionary<int, SqlServerObjectName> m_ObjectIdTableMap = new();

	internal string? m_DatabaseName;
	internal string? m_DefaultSchema;

	/// <summary>
	/// Gets the metadata for a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <returns>Null if the object could not be found.</returns>
	public override ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType> GetScalarFunction(SqlServerObjectName scalarFunctionName)
	{
		return m_ScalarFunctions.GetOrAdd(scalarFunctionName, GetScalarFunctionInternal);
	}

	/// <summary>
	/// Gets the scalar functions that were loaded by this cache.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
	/// </remarks>
	public override IReadOnlyCollection<ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType>> GetScalarFunctions()
	{
		return m_ScalarFunctions.GetValues();
	}

	/// <summary>
	/// Gets the stored procedure's metadata.
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <returns>Null if the object could not be found.</returns>
	public override StoredProcedureMetadata<SqlServerObjectName, AbstractDbType> GetStoredProcedure(SqlServerObjectName procedureName)
	{
		return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
	}

	/// <summary>
	/// Gets the stored procedures that were loaded by this cache.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
	/// </remarks>
	public override IReadOnlyCollection<StoredProcedureMetadata<SqlServerObjectName, AbstractDbType>> GetStoredProcedures()
	{
		return m_StoredProcedures.GetValues();
	}

	/// <summary>
	/// Gets the metadata for a table function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <returns>Null if the object could not be found.</returns>
	public override TableFunctionMetadata<SqlServerObjectName, AbstractDbType> GetTableFunction(SqlServerObjectName tableFunctionName)
	{
		return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
	}

	/// <summary>
	/// Gets the table-valued functions that were loaded by this cache.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
	/// </remarks>
	public override IReadOnlyCollection<TableFunctionMetadata<SqlServerObjectName, AbstractDbType>> GetTableFunctions()
	{
		return m_TableFunctions.GetValues();
	}

	/// <summary>
	/// Gets the tables and views that were loaded by this cache.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
	/// </remarks>
	public override IReadOnlyCollection<TableOrViewMetadata<SqlServerObjectName, AbstractDbType>> GetTablesAndViews()
	{
		return m_Tables.GetValues();
	}

	/// <summary>
	/// Gets the metadata for a user defined type.
	/// </summary>
	/// <param name="typeName">Name of the type.</param>
	/// <returns>UserDefinedTableTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;.</returns>
	public override UserDefinedTableTypeMetadata<SqlServerObjectName, AbstractDbType> GetUserDefinedTableType(SqlServerObjectName typeName)
	{
		return m_UserDefinedTableTypes.GetOrAdd(typeName, GetUserDefinedTableTypeInternal);
	}

	/// <summary>
	/// Gets the table-valued functions that were loaded by this cache.
	/// </summary>
	/// <returns>ICollection&lt;UserDefinedTableTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;&gt;.</returns>
	/// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
	public override IReadOnlyCollection<UserDefinedTableTypeMetadata<SqlServerObjectName, AbstractDbType>> GetUserDefinedTableTypes()
	{
		return m_UserDefinedTableTypes.GetValues();
	}

	/// <summary>
	/// Preloads all of the metadata for this data source.
	/// </summary>
	public override void Preload() => Preload(null);

	/// <summary>
	/// Preloads all of the metadata for this data source.
	/// </summary>
	/// <param name="schemaName">If not null, only the indicated schema will be loaded.</param>
	public void Preload(string? schemaName = null)
	{
		PreloadTables(schemaName);
		PreloadViews(schemaName);
		PreloadStoredProcedures(schemaName);
		PreloadTableFunctions(schemaName);
		PreloadUserDefinedTableTypes(schemaName);
		PreloadScalarFunctions(schemaName);
	}

	/// <summary>
	/// Resets the metadata cache, clearing out all cached metadata.
	/// </summary>
	public override void Reset()
	{
		m_StoredProcedures.Clear();
		m_TableFunctions.Clear();
		m_Tables.Clear();
		m_TypeTableMap.Clear();
		m_UserDefinedTableTypes.Clear();
		m_ScalarFunctions.Clear();
		m_ScalarFunctions.Clear();
		m_ObjectIdTableMap.Clear();
		m_DefaultSchema = null;
	}

	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	internal static void AdjustTypeDetails(string typeName, ref int? maxLength, ref int? precision, ref int? scale, out string fullTypeName)
	{
		switch (typeName)
		{
			case "bigint":
			case "bit":
			case "date":
			case "datetime":
			case "timestamp":
			case "tinyint":
			case "uniqueidentifier":
			case "smallint":
			case "sql_variant":
			case "float":
			case "int":
				maxLength = null;
				precision = null;
				scale = null;
				fullTypeName = typeName;
				break;

			case "binary":
			case "char":
				precision = null;
				scale = null;
				fullTypeName = $"{typeName}({maxLength})";
				break;

			case "datetime2":
			case "datetimeoffset":
			case "time":
				maxLength = null;
				precision = null;
				fullTypeName = $"{typeName}({scale})";
				break;

			case "numeric":
			case "decimal":
				fullTypeName = $"{typeName}({precision},{scale})";
				break;

			case "nchar":
				maxLength = maxLength / 2;
				precision = null;
				scale = null;
				fullTypeName = $"nchar({maxLength})";
				break;

			case "nvarchar":
				precision = null;
				scale = null;
				if (maxLength > 0)
				{
					maxLength = maxLength / 2;
					fullTypeName = $"nvarchar({maxLength})";
				}
				else
					fullTypeName = $"nvarchar(max)";
				break;

			case "varbinary":
			case "varchar":
				precision = null;
				scale = null;
				if (maxLength > 0)
					fullTypeName = $"{typeName}({maxLength})";
				else
					fullTypeName = $"{typeName}(max)";
				break;

			default:
				if (maxLength <= 0)
					maxLength = 0;
				if (precision <= 0)
					precision = 0;
				if (scale <= 0)
					scale = 0;
				fullTypeName = typeName;
				break;
		}
	}

	/// <summary>
	/// Parse a string and return the database specific representation of the object name.
	/// </summary>
	/// <param name="schema">The schema.</param>
	/// <param name="name">The name.</param>
	/// <returns>SqlServerObjectName.</returns>
	protected override SqlServerObjectName ParseObjectName(string? schema, string name)
	{
		if (schema == null)
			return new SqlServerObjectName(name);
		return new SqlServerObjectName(schema, name);
	}

	internal override TableOrViewMetadata<SqlServerObjectName, AbstractDbType> OnGetTableOrView(SqlServerObjectName tableName)
	{
		return GetTableOrView(tableName);
	}

	/// <summary>
	/// Gets the maximum number of parameters in a single SQL batch.
	/// </summary>
	/// <value>The maximum number of parameters.</value>
	/// <remarks>Note that the documentation says 2100, but you need to subtract one for the SQL statement itself. https://docs.microsoft.com/en-us/sql/sql-server/maximum-capacity-specifications-for-sql-server?view=sql-server-ver15</remarks>
	public override int? MaxParameters => 2099;

	/// <summary>
	/// Get the maximum number of rows in a single SQL statement's Values clause.
	/// </summary>
	public override int? MaxRowsPerValuesClause => 1000;


	/// <summary>
	/// Gets the name of the table or view.
	/// </summary>
	/// <param name="objectId">The object identifier.</param>
	public SqlServerObjectName GetTableOrViewName(int objectId)
	{
		return m_ObjectIdTableMap.GetOrAdd(objectId, x => GetTableOrViewNameInternal(x));
	}

	/// <summary>
	/// Gets the table or view from its object_id.
	/// </summary>
	/// <param name="objectId">The object identifier.</param>
	public SqlServerTableOrViewMetadata<AbstractDbType> GetTableOrView(int objectId)
	{
		return GetTableOrView(GetTableOrViewName(objectId));
	}

}
