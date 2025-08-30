using Npgsql;
using NpgsqlTypes;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Tortuga.Anchor;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql;

/// <summary>
/// Class PostgreSqlMetadataCache.
/// </summary>
public class PostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
{
	readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
	readonly ConcurrentDictionary<PostgreSqlObjectName, ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_ScalarFunctions = new();
	readonly ConcurrentDictionary<PostgreSqlObjectName, StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_StoredProcedures = new();
	readonly ConcurrentDictionary<PostgreSqlObjectName, TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TableFunctions = new();
	readonly ConcurrentDictionary<PostgreSqlObjectName, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_Tables = new();
	readonly ConcurrentDictionary<Type, TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> m_TypeTableMap = new();
	string? m_DatabaseName;
	ImmutableArray<string> m_DefaultSchemaList;
	ImmutableDictionary<PostgreSqlObjectName, ImmutableHashSet<string>>? m_SequenceColumns;
	Version? m_ServerVersion;
	string? m_ServerVersionName;

	static Regex s_DecimalMatcher = new(@"\d", RegexOptions.Compiled);

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlMetadataCache"/> class.
	/// </summary>
	/// <param name="connectionBuilder">The connection builder.</param>
	public PostgreSqlMetadataCache(NpgsqlConnectionStringBuilder connectionBuilder)
	{
		m_ConnectionBuilder = connectionBuilder;
	}

	/// <summary>
	/// Returns the current database.
	/// </summary>
	/// <returns></returns>
	public string DatabaseName
	{
		get
		{
			if (m_DatabaseName == null)
			{
				using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
				{
					con.Open();
					using (var cmd = new NpgsqlCommand("select current_database()", con))
					{
						m_DatabaseName = (string)cmd.ExecuteScalar()!;
					}
				}
			}
			return m_DatabaseName;
		}
	}

	/// <summary>
	/// Returns the user's default schema.
	/// </summary>
	/// <returns></returns>
	public ImmutableArray<string> DefaultSchemaList
	{
		get
		{
			if (m_DefaultSchemaList == default)
			{
				using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
				{
					con.Open();

					string currentUser;
					string defaultSchema;

					using (var cmd = new NpgsqlCommand("select current_user;", con))
					{
						currentUser = (string)cmd.ExecuteScalar()!;
					}

					using (var cmd = new NpgsqlCommand("SHOW search_path;", con))
					{
						defaultSchema = (string)cmd.ExecuteScalar()!;
					}
					defaultSchema = defaultSchema.Replace("\"$user\"", currentUser, StringComparison.Ordinal);
					m_DefaultSchemaList = defaultSchema.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToImmutableArray();
				}
			}
			return m_DefaultSchemaList;
		}
	}

	/// <summary>
	/// Gets the server version number.
	/// </summary>
	public override Version ServerVersion
	{
		get
		{
			if (m_ServerVersion == null)
			{
				using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
				{
					con.Open();

					using (var cmd = new NpgsqlCommand("SHOW server_version;", con))
					{
						var versionString = (string)cmd.ExecuteScalar()!;
						if (versionString.Contains(' ', StringComparison.Ordinal))
							versionString = versionString.Substring(0, versionString.IndexOf(' ', StringComparison.Ordinal));
						m_ServerVersion = Version.Parse(versionString);
					}
				}
			}
			return m_ServerVersion;
		}
	}

	/// <summary>
	/// Gets the server version name.
	/// </summary>
	public override string ServerVersionName
	{
		get
		{
			if (m_ServerVersionName == null)
			{
				using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
				{
					con.Open();

					using (var cmd = new NpgsqlCommand("SELECT version();", con))
						m_ServerVersionName = (string)cmd.ExecuteScalar()!;
				}
			}
			return m_ServerVersionName;
		}
	}

	/// <summary>
	/// Gets the indexes for a table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <returns></returns>
	/// <remarks>
	/// This should be read from a TableOrViewMetadata object. Do not call this method directly.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override IndexMetadataCollection<PostgreSqlObjectName, NpgsqlDbType> GetIndexesForTable(PostgreSqlObjectName tableName)
	{
		string indexSql;

		if (ServerVersion.Major < 11) //no included columns
		{
			indexSql =
@"SELECT
	idx.indexrelid as index_oid,
	ns.nspname as schema_name,
	tab.relname as table_name,
	cls.relname as index_name,
	am.amname as index_type,
	idx.indisprimary as is_primary,
	idx.indisunique as is_unique,
	idx.indkey as column_indices,
	idx.indnatts as key_column_count,
	idx.indoption as column_options
FROM
	pg_index idx
INNER JOIN pg_class cls ON cls.oid=idx.indexrelid
INNER JOIN pg_class tab ON tab.oid=idx.indrelid
INNER JOIN pg_am am ON am.oid=cls.relam
INNER JOIN pg_namespace ns on ns.oid=tab.relnamespace
WHERE ns.nspname = @Schema AND tab.relname = @Name";
		}
		else
		{
			indexSql =
@"SELECT
	idx.indexrelid as index_oid,
	ns.nspname as schema_name,
	tab.relname as table_name,
	cls.relname as index_name,
	am.amname as index_type,
	idx.indisprimary as is_primary,
	idx.indisunique as is_unique,
	idx.indkey as column_indices,
	idx.indnkeyatts as key_column_count,
	idx.indoption as column_options
FROM
	pg_index idx
INNER JOIN pg_class cls ON cls.oid=idx.indexrelid
INNER JOIN pg_class tab ON tab.oid=idx.indrelid
INNER JOIN pg_am am ON am.oid=cls.relam
INNER JOIN pg_namespace ns on ns.oid=tab.relnamespace
WHERE ns.nspname = @Schema AND tab.relname = @Name";
		}

		var table = GetTableOrView(tableName);

		var results = new List<IndexMetadata<PostgreSqlObjectName, NpgsqlDbType>>();
		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		using (var con2 = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			con2.Open();
			using (var cmd = new NpgsqlCommand(indexSql, con))
			{
				cmd.Parameters.AddWithValue("@Schema", tableName.Schema!);
				cmd.Parameters.AddWithValue("@Name", tableName.Name);
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var indexOid = reader.GetUInt32("index_oid");
						var name = reader.GetString("index_name");
						var isUnique = reader.GetBoolean("is_unique");

						var isPrimaryKey = reader.GetBoolean("is_primary");
						var isUniqueConstraint = false; //TASk-285: Identify unique indexes that are also unique constraints
						var keyColumnCount = reader.GetInt16("key_column_count");

						//Task-284: Index size

						var indexType = reader.GetString("index_type") switch
						{
							"btree" => IndexType.BTree,
							"hash" => IndexType.Hash,
							"gist" => IndexType.Gist,
							"gin" => IndexType.Gin,
							"spgist" => IndexType.Spgist,
							"brin" => IndexType.Brin,
							_ => IndexType.Unknown,
						};
						var columnIndices = reader.GetValue<short[]>("column_indices");
						var descendingColumns = new Dictionary<int, bool?>();

						var columnSql = @"SELECT p.colIndex, pg_index_column_has_property(@Oid,p.colIndex,'desc') AS descending from generate_series(1, @IndexCount) p(colIndex)";

						using (var cmd2 = new NpgsqlCommand(columnSql, con2))
						{
							cmd2.Parameters.AddWithValue("@Oid", NpgsqlDbType.Oid, indexOid);
							cmd2.Parameters.AddWithValue("@IndexCount", columnIndices.Length);
							using (var reader2 = cmd2.ExecuteReader())
							{
								while (reader2.Read())
								{
									descendingColumns[reader2.GetInt32("colIndex")] = reader2.GetBooleanOrNull("descending");
									//Other column properties can be added here
								}
							}
						}

						var columns = new List<IndexColumnMetadata<NpgsqlDbType>>();
						for (var i = 0; i < columnIndices.Length; i++)
						{
							//Note: The values in columnIndices is 1-based, so we need to offset it.
							var column = table.Columns[columnIndices[i] - 1];
							var isIncluded = i < keyColumnCount;
							var isDescending = descendingColumns[i + 1];

							columns.Add(new IndexColumnMetadata<NpgsqlDbType>(column, isDescending, isIncluded));
						}

						results.Add(new IndexMetadata<PostgreSqlObjectName, NpgsqlDbType>(tableName, name, isPrimaryKey, isUnique, isUniqueConstraint, new IndexColumnMetadataCollection<NpgsqlDbType>(columns), null, null, indexType));
					}
				}
			}
		}
		return new IndexMetadataCollection<PostgreSqlObjectName, NpgsqlDbType>(results);
	}

	/// <summary>
	/// Gets the foreign keys for a table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <returns>ForeignKeyConstraintCollection&lt;MySqlObjectName, MySqlDbType&gt;.</returns>
	/// <remarks>This should be read from a TableOrViewMetadata object. Do not call this method directly.</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override ForeignKeyConstraintCollection<PostgreSqlObjectName, NpgsqlDbType> GetForeignKeysForTable(AbstractObjectName tableName)
	{
		var table = GetTableOrView(tableName);
		var results = new List<ForeignKeyConstraint<PostgreSqlObjectName, NpgsqlDbType>>();

		var scratch = new List<FKTemp>();

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(@$"SELECT *
FROM (
	SELECT
		conname AS ""constraint_name"",
		con.constrained_schema,
		con.constrained_table,
		att1.attname AS ""constrained_column"",
		ns2.nspname AS ""referenced_schema"",
		cl2.relname AS ""referenced_table"",
		att2.attname AS ""referenced_column""
	FROM (
		SELECT
			unnest(con1.conkey) AS ""parent"",
			unnest(con1.confkey) AS ""child"",
			con1.confrelid,
			con1.conrelid,
			con1.conname,
			cl.relname AS ""constrained_table"",
			ns.nspname AS ""constrained_schema""
		FROM pg_class cl
		JOIN pg_namespace ns
			ON cl.relnamespace = ns.oid
		JOIN pg_constraint con1
			ON con1.conrelid = cl.oid
		WHERE con1.contype = 'f'
	) con
	JOIN pg_attribute att2
		ON att2.attrelid = con.confrelid AND att2.attnum = con.child
	JOIN pg_class cl2
		ON cl2.oid = con.confrelid
	JOIN pg_attribute att1
		ON att1.attrelid = con.conrelid AND att1.attnum = con.parent
	JOIN pg_namespace ns2
		ON cl2.relnamespace = ns2.oid
)
WHERE (constrained_schema = '{tableName.Schema}' AND constrained_table = '{tableName.Name}')
		OR
	  (referenced_schema = '{tableName.Schema}' AND referenced_table = '{tableName.Name}')
", con))
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					scratch.Add(new FKTemp()
					{
						ConstraintName = reader.GetString("constraint_name"),
						ConstrainedSchemaName = reader.GetString("constrained_schema"),
						ConstrainedTableName = reader.GetString("constrained_table"),
						ConstrainedColumnName = reader.GetString("constrained_column"),
						ReferencedSchemaName = reader.GetString("referenced_schema"),
						ReferencedTableName = reader.GetString("referenced_table"),
						ReferencedColumnName = reader.GetString("referenced_column")
					});
				}
			}
		}

		foreach (var fkName in scratch.GroupBy(x => (x.ConstraintName, x.ConstrainedSchemaName, x.ConstrainedTableName, x.ReferencedSchemaName, x.ReferencedTableName)))
		{
			TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> constrainedTable;
			TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> referencedTable;

			if (fkName.Key.ConstrainedSchemaName == table.Name.Schema && fkName.Key.ConstrainedTableName == table.Name.Name)
			{
				constrainedTable = table;
				referencedTable = GetTableOrView(new PostgreSqlObjectName(fkName.Key.ReferencedSchemaName, fkName.Key.ReferencedTableName));
			}
			else
			{
				constrainedTable = GetTableOrView(new PostgreSqlObjectName(fkName.Key.ConstrainedSchemaName, fkName.Key.ConstrainedTableName));
				referencedTable = table;
			}
			var columns = fkName.OrderBy(x => x.Order).ToList();
			var constrainedColumns = new ColumnMetadataCollection<NpgsqlDbType>(fkName.Key.ConstraintName, columns.Select(c => constrainedTable.Columns[c.ConstrainedColumnName]).ToList());
			var referencedColumns = new ColumnMetadataCollection<NpgsqlDbType>(fkName.Key.ConstraintName, columns.Select(c => referencedTable.Columns[c.ReferencedColumnName]).ToList());

			results.Add(new(fkName.Key.ConstraintName, constrainedTable.Name, constrainedColumns, referencedTable.Name, referencedColumns));
		}

		return new ForeignKeyConstraintCollection<PostgreSqlObjectName, NpgsqlDbType>(results);
	}

	sealed class FKTemp
	{
		public string ConstrainedColumnName { get; set; } = "";
		public string ConstrainedSchemaName { get; set; } = "";
		public string ConstrainedTableName { get; set; } = "";
		public string ConstraintName { get; set; } = "";
		public int Order { get; set; }
		public string ReferencedColumnName { get; set; } = "";
		public string ReferencedSchemaName { get; set; } = "";
		public string ReferencedTableName { get; set; } = "";
	}

	/// <summary>
	/// Gets the metadata for a scalar function.
	/// </summary>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <returns>Null if the object could not be found.</returns>
	public override ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetScalarFunction(PostgreSqlObjectName scalarFunctionName)
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
	public override IReadOnlyCollection<ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetScalarFunctions() => m_ScalarFunctions.GetValues();

	/// <summary>
	/// Gets the stored procedure's metadata.
	/// </summary>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public override StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedure(PostgreSqlObjectName procedureName)
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
	public override IReadOnlyCollection<StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetStoredProcedures() => m_StoredProcedures.GetValues();

	/// <summary>
	/// Gets the metadata for a table function.
	/// </summary>
	/// <param name="tableFunctionName">Name of the table function.</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public override TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunction(PostgreSqlObjectName tableFunctionName)
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
	public override IReadOnlyCollection<TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTableFunctions() => m_TableFunctions.GetValues();

	/// <summary>
	/// Gets the tables and views that were loaded by this cache.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
	/// </remarks>
	public override IReadOnlyCollection<TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>> GetTablesAndViews()
	{
		return m_Tables.GetValues();
	}

	/// <summary>
	/// Preloads all of the metadata for this data source.
	/// </summary>
	public override void Preload()
	{
		PreloadTables();
		PreloadViews();
		PreloadTableFunctions();
		PreloadStoredProcedures();
		PreloadScalarFunctions();
	}

	/// <summary>
	/// Preloads the scalar functions.
	/// </summary>
	public void PreloadScalarFunctions()
	{
		const string TvfSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type<>'record';";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(TvfSql, con))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString("routine_schema");
						var name = reader.GetString("routine_name");
						GetScalarFunction(new PostgreSqlObjectName(schema, name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Preloads the table value functions.
	/// </summary>
	public void PreloadStoredProcedures()
	{
		const string procSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type='refcursor';";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(procSql, con))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString("routine_schema");
						var name = reader.GetString("routine_name");
						GetStoredProcedure(new PostgreSqlObjectName(schema, name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Preloads the table value functions.
	/// </summary>
	public void PreloadTableFunctions()
	{
		const string TvfSql = @"SELECT routine_schema, routine_name FROM information_schema.routines where routine_type = 'FUNCTION' AND data_type='record';";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(TvfSql, con))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString("routine_schema");
						var name = reader.GetString("routine_name");
						GetTableFunction(new PostgreSqlObjectName(schema, name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Preloads the metadata for all tables.
	/// </summary>
	public void PreloadTables()
	{
		const string TableSql =
			@"SELECT
				table_schema as schemaname,
				table_name as tablename,
				table_type as type
				FROM information_schema.tables
				WHERE table_type='BASE TABLE' AND
					  table_schema<>'pg_catalog' AND
					  table_schema<>'information_schema';";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(TableSql, con))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString("schemaname");
						var name = reader.GetString("tablename");
						GetTableOrView(new PostgreSqlObjectName(schema, name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Preloads the metadata for all views.
	/// </summary>
	public void PreloadViews()
	{
		const string ViewSql =
			@"SELECT
				table_schema as schemaname,
				table_name as tablename,
				table_type as type
				FROM information_schema.tables
				WHERE table_type='VIEW' AND
					  table_schema<>'pg_catalog' AND
					  table_schema<>'information_schema';";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();
			using (var cmd = new NpgsqlCommand(ViewSql, con))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString("schemaname");
						var name = reader.GetString("tablename");
						GetTableOrView(new PostgreSqlObjectName(schema, name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Resets the metadata cache, clearing out all cached metadata.
	/// </summary>
	public override void Reset()
	{
		m_DatabaseName = null;
		m_DefaultSchemaList = default;
		m_SequenceColumns = null;
		m_StoredProcedures.Clear();
		m_TableFunctions.Clear();
		m_Tables.Clear();
		m_TypeTableMap.Clear();
		m_ScalarFunctions.Clear();
	}

	/// <summary>
	/// Determines the database column type from the column type name.
	/// </summary>
	/// <param name="typeName">Name of the database column type.</param>
	/// <param name="isUnsigned">NOT USED</param>
	/// <returns></returns>
	/// <remarks>This does not honor registered types. This is only used for the database's hard-coded list of native types.</remarks>
	protected override NpgsqlDbType? SqlTypeNameToDbType(string typeName, bool? isUnsigned = null)
	{
		if (string.IsNullOrEmpty(typeName))
			throw new ArgumentException($"{nameof(typeName)} is null or empty.", nameof(typeName));

#pragma warning disable CS0618 // Type or member is obsolete
		return typeName.ToUpperInvariant().Replace("_", "", StringComparison.Ordinal).Replace("\"", "", StringComparison.Ordinal) switch
		{
			"ABSTIME" => NpgsqlDbType.Abstime,
			"ARRAY" => NpgsqlDbType.Array,
			"BIGINT" => NpgsqlDbType.Bigint,
			"BIT" => NpgsqlDbType.Bit,
			"BOOL" => NpgsqlDbType.Boolean,
			"BOOLEAN" => NpgsqlDbType.Boolean,
			"BOX" => NpgsqlDbType.Box,
			"BYTEA" => NpgsqlDbType.Bytea,
			"CHAR" => NpgsqlDbType.Char,
			"CHARACTER" or "CID" => NpgsqlDbType.Cid,
			"CIDR" => NpgsqlDbType.Cidr,
			"CIRCLE" => NpgsqlDbType.Circle,
			"CITEXT" => NpgsqlDbType.Citext,
			"DATE" => NpgsqlDbType.Date,
			"DOUBLE" => NpgsqlDbType.Double,
			"DOUBLE PRECISION" => NpgsqlDbType.Double,
			"FLOAT4" => NpgsqlDbType.Real,
			"FLOAT8" => NpgsqlDbType.Double,
			"GEOGRAPHY" => NpgsqlDbType.Geography,
			"GEOMETRY" => NpgsqlDbType.Geometry,
			"HSTORE" => NpgsqlDbType.Hstore,
			"INET" => NpgsqlDbType.Inet,
			"INT2" => NpgsqlDbType.Smallint,
			"INT2VECTOR" => NpgsqlDbType.Int2Vector,
			"INT4" => NpgsqlDbType.Integer,
			"INT8" => NpgsqlDbType.Bigint,
			"INTEGER" => NpgsqlDbType.Integer,
			"INTERVAL" => NpgsqlDbType.Interval,
			"JSON" => NpgsqlDbType.Json,
			"JSONB" => NpgsqlDbType.Jsonb,
			"LINE" => NpgsqlDbType.Line,
			"LSEG" => NpgsqlDbType.LSeg,
			"MACADDR" => NpgsqlDbType.MacAddr,
			"MACADDR8" => NpgsqlDbType.MacAddr8,
			"MONEY" => NpgsqlDbType.Money,
			"NAME" => NpgsqlDbType.Name,
			"NUMERIC" => NpgsqlDbType.Numeric,
			"OID" => NpgsqlDbType.Oid,
			"OIDVECTOR" => NpgsqlDbType.Oidvector,
			"PATH" => NpgsqlDbType.Path,
			"POINT" => NpgsqlDbType.Point,
			"POLYGON" => NpgsqlDbType.Polygon,
			"RANGE" => NpgsqlDbType.Range,
			"REAL" => NpgsqlDbType.Real,
			"REFCURSOR" => NpgsqlDbType.Refcursor,
			"REGCONFIG" => NpgsqlDbType.Regconfig,
			"REGTYPE" => NpgsqlDbType.Regtype,
			"SMALLINT" => NpgsqlDbType.Smallint,
			"TEXT" => NpgsqlDbType.Text,
			"TID" => NpgsqlDbType.Tid,
			"TIME WITH TIME ZONE" => NpgsqlDbType.TimeTz,
			"TIMETZ" => NpgsqlDbType.TimeTz,
			"TIME" => NpgsqlDbType.Time,
			"TIME WITHOUT TIME ZONE" => NpgsqlDbType.Time,
			"TIMESTAMP WITH TIME ZONE" => NpgsqlDbType.TimestampTz,
			"TIMESTAMPTZ" => NpgsqlDbType.TimestampTz,
			"TIMESTAMP" => NpgsqlDbType.Timestamp,
			"TIMESTAMP WITHOUT TIME ZONE" => NpgsqlDbType.Timestamp,
			"TSQUERY" => NpgsqlDbType.TsQuery,
			"TSVECTOR" => NpgsqlDbType.TsVector,
			"UUID" => NpgsqlDbType.Uuid,
			"VARBIT" => NpgsqlDbType.Varbit,
			"BIT VARYING" => NpgsqlDbType.Varbit,
			"VARCHAR" => NpgsqlDbType.Varchar,
			"CHARACTER VARYING" => NpgsqlDbType.Varchar,
			"XID" => NpgsqlDbType.Xid,
			"XML" => NpgsqlDbType.Xml,
			_ => null,
		};
#pragma warning restore CS0618 // Type or member is obsolete
	}

	/// <summary>
	/// Gets a list of known, unsupported SQL type names that cannot be mapped to a NpgsqlDbType.
	/// </summary>
	/// <value>Case-insensitive list of database-specific type names</value>
	/// <remarks>This list is based on driver limitations.</remarks>
	public override ImmutableHashSet<string> UnsupportedSqlTypeNames { get; } = ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, ["trigger", "internal", "regclass", "bpchar", "pg_lsn", "void", "cstring", "reltime", "anyenum", "anyarray", "anyelement", "anyrange", "_regdictionary", "any", "regdictionary", "tstzrange", "jsonpath", "pg_mcv_list", "table_am_handler", "ANYNONARRAY", "UNKNOWN", "PG_DDL_COMMAND", "TINTERVAL", "RECORD", "OPAQUE", "REGROLE", "_CSTRING", "REGOPERATOR", "_ACLITEM", "ACLITEM", "REGPROCEDURE", "\"ANY\"", "SMGR", "TXID_SNAPSHOT", "PG_NODE_TREE", "EVENT_TRIGGER", "INDEX_AM_HANDLER", "INT8RANGE", "REGPROC", "PG_ATTRIBUTE", "PG_TYPE", "REGOPER", "REGNAMESPACE", "INT4RANGE", "PG_DEPENDENCIES", "FDW_HANDLER", "TSM_HANDLER", "LANGUAGE_HANDLER", "DATERANGE", "GTSVECTOR", "NUMRANGE", "TSRANGE", "PG_NDISTINCT", "anycompatiblenonarray", "anymultirange", "pg_snapshot", "pg_brin_bloom_summary", "anycompatiblearray", "int8multirange", "regcollation", "anycompatiblemultirange", "tsmultirange", "anycompatible", "xid8", "datemultirange", "anycompatiblerange", "pg_brin_minmax_multi_summary", "int4multirange", "nummultirange", "tstzmultirange"]);

	/// <summary>
	/// Parse a string and return the database specific representation of the object name.
	/// </summary>
	/// <param name="schema">The schema.</param>
	/// <param name="name">The name.</param>
	/// <returns>PostgreSqlObjectName.</returns>
	protected override PostgreSqlObjectName ParseObjectName(string? schema, string name)
	{
		if (schema == null)
			return new PostgreSqlObjectName(name);
		return new PostgreSqlObjectName(schema, name);
	}

	[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "<Pending>")]
	Tuple<ParameterMetadataCollection<NpgsqlDbType>, ColumnMetadataCollection<NpgsqlDbType>> GetParametersAndColumns(string specificName, NpgsqlConnection connection)
	{
		const string parameterSql = @"SELECT * FROM information_schema.parameters WHERE specific_name = @SpecificName ORDER BY ordinal_position";

		var parameters = new List<ParameterMetadata<NpgsqlDbType>>();
		var columns = new List<ColumnMetadata<NpgsqlDbType>>();
		using (var cmd = new NpgsqlCommand(parameterSql, connection))
		{
			cmd.Parameters.AddWithValue("@SpecificName", specificName);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					if (string.Equals(reader.GetString("parameter_mode"), "IN", StringComparison.Ordinal))
					{
						var parameterName = reader.GetStringOrNull("parameter_name") ?? "Parameter" + reader.GetInt32("ordinal_position");

						var typeName = reader.GetString("udt_name");
						var isNullable = true;
						int? maxLength = null;
						int? precision = null;
						int? scale = null;
						var fullTypeName = ""; //Task-291: Add support for full name

						//Task-120: Add support for length, precision, and scale
						//Task-384: OUTPUT Parameters for PostgreSQL
						var direction = ParameterDirection.Input;

						parameters.Add(new ParameterMetadata<NpgsqlDbType>(parameterName, "@" + parameterName, typeName, SqlTypeNameToDbType(typeName), isNullable, maxLength, precision, scale, fullTypeName, direction));
					}
					else
					{
						var name = reader.GetString("parameter_name");
						var typeName = reader.GetString("udt_name");
						var isPrimary = false;
						var isIdentity = false;
						bool? isNullable = null;
						int? maxLength = null;
						int? precision = null;
						int? scale = null;
						var fullTypeName = ""; //Task-291: Add support for full name

						//Task-120: Add support for length, precision, and scale

						columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, isPrimary, isIdentity, typeName, SqlTypeNameToDbType(typeName), "\"" + name + "\"", isNullable, maxLength, precision, scale, fullTypeName, ToClrType(typeName, isNullable ?? true, maxLength)));
					}
				}
			}
		}
		return Tuple.Create(new ParameterMetadataCollection<NpgsqlDbType>(specificName, parameters), new ColumnMetadataCollection<NpgsqlDbType>(specificName, columns));
	}

	ColumnMetadataCollection<NpgsqlDbType> GetColumns(PostgreSqlObjectName tableName, NpgsqlConnection connection)
	{
		const string columnSql =
@"
SELECT att.attname as column_name,
	   t.typname as data_type,
	   pk.contype as is_primary_key,
	   att.attnotnull as not_null,
	   att.attidentity as is_identity,
	   format_type(att.atttypid, att.atttypmod) as data_type_full,
	   COL_DESCRIPTION(CONCAT(ns.nspname, '.', c.relname)::regclass, att.attnum) as description
FROM pg_class as c
JOIN pg_namespace as ns on ns.oid=c.relnamespace
JOIN pg_attribute as att on c.oid=att.attrelid AND
							att.attnum>0
JOIN pg_type as t on t.oid=att.atttypid
LEFT JOIN (SELECT cnst.conrelid,
				  cnst.conkey,
				  cnst.contype
		   FROM pg_constraint as cnst
		   WHERE cnst.contype='p') pk ON att.attnum=ANY(pk.conkey) AND
										 pk.conrelid=c.oid
WHERE c.relname ILIKE @Name AND
	  ns.nspname ILIKE @Schema;";

		var columns = new List<ColumnMetadata<NpgsqlDbType>>();
		var sequenceColumns = GetSequenceColumns(tableName);

		using (var cmd = new NpgsqlCommand(columnSql, connection))
		{
			cmd.Parameters.AddWithValue("@Schema", tableName.Schema!);
			cmd.Parameters.AddWithValue("@Name", tableName.Name);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var name = reader.GetString("column_name");
					var typeName = reader.GetString("data_type");
					var description = reader.GetStringOrNull("description");
					var isPrimary = !reader.IsDBNull("is_primary_key");

					var identity_type = char.ToUpperInvariant(reader.GetCharOrNull("is_identity") ?? ' ');
					var isIdentity = identity_type == 'A' || identity_type == 'D';
					if (!isIdentity) //check if its attached to a sequence
						isIdentity = sequenceColumns.Contains(name);

					var isNullable = !reader.GetBoolean("not_null");

					var dbType = SqlTypeNameToDbType(typeName);
					var fullTypeName = reader.GetString("data_type_full");

					var match1 = s_DecimalMatcher.Match(fullTypeName);
					var match2 = match1.Success ? match1.NextMatch() : null;
					var value1 = match1.Success ? int.Parse(match1.Value, CultureInfo.InvariantCulture) : (int?)null;
					var value2 = match2?.Success == true ? int.Parse(match2.Value, CultureInfo.InvariantCulture) : (int?)null;

					int? maxLength = null;
					int? precision = null;
					int? scale = null;

					switch (dbType)
					{
						case NpgsqlDbType.Char:
						case NpgsqlDbType.Varchar:
						case NpgsqlDbType.Bit:
						case NpgsqlDbType.Varbit:
							maxLength = value1;
							break;

						case NpgsqlDbType.Time:
						case NpgsqlDbType.TimeTz:
						case NpgsqlDbType.Timestamp:
						case NpgsqlDbType.TimestampTz:
							precision = value1;
							break;

						case NpgsqlDbType.Numeric:
							precision = value1;
							scale = value2;
							if (precision.HasValue && scale == null)
								scale = 0;
							break;
					}

					columns.Add(new ColumnMetadata<NpgsqlDbType>(name, false, isPrimary, isIdentity, typeName, dbType, QuoteColumnName(name), isNullable, maxLength, precision, scale, fullTypeName, ToClrType(typeName, isNullable, maxLength)) { Description = description });
				}
			}
		}

		return new ColumnMetadataCollection<NpgsqlDbType>(tableName.ToString(), columns);
	}

	/// <summary>
	/// Quotes the name of the column.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>System.String.</returns>
	/// <remarks>This assumes the column name wasn't already quoted.</remarks>
	public override string QuoteColumnName(string columnName)
	{
		if (columnName == "*")
			return columnName;

		return "\"" + columnName + "\"";
	}

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
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	protected override Type? ToClrType(NpgsqlDbType dbType, bool isNullable, int? maxLength)
	{
		return dbType switch
		{
			NpgsqlDbType.Bigint => isNullable ? typeof(long?) : typeof(long),
			NpgsqlDbType.Double => isNullable ? typeof(double?) : typeof(double),
			NpgsqlDbType.Integer => isNullable ? typeof(int?) : typeof(int),
			NpgsqlDbType.Numeric or NpgsqlDbType.Money => isNullable ? typeof(decimal?) : typeof(decimal),
			NpgsqlDbType.Real => isNullable ? typeof(float?) : typeof(float),
			NpgsqlDbType.Smallint => isNullable ? typeof(short?) : typeof(short),
			NpgsqlDbType.Boolean => isNullable ? typeof(bool?) : typeof(bool),
			NpgsqlDbType.Char or NpgsqlDbType.Varchar => (maxLength == 1) ? (isNullable ? typeof(char?) : typeof(char)) : typeof(string),
			NpgsqlDbType.Text => typeof(string),
			NpgsqlDbType.Bytea => typeof(byte[]),
			NpgsqlDbType.Date or NpgsqlDbType.Timestamp => isNullable ? typeof(DateTime?) : typeof(DateTime),
			NpgsqlDbType.Time => isNullable ? typeof(TimeSpan?) : typeof(TimeSpan),
			NpgsqlDbType.Interval => isNullable ? typeof(TimeSpan?) : typeof(TimeSpan),
			NpgsqlDbType.Bit => typeof(BitArray),
			NpgsqlDbType.Uuid => isNullable ? typeof(Guid?) : typeof(Guid),
			NpgsqlDbType.Xml or NpgsqlDbType.Json or NpgsqlDbType.Jsonb => typeof(string),
			NpgsqlDbType.Inet or NpgsqlDbType.Cidr or NpgsqlDbType.MacAddr or NpgsqlDbType.MacAddr8 or NpgsqlDbType.Varbit or NpgsqlDbType.TsVector or NpgsqlDbType.TsQuery or NpgsqlDbType.Regconfig or NpgsqlDbType.Hstore or NpgsqlDbType.Array or NpgsqlDbType.Range or NpgsqlDbType.Refcursor or NpgsqlDbType.Oidvector or NpgsqlDbType.Int2Vector or NpgsqlDbType.Oid or NpgsqlDbType.Xid or NpgsqlDbType.Cid or NpgsqlDbType.Regtype or NpgsqlDbType.Tid or NpgsqlDbType.Unknown or NpgsqlDbType.Geometry or NpgsqlDbType.Geography or NpgsqlDbType.Box or NpgsqlDbType.Circle or NpgsqlDbType.Line or NpgsqlDbType.LSeg or NpgsqlDbType.Path or NpgsqlDbType.Point or NpgsqlDbType.Polygon or NpgsqlDbType.Name or NpgsqlDbType.Citext or NpgsqlDbType.InternalChar => null,
			_ => null,
		};
	}

	ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetScalarFunctionInternal(PostgreSqlObjectName tableFunctionName)
	{
		const string functionSql = @"SELECT routine_schema, routine_name, specific_name, data_type FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type<>'record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();

			foreach (var schema in GetSchemasToCheck(tableFunctionName))
			{
				string actualSchema;
				string actualName;
				string specificName;
				string typeName;

				using (var cmd = new NpgsqlCommand(functionSql, con))
				{
					cmd.Parameters.AddWithValue("@Schema", schema);
					cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
					using (var reader = cmd.ExecuteReader())
					{
						if (!reader.Read())
							continue;

						actualSchema = reader.GetString("routine_schema");
						actualName = reader.GetString("routine_name");
						specificName = reader.GetString("specific_name");
						typeName = reader.GetString("data_type");
					}
				}

				var pAndC = GetParametersAndColumns(specificName, con);
				var isNullable = true;
				int? maxLength = null;
				int? precision = null;
				int? scale = null;
				var fullTypeName = ""; //Task-291: Add support for full name

				//Task-120: Add support for length, precision, and scale for return type

				return new ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1, typeName, SqlTypeNameToDbType(typeName), isNullable, maxLength, precision, scale, fullTypeName);
			}
		}

		throw new MissingObjectException($"Could not find scalar function {tableFunctionName}");
	}

	ImmutableArray<string> GetSchemasToCheck(PostgreSqlObjectName objectName) => objectName.Schema == null ? DefaultSchemaList : [objectName.Schema];

	ImmutableHashSet<string> GetSequenceColumns(PostgreSqlObjectName tableName)
	{
		const string sql = @"select s.relname as SequenceName, n.nspname as SchemaName, t.relname as TableName, a.attname as ColumnName
from pg_class s
  join pg_depend d on d.objid=s.oid and d.classid='pg_class'::regclass and d.refclassid='pg_class'::regclass
  join pg_class t on t.oid=d.refobjid
  join pg_namespace n on n.oid=t.relnamespace
  join pg_attribute a on a.attrelid=t.oid and a.attnum=d.refobjsubid
where s.relkind='S' and d.deptype='a'";

		if (m_SequenceColumns == null)
		{
			using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
			{
				con.Open();

				using (var cmd = new NpgsqlCommand(sql, con))
				{
					using (var reader = cmd.ExecuteReader())
					{
						var result = new Dictionary<PostgreSqlObjectName, HashSet<string>>();
						while (reader.Read())
						{
							var schemaTableName = new PostgreSqlObjectName(reader.GetString("SchemaName"), reader.GetString("TableName"));
							var columnName = reader.GetString("ColumnName");

							if (result.TryGetValue(schemaTableName, out var identityColumns))
							{
								identityColumns.Add(columnName);
							}
							else
							{
								identityColumns = new HashSet<string>() { columnName };
								result.Add(schemaTableName, identityColumns);
							}
						}
						m_SequenceColumns = result.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase));
					}
				}
			}
		}
		return m_SequenceColumns.GetValueOrDefault(tableName, ImmutableHashSet<string>.Empty);
	}

	StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> GetStoredProcedureInternal(PostgreSqlObjectName storedProcedureName)
	{
		const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='refcursor' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();

			foreach (var schema in GetSchemasToCheck(storedProcedureName))
			{
				string actualSchema;
				string actualName;
				string specificName;
				using (var cmd = new NpgsqlCommand(functionSql, con))
				{
					cmd.Parameters.AddWithValue("@Schema", schema);
					cmd.Parameters.AddWithValue("@Name", storedProcedureName.Name);
					using (var reader = cmd.ExecuteReader())
					{
						if (!reader.Read())
							continue;
						actualSchema = reader.GetString("routine_schema");
						actualName = reader.GetString("routine_name");
						specificName = reader.GetString("specific_name");
					}
				}

				var pAndC = GetParametersAndColumns(specificName, con);

				return new StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1);
			}
		}

		throw new MissingObjectException($"Could not find function {storedProcedureName}");
	}

	TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableFunctionInternal(PostgreSqlObjectName tableFunctionName)
	{
		const string functionSql = @"SELECT routine_schema, routine_name, specific_name FROM information_schema.routines WHERE routine_type = 'FUNCTION' AND data_type='record' AND routine_schema ILIKE @Schema AND routine_name ILIKE @Name;";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();

			foreach (var schema in GetSchemasToCheck(tableFunctionName))
			{
				string actualSchema;
				string actualName;
				string specificName;

				using (var cmd = new NpgsqlCommand(functionSql, con))
				{
					cmd.Parameters.AddWithValue("@Schema", schema);
					cmd.Parameters.AddWithValue("@Name", tableFunctionName.Name);
					using (var reader = cmd.ExecuteReader())
					{
						if (!reader.Read())
							continue;

						actualSchema = reader.GetString("routine_schema");
						actualName = reader.GetString("routine_name");
						specificName = reader.GetString("specific_name");
					}
				}

				var pAndC = GetParametersAndColumns(specificName, con);

				return new TableFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType>(new PostgreSqlObjectName(actualSchema, actualName), pAndC.Item1, pAndC.Item2);
			}
		}

		throw new MissingObjectException($"Could not find function {tableFunctionName}");
	}

	/// <summary>
	/// Gets the detailed metadata for a table or view.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <returns>SqlServerTableOrViewMetadata&lt;TDbType&gt;.</returns>
	public override TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrView(PostgreSqlObjectName tableName)
	{
		return m_Tables.GetOrAdd(tableName, GetTableOrViewInternal);
	}

	/// <summary>
	/// Gets the detailed metadata for a table or view.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <exception cref="MissingObjectException">Could not find table or view {tableName}</exception>
	public TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrViewInternal(PostgreSqlObjectName tableName)
	{
		const string TableSql =
@"SELECT
	table_schema as schemaname,
	table_name as tablename,
	table_type as type,
	OBJ_DESCRIPTION(CONCAT(table_schema, '.', table_name)::regclass) as description
FROM information_schema.tables
WHERE table_schema ILIKE @Schema AND
		table_name ILIKE @Name AND
		(table_type='BASE TABLE' OR
		table_type='VIEW');";

		using (var con = new NpgsqlConnection(m_ConnectionBuilder.ConnectionString))
		{
			con.Open();

			foreach (var schmea in GetSchemasToCheck(tableName))
			{
				string actualSchema;
				string actualTableName;
				bool isTable;
				string? description;

				using (var cmd = new NpgsqlCommand(TableSql, con))
				{
					cmd.Parameters.AddWithValue("@Schema", schmea);
					cmd.Parameters.AddWithValue("@Name", tableName.Name);
					using (var reader = cmd.ExecuteReader())
					{
						if (!reader.Read())
							continue; //try the next schema in the search path
						actualSchema = reader.GetString("schemaname");
						actualTableName = reader.GetString("tablename");
						var type = reader.GetString("type");
						isTable = type.Equals("BASE TABLE", StringComparison.Ordinal);
						description = reader.GetStringOrNull("description");
					}
				}

				var actualName = new PostgreSqlObjectName(actualSchema, actualTableName);
				var columns = GetColumns(actualName, con);
				return new TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType>(this, actualName, isTable, columns) { Description = description };
			}
		}

		throw new MissingObjectException($"Could not find table or view {tableName}");
	}

	/// <summary>
	/// Gets the maximum number of parameters in a single SQL batch.
	/// </summary>
	/// <value>The maximum number of parameters.</value>
	/// <remarks>https://stackoverflow.com/a/6582902/5274</remarks>
	public override int? MaxParameters => 34464;
}
