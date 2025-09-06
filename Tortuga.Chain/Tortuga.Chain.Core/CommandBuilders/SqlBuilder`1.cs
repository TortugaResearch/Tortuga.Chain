using System.Collections.Immutable;
using System.Data.Common;
using System.Text;
using Tortuga.Anchor;
using Tortuga.Anchor.ComponentModel;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>Class SqlBuilder. This class cannot be inherited.</summary>
/// <typeparam name="TDbType">The type of the database type.</typeparam>
public sealed class SqlBuilder<TDbType>
	where TDbType : struct
{
	readonly SqlBuilderEntry<TDbType>[] m_Entries;

	readonly string m_Name;

	internal SqlBuilder(string name, IReadOnlyList<ColumnMetadata<TDbType>> columns, IReadOnlyList<ParameterMetadata<TDbType>> parameters)
	{
		m_Name = name;

		m_Entries = new SqlBuilderEntry<TDbType>[columns.Count + parameters.Count];

		for (var i = 0; i < columns.Count; i++)
		{
			var column = columns[i];
			m_Entries[i] = new SqlBuilderEntry<TDbType>()
			{
				Details = column,
				IsKey = column.IsPrimaryKey,
				UseForInsert = !column.IsComputed && !column.IsIdentity,
				UseForUpdate = !column.IsComputed && !column.IsIdentity
			};
		}

		var offset = columns.Count;
		for (var i = 0; i < parameters.Count; i++)
		{
			var column = parameters[i];
			m_Entries[offset + i] = new SqlBuilderEntry<TDbType>()
			{
				Details = column,
				IsFormalParameter = true
			};
		}
	}

	internal SqlBuilder(string name, IReadOnlyList<ColumnMetadata<TDbType>> columns)
	{
		m_Name = name;

		m_Entries = new SqlBuilderEntry<TDbType>[columns.Count];

		for (var i = 0; i < columns.Count; i++)
		{
			var column = columns[i];
			m_Entries[i] = new SqlBuilderEntry<TDbType>()
			{
				Details = column,
				IsKey = column.IsPrimaryKey,
				UseForInsert = !column.IsComputed && !column.IsIdentity,
				UseForUpdate = !column.IsComputed && !column.IsIdentity
			};
		}
	}

	internal SqlBuilder(string name, IReadOnlyList<ParameterMetadata<TDbType>> parameters)
	{
		m_Name = name;

		m_Entries = new SqlBuilderEntry<TDbType>[parameters.Count];
		for (var i = 0; i < parameters.Count; i++)
		{
			var column = parameters[i];
			m_Entries[i] = new SqlBuilderEntry<TDbType>()
			{
				Details = column,
				IsFormalParameter = true
			};
		}
	}

	SqlBuilder(string name, SqlBuilderEntry<TDbType>[] entries, bool strictMode)
	{
		m_Name = name;
#pragma warning disable IDE0305 // Simplify collection initialization
		m_Entries = entries.ToArray(); //since this is an array of struct, this does a deep copy
#pragma warning restore IDE0305 // Simplify collection initialization
		StrictMode = strictMode;
	}

	/// <summary>
	/// Gets a value indicating whether this instance has fields marked for reading.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance has read fields; otherwise, <c>false</c>.
	/// </value>
	public bool HasReadFields
	{
		get
		{
			for (var i = 0; i < m_Entries.Length; i++)
				if (m_Entries[i].UseForRead)
					return true;
			return false;
		}
	}

	/// <summary>
	/// Gets a value indicating whether strict mode is enabled
	/// </summary>
	public bool StrictMode { get; }

	/// <summary>
	/// Applies the filter value, returning a set of expressions suitable for use in a WHERE clause.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <param name="useSecondSlot">if set to <c>true</c> uses the second parameter slot.</param>
	/// <returns>System.String.</returns>
	/// <exception cref="ArgumentNullException">filterValue - filterValue</exception>
	/// <exception cref="MappingException">
	/// </exception>
	/// <exception cref="ArgumentNullException"></exception>
	public string ApplyAnonymousFilterValue(object filterValue, FilterOptions filterOptions, bool useSecondSlot = false)
	{
		if (filterValue == null)
			throw new ArgumentNullException(nameof(filterValue), $"{nameof(filterValue)} is null.");

		var ignoreNullProperties = filterOptions.HasFlag(FilterOptions.IgnoreNullProperties);
		var parts = new string[m_Entries.Length];
		var found = false;

		if (filterValue is IReadOnlyDictionary<string, object> readOnlyDictionary)
		{
			foreach (var item in readOnlyDictionary)
			{
				var keyFound = false;
				for (var i = 0; i < m_Entries.Length; i++)
				{
					ref var entry = ref m_Entries[i];

					if (string.Equals(entry.Details.ClrName, item.Key, StringComparison.OrdinalIgnoreCase)
						|| string.Equals(entry.Details.SqlName, item.Key, StringComparison.OrdinalIgnoreCase))
					{
						var value = item.Value ?? DBNull.Value;

						if (value == DBNull.Value)
						{
							if (!ignoreNullProperties)
								parts[i] = ($"{entry.Details.QuotedSqlName} IS NULL");
						}
						else
						{
							entry.ParameterValue = value;
							if (!useSecondSlot)
								entry.UseParameter = true;
							else
								entry.UseParameter2 = true;
							parts[i] = ($"{entry.Details.QuotedSqlName} = ?");
						}

						found = true;
						keyFound = true;
						break;
					}
				}
				if (StrictMode && !keyFound)
					throw new MappingException($"Strict mode was enabled, but property {item.Key} could be matched to a column in {m_Name}. Disable strict mode or remove the item from the dictionary.");
			}
		}
		else
		{
			foreach (var property in MetadataCache.GetMetadata(filterValue.GetType()).Properties.Where(p => p.MappedColumnName != null && p.CanRead))
			{
				var propertyFound = false;
				for (var i = 0; i < m_Entries.Length; i++)
				{
					ref var entry = ref m_Entries[i];

					if (entry.Details.ClrName.Equals(property.MappedColumnName, StringComparison.OrdinalIgnoreCase))
					{
						var value = property.InvokeGet(filterValue) ?? DBNull.Value;

						if (value == DBNull.Value)
						{
							if (!ignoreNullProperties)
								parts[i] = ($"{entry.Details.QuotedSqlName} IS NULL");
						}
						else
						{
							entry.ParameterValue = value;
							if (!useSecondSlot)
								entry.UseParameter = true;
							else
								entry.UseParameter2 = true;
							parts[i] = ($"{entry.Details.QuotedSqlName} = ?");
						}

						found = true;
						propertyFound = true;
						break;
					}
				}
				if (StrictMode && !propertyFound)
					throw new MappingException($"Strict mode was enabled, but property {property.Name} could be matched to a column in {m_Name}. Disable strict mode or mark the property as NotMapped.");
			}
		}

		if (!found)
			throw new MappingException($"None of the properties on {filterValue.GetType().Name} could be matched to columns in {m_Name}.");

		return string.Join(" AND ", parts.Where(s => s != null));
	}

	/// <summary>
	/// Applies the argument value for an undefined operation.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The value.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.</remarks>
	public void ApplyArgumentValue(IDataSource dataSource, object? argumentValue)
	{
		ApplyArgumentValue(dataSource, OperationTypes.None, argumentValue, false, false, false);
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="operationType">Type of the operation.</param>
	/// <param name="argumentValue">The value.</param>
	/// <param name="nonQueryDelete">No result will be read from this operation.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.</remarks>
	public void ApplyArgumentValue(IDataSource dataSource, OperationTypes operationType, object? argumentValue, bool nonQueryDelete)
	{
		ApplyArgumentValue(dataSource, operationType, argumentValue, false, false, nonQueryDelete);
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The value.</param>
	/// <param name="options">The options.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.
	/// </remarks>
	public void ApplyArgumentValue(IDataSource dataSource, object argumentValue, InsertOptions options)
	{
		ApplyArgumentValue(dataSource, OperationTypes.Insert, argumentValue, false, false, false);
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The value.</param>
	/// <param name="options">The options.</param>
	/// <param name="nonQuery">No result will be read from this operation.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.
	/// </remarks>
	public void ApplyArgumentValue(IDataSource dataSource, object argumentValue, DeleteOptions options, bool nonQuery)
	{
		ApplyArgumentValue(dataSource, OperationTypes.Delete, argumentValue, options.HasFlag(DeleteOptions.UseKeyAttribute), false, nonQuery);
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The value.</param>
	/// <param name="options">The options.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.
	/// If the object does not implement IPropertyChangeTracking, the changedPropertiesOnly flag has no effect.
	/// </remarks>
	public void ApplyArgumentValue(IDataSource dataSource, object argumentValue, UpsertOptions options)
	{
		ApplyArgumentValue(dataSource, OperationTypes.InsertOrUpdate, argumentValue, options.HasFlag(UpsertOptions.UseKeyAttribute), options.HasFlag(UpsertOptions.ChangedPropertiesOnly), false);
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="argumentValue">The value.</param>
	/// <param name="options">The options.</param>
	/// <param name="nonQueryDelete">No result will be read from this operation. This is ignored unless using UpdateOptions.SoftDelete.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.
	/// If the object does not implement IPropertyChangeTracking and changedPropertiesOnly is set, an error will occur.
	/// </remarks>
	public void ApplyArgumentValue(IDataSource dataSource, object? argumentValue, UpdateOptions options, bool nonQueryDelete)
	{
		if (options.HasFlag(UpdateOptions.SoftDelete))
			ApplyArgumentValue(dataSource, OperationTypes.Delete, argumentValue, options.HasFlag(UpdateOptions.UseKeyAttribute), options.HasFlag(UpdateOptions.ChangedPropertiesOnly), nonQueryDelete);
		else
			ApplyArgumentValue(dataSource, OperationTypes.Update, argumentValue, options.HasFlag(UpdateOptions.UseKeyAttribute), options.HasFlag(UpdateOptions.ChangedPropertiesOnly), false);
	}

	/// <summary>
	/// Uses a desired columns enumeration to indicate which columns should be set to read-mode.
	/// </summary>
	/// <param name="desiredColumns">The desired columns. This also supports Materializer.NoColumns, Materializer.AutoSelectDesiredColumns, and Materializer.AllColumns.</param>
	/// <exception cref="MappingException">This is thrown is no desired columns were actually part of the table or view. If strict mode, all desired columns must be found.</exception>
	/// <remarks>Calling this a second time will be additive with prior call.</remarks>
	public void ApplyDesiredColumns(IEnumerable<string> desiredColumns)
	{
		if (desiredColumns == null)
			throw new ArgumentNullException(nameof(desiredColumns), $"{nameof(desiredColumns)} is null.");

		var found = false;

		if (desiredColumns == Materializer.NoColumns)
			return;//no-op, we default m_Entries[i].Read to false

		if (desiredColumns == Materializer.AutoSelectDesiredColumns)
		{
			//we want primary keys
			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];
				if (entry.IsKey)
				{
					entry.UseForRead = true;
					found = true;
				}
			}
			if (found)
				return;

			//we will accept identity columns
			{
				for (var i = 0; i < m_Entries.Length; i++)
				{
					ref var entry = ref m_Entries[i];
					if (entry.Details.IsIdentity)
					{
						entry.UseForRead = true;
						found = true;
					}
				}
			}
			if (found)
				return;

			//we have nothing
			if (!found)
				throw new MappingException($"Could not find a primary key for {m_Name}.");
		}

		if (desiredColumns == Materializer.AllColumns)
		{
			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];

				if (entry.Details.SqlName != null)
					entry.UseForRead = true;
			}

			return;
		}

		var desiredColumnsList = desiredColumns.AsList();

		//This has to go last. THe special case versions of desired columns also have 0 named columns.
		if (!desiredColumnsList.Any())
			throw new ChainInternalException("No desired columns were requested.");

		foreach (var column in desiredColumnsList)
		{
			var columnFound = false;

			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];

				if (string.Equals(entry.Details.SqlName, column, StringComparison.OrdinalIgnoreCase))
				{
					entry.UseForRead = true;
					columnFound = true;
					found = true;
					break;
				}
				else if (string.Equals(entry.Details.ClrName, column, StringComparison.OrdinalIgnoreCase))
				{
					entry.UseClrNameAsAlias = true;
					entry.UseForRead = true;
					columnFound = true;
					found = true;
					break;
				}
				else if (string.Equals(entry.Details.QuotedSqlName, column, StringComparison.OrdinalIgnoreCase))
				{
					throw new MappingException($"Modify the ColumnAttribute for the desired column \"{column}\" to be \"{entry.Details.SqlName}\". SQL Quoted column names are not supported.");
				}
			}

			if (StrictMode && !columnFound)
				throw new MappingException($"Strict mode was enabled, but desired column {column} was not found on {m_Name}. Disable strict mode or mark the property as NotMapped.");
		}

		if (!found)
			throw new MappingException($"None of the desired columns were found on {m_Name}."
				+ Environment.NewLine + "\tAvailable columns: " + string.Join(", ", m_Entries.Select(c => c.Details.SqlName))
				+ Environment.NewLine + "\tDesired columns: " + string.Join(", ", desiredColumns)
				);
	}

	/// <summary>
	/// Applies the filter value, returning a set of expressions suitable for use in a WHERE clause.
	/// </summary>
	/// <param name="filterValue">The filter value.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <returns>System.String.</returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	/// <exception cref="MappingException">
	/// </exception>
	/// <exception cref="ArgumentNullException"></exception>
	public string ApplyFilterValue(object filterValue, FilterOptions filterOptions)
	{
		if (filterValue == null)
			throw new ArgumentNullException(nameof(filterValue), $"{nameof(filterValue)} is null.");

		var ignoreNullProperties = filterOptions.HasFlag(FilterOptions.IgnoreNullProperties);
		var parts = new List<string>();
		var found = false;

		if (filterValue is IReadOnlyDictionary<string, object> readOnlyDictionary)
		{
			foreach (var item in readOnlyDictionary)
			{
				var keyFound = false;
				for (var i = 0; i < m_Entries.Length; i++)
				{
					ref var entry = ref m_Entries[i];

					if (string.Equals(entry.Details.ClrName, item.Key, StringComparison.OrdinalIgnoreCase)
						|| string.Equals(entry.Details.SqlName, item.Key, StringComparison.OrdinalIgnoreCase))
					{
						var value = item.Value ?? DBNull.Value;

						if (value == DBNull.Value)
						{
							if (!ignoreNullProperties)
								parts.Add($"{entry.Details.QuotedSqlName} IS NULL");
						}
						else
						{
							entry.ParameterValue = value;
							entry.UseParameter = true;
							parts.Add($"{entry.Details.QuotedSqlName} = {entry.Details.SqlVariableName}");
						}

						found = true;
						keyFound = true;
						break;
					}
				}
				if (StrictMode && !keyFound)
					throw new MappingException($"Strict mode was enabled, but property {item.Key} could be matched to a column in {m_Name}. Disable strict mode or remove the item from the dictionary.");
			}
		}
		else
		{
			foreach (var property in MetadataCache.GetMetadata(filterValue.GetType()).Properties.Where(p => p.MappedColumnName != null && p.CanRead))
			{
				var propertyFound = false;
				for (var i = 0; i < m_Entries.Length; i++)
				{
					ref var entry = ref m_Entries[i];

					if (entry.Details.ClrName.Equals(property.MappedColumnName, StringComparison.OrdinalIgnoreCase))
					{
						var value = property.InvokeGet(filterValue) ?? DBNull.Value;

						if (value == DBNull.Value)
						{
							if (!ignoreNullProperties)
								parts.Add($"{entry.Details.QuotedSqlName} IS NULL");
						}
						else
						{
							entry.ParameterValue = value;
							entry.UseParameter = true;
							parts.Add($"{entry.Details.QuotedSqlName} = {entry.Details.SqlVariableName}");
						}

						found = true;
						propertyFound = true;
						break;
					}
				}
				if (StrictMode && !propertyFound)
					throw new MappingException($"Strict mode was enabled, but property {property.Name} could be matched to a column in {m_Name}. Disable strict mode or mark the property as NotMapped.");
			}
		}

		if (!found)
			throw new MappingException($"None of the properties on {filterValue.GetType().Name} could be matched to columns in {m_Name}.");

		return string.Join(" AND ", parts);
	}

	/// <summary>
	/// Applies the audit rules for select operations.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	public void ApplyRulesForSelect(IDataSource dataSource)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		ApplyRules(dataSource.AuditRules, OperationTypes.Select, null, dataSource.UserValue);
	}

	/// <summary>
	/// Applies a user defined table type as the argument.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="operationType">Type of the operation.</param>
	/// <param name="tableTypeColumns">The table type columns.</param>
	/// <exception cref="ArgumentNullException">
	/// dataSource
	/// or
	/// tableTypeColumns
	/// </exception>
	/// <exception cref="MappingException">None of the columns on the indicated user defined type could be matched to columns in {m_Name}.</exception>
	public void ApplyTableType(IDataSource dataSource, OperationTypes operationType, IEnumerable<ISqlBuilderEntryDetails<TDbType>> tableTypeColumns)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
		if (tableTypeColumns == null)
			throw new ArgumentNullException(nameof(tableTypeColumns), $"{nameof(tableTypeColumns)} is null.");

		var found = false;

		foreach (var column in tableTypeColumns)
		{
			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];
				if (string.Equals(entry.Details.SqlName, column.SqlName, StringComparison.OrdinalIgnoreCase))
				{
					found = true;
					entry.ParameterColumn = column;
				}
			}
		}

		if (!found)
			throw new MappingException($"None of the columns on the indicated user defined type could be matched to columns in {m_Name}.");

		ApplyRules(dataSource.AuditRules, operationType, null, dataSource.UserValue);
	}

	/// <summary>
	/// Builds FROM clause for a function.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="header">The header.</param>
	/// <param name="footer">The footer.</param>
	public void BuildAnonymousFromFunctionClause(StringBuilder sql, string header, string footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetFormalParameters().Select(_ => "?")));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a list of assignments suitable for using in the SET clause of UPDATE statement. This does not include the actual SET keyword.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually not used.</param>
	/// <param name="prefix">An optional prefix for each column name.</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	public void BuildAnonymousSetClause(StringBuilder sql, string? header, string? prefix, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetUpdateColumns().Select(x => $"{prefix}{x.QuotedSqlName} = ?")));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds the soft delete clause.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="header">The header.</param>
	/// <param name="dataSource">The data source.</param>
	/// <param name="footer">The footer.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public void BuildAnonymousSoftDeleteClause(StringBuilder sql, string header, IDataSource dataSource, string? footer)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		var softDeletes = dataSource.AuditRules.SoftDeleteForSelect;

		if (softDeletes.Length == 0)
			return;

		var applicableColumns = new HashSet<SqlBuilderEntry<TDbType>>();

		for (var i = 0; i < m_Entries.Length; i++)
		{
			foreach (var rule in softDeletes)
			{
				ref var entry = ref m_Entries[i];

				if (string.Equals(entry.Details.SqlName, rule.ColumnName, StringComparison.OrdinalIgnoreCase) || entry.Details.ClrName.Equals(rule.ColumnName, StringComparison.OrdinalIgnoreCase))
				{
					entry.ParameterValue = rule.DeletedValue;
					entry.UseParameter2 = true;
					applicableColumns.Add(entry);
				}
			}
		}

		if (applicableColumns.Count > 0)
		{
			sql.Append(header);
			sql.Append(string.Join(" AND ", applicableColumns.Select(x => x.Details.QuotedSqlName + " <> ?")));
			sql.Append(footer);
		}
	}

	/// <summary>
	/// Builds a list of columns suitable for using in the VALUES clause of INSERT statement. This does not include the actual VALUES keyword.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually "VALUES  (".</param>
	/// <param name="footer">The optional footer. Usually just ")".</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	public void BuildAnonymousValuesClause(StringBuilder sql, string header, string footer, bool includeIdentityColumn = false)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetInsertColumns(includeIdentityColumn).Select(x => "?")));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds the standard WHERE clause from Key columns.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header Usually "WHERE".</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	/// <param name="firstPassParameter">if set to true if this is a first pass parameter.</param>
	/// <exception cref="System.ArgumentNullException">sql - sql</exception>
	public void BuildAnonymousWhereClause(StringBuilder sql, string? header, string? footer, bool firstPassParameter)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		if (firstPassParameter)
			sql.Append(string.Join(" AND ", GetKeyColumns().Select(x => x.QuotedSqlName + " = ?")));
		else
			sql.Append(string.Join(" AND ", GetKeyColumns2().Select(x => x.QuotedSqlName + " = ?")));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a complete select statement using the keys as a filter. This is mostly used in conjunction with an UPDATE or DELETE operation.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="footer">Optional footer, usually the statement terminator (;).</param>
	public void BuildDeleteStatement(StringBuilder sql, string tableName, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");
		if (string.IsNullOrEmpty(tableName))
			throw new ArgumentException($"{nameof(tableName)} is null or empty.", nameof(tableName));

		sql.Append("DELETE FROM ").Append(tableName);
		BuildWhereClause(sql, " WHERE ", footer);
	}

	/// <summary>
	/// Builds FROM clause for a function.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="header">The header.</param>
	/// <param name="footer">The footer.</param>
	/// <param name="parameterPrefix">Prefix to apply to each parameter name.</param>
	public void BuildFromFunctionClause(StringBuilder sql, string header, string footer, string? parameterPrefix = null)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetFormalParameters().Select(s => parameterPrefix + s)));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a list of columns suitable for using in an INSERT statement.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. e.g. "INSERT tableName (".</param>
	/// <param name="prefix">An optional prefix for each column name.</param>
	/// <param name="footer">The optional footer. Usually just ")".</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	public void BuildInsertClause(StringBuilder sql, string? header, string? prefix, string? footer, bool includeIdentityColumn = false)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetInsertColumns(includeIdentityColumn).Select(x => prefix + x.QuotedSqlName)));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a complete insert statement.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="footer">The footer.</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	public void BuildInsertStatement(StringBuilder sql, string tableName, string? footer, bool includeIdentityColumn = false)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");
		if (string.IsNullOrEmpty(tableName))
			throw new ArgumentException($"{nameof(tableName)} is null or empty.", nameof(tableName));

		BuildInsertClause(sql, $"INSERT INTO {tableName} (", null, ")", includeIdentityColumn);
		BuildValuesClause(sql, " VALUES (", ")", includeIdentityColumn);
		sql.Append(footer);
	}

	/// <summary>
	/// Builds an order by clause.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="header">The header.</param>
	/// <param name="sortExpressions">The sort expressions.</param>
	/// <param name="footer">The footer.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="MappingException"></exception>
	public void BuildOrderByClause(StringBuilder sql, string? header, IEnumerable<SortExpression> sortExpressions, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");

		if (sortExpressions?.Any() != true)
			return;

		sql.Append(header);

		var isFirst = true;

		foreach (var expression in sortExpressions)
		{
			//this shouldn't happen, but if it does we'll just ignore it.
			if (expression.ColumnName == null)
				continue;

			if (!isFirst)
				sql.Append(", ");

			if (expression.Direction == SortDirection.Expression)
			{
				sql.Append(expression.ColumnName);
			}
			else
			{
				var columnIndex = FindColumnIndexByName(expression.ColumnName);
				if (columnIndex.HasValue)
				{
					sql.Append(m_Entries[columnIndex.Value].Details.QuotedSqlName! + (expression.Direction == SortDirection.Descending ? " DESC " : null));
				}
				else if (expression.ColumnName.EndsWith(" ACS", StringComparison.OrdinalIgnoreCase))
				{
					var truncateedName = expression.ColumnName[..^4];
					columnIndex = FindColumnIndexByName(truncateedName);
					if (columnIndex.HasValue)
						sql.Append(m_Entries[columnIndex.Value].Details.QuotedSqlName!);
					else
						throw new MappingException($"Cannot find a column on {m_Name} named '{truncateedName}' or '{expression.ColumnName}'");
				}
				else if (expression.ColumnName.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
				{
					var truncateedName = expression.ColumnName[..^5];
					columnIndex = FindColumnIndexByName(truncateedName);
					if (columnIndex.HasValue)
						sql.Append(m_Entries[columnIndex.Value].Details.QuotedSqlName! + " DESC ");
					else
						throw new MappingException($"Cannot find a column on {m_Name} named '{truncateedName}' or '{expression.ColumnName}'");
				}
				else
					throw new MappingException($"Cannot find a column on {m_Name} named '{expression.ColumnName}'");
			}
			isFirst = false;
		}

		sql.Append(footer);
	}

	/// <summary>
	/// Builds a complete select statement using the keys as a filter. This is mostly used in conjunction with an UPDATE or DELETE operation.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="footer">Optional footer, usually the statement terminator (;).</param>
	public void BuildSelectByKeyStatement(StringBuilder sql, string tableName, string footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} is null.");
		if (string.IsNullOrEmpty(tableName))
			throw new ArgumentException($"{nameof(tableName)} is null or empty.", nameof(tableName));

		if (!HasReadFields)
			return;

		BuildSelectClause(sql, "SELECT ", null, $" FROM {tableName}");
		BuildWhereClause(sql, " WHERE ", footer);
	}

	/// <summary>
	/// Builds a list of columns suitable for using in a SELECT or OUTPUT statement.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header (e.g. "SELECT, OUTPUT).</param>
	/// <param name="prefix">An optional prefix for each column name.</param>
	/// <param name="footer">The optional footer.</param>
	/// <remarks>
	/// If no columns are marked for reading, the header and footer won't be emitted.
	/// </remarks>
	public void BuildSelectClause(StringBuilder sql, string? header, string? prefix, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		if (!HasReadFields)
			return;

		sql.Append(header);
		sql.Append(string.Join(", ", GetSelectColumns().Select(x => prefix + x)));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a list of columns suitable for using in a SELECT from @TableParameter clause.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header (e.g. "SELECT, OUTPUT).</param>
	/// <param name="prefix">An optional prefix for each column name.</param>
	/// <param name="footer">The optional footer.</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	/// <remarks>
	/// If no columns are marked for reading, the header and footer won't be emitted.
	/// </remarks>
	public void BuildSelectTvpForInsertClause(StringBuilder sql, string? header, string? prefix, string? footer, bool includeIdentityColumn = false)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		var parts = new List<string>();

		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if (!entry.RestrictedInsert && (entry.UseForInsert || (includeIdentityColumn && entry.Details.IsIdentity)))
			{
				if (entry.ParameterValue != null)
				{
					entry.UseParameter = true;
					parts.Add(prefix + entry.Details.SqlVariableName);
				}
				else if (entry.ParameterColumn != null)
				{
					parts.Add(prefix + entry.ParameterColumn.QuotedSqlName);
				}
			}
		}

		sql.Append(header);
		sql.Append(string.Join(", ", parts));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a list of assignments suitable for using in the SET clause of UPDATE statement. This does not include the actual SET keyword.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually not used.</param>
	/// <param name="prefix">An optional prefix for each column name.</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	public void BuildSetClause(StringBuilder sql, string? header, string? prefix, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetUpdateColumns().Select(x => $"{prefix}{x.QuotedSqlName} = {x.SqlVariableName}")));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds the soft delete clause.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually not used.</param>
	/// <param name="dataSource">The data source.</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public void BuildSoftDeleteClause(StringBuilder sql, string? header, IDataSource dataSource, string? footer)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		var softDeletes = dataSource.AuditRules.SoftDeleteForSelect;

		if (softDeletes.Length == 0)
			return;

		var applicableColumns = new HashSet<SqlBuilderEntry<TDbType>>();

		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			foreach (var rule in softDeletes)
			{
				if (string.Equals(entry.Details.SqlName, rule.ColumnName, StringComparison.OrdinalIgnoreCase) || entry.Details.ClrName.Equals(rule.ColumnName, StringComparison.OrdinalIgnoreCase))
				{
					entry.ParameterValue = rule.DeletedValue;
					entry.UseParameter = true;
					applicableColumns.Add(entry);
				}
			}
		}

		if (applicableColumns.Count > 0)
		{
			sql.Append(header);
			sql.Append(string.Join(" AND ", applicableColumns.Select(x => x.Details.QuotedSqlName + " <> " + x.Details.SqlVariableName)));
			sql.Append(footer);
		}
	}

	/// <summary>
	/// Builds the complete update statement using primary keys.
	/// </summary>
	/// <param name="sql">The SQL.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="footer">The footer.</param>
	public void BuildUpdateByKeyStatement(StringBuilder sql, string tableName, string? footer)
	{
		BuildSetClause(sql, $"UPDATE {tableName} SET ", null, null);
		BuildWhereClause(sql, " WHERE ", null);
		sql.Append(footer);
	}

	/// <summary>
	/// Builds a list of columns suitable for using in the VALUES clause of INSERT statement. This does not include the actual VALUES keyword.
	/// This will add the associated parameters to the parameters collection.
	/// </summary>
	/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually "VALUES  (".</param>
	/// <param name="footer">The optional footer. Usually just ")".</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	/// <param name="parameterSuffix">The parameter suffix. Must be unique for each row.</param>
	/// <param name="parameters">The parameter list to be updated.</param>
	/// <param name="parameterBuilder">The parameter builder.</param>
	/// <exception cref="ArgumentNullException">
	/// sql
	/// or
	/// value
	/// or
	/// parameters
	/// or
	/// parameterBuilder
	/// </exception>
	/// <remarks>Call OverrideArgumentValue before invoking this method</remarks>
	public void BuildValuesClause<TParameter>(StringBuilder sql, string header, string footer, bool includeIdentityColumn, string parameterSuffix, List<TParameter> parameters, ParameterBuilderCallback<TParameter, TDbType> parameterBuilder)
		where TParameter : DbParameter
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");
		if (parameters == null)
			throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} was null.");
		if (parameterBuilder == null)
			throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetInsertColumns(includeIdentityColumn).Select(x => x.SqlVariableName + parameterSuffix)));
		sql.Append(footer);

		var temp = GetParameters(parameterBuilder);
		foreach (var item in temp)
			item.ParameterName += parameterSuffix;
		parameters.AddRange(temp);
	}

	/// <summary>
	/// Builds a list of columns suitable for using in the VALUES clause of INSERT statement. This does not include the actual VALUES keyword.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header. Usually "VALUES  (".</param>
	/// <param name="footer">The optional footer. Usually just ")".</param>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	public void BuildValuesClause(StringBuilder sql, string header, string footer, bool includeIdentityColumn = false)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(", ", GetInsertColumns(includeIdentityColumn).Select(x => x.SqlVariableName)));
		sql.Append(footer);
	}

	/// <summary>
	/// Builds the standard WHERE clause from Key columns.
	/// This will mark key columns for use in parameter building.
	/// </summary>
	/// <param name="sql">The SQL being generated.</param>
	/// <param name="header">The optional header Usually "WHERE".</param>
	/// <param name="footer">The optional footer. Usually not used.</param>
	public void BuildWhereClause(StringBuilder sql, string? header, string? footer)
	{
		if (sql == null)
			throw new ArgumentNullException(nameof(sql), $"{nameof(sql)} was null.");

		sql.Append(header);
		sql.Append(string.Join(" AND ", GetKeyColumns().Select(x => $"{x.QuotedSqlName} = {x.SqlVariableName}")));
		sql.Append(footer);
	}

	/// <summary>
	/// Gets every column with a ParameterValue.
	/// </summary>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "<Pending>")]
	public IEnumerable<string> GetFormalParameters()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (m_Entries[i].IsFormalParameter && m_Entries[i].ParameterValue != null)
				yield return m_Entries[i].Details.SqlVariableName;
	}

	/// <summary>
	/// Gets the insert columns.
	/// </summary>
	/// <param name="includeIdentityColumn">Include the identity column. Used when performing an identity insert operation.</param>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	/// <remarks>This will mark the returned columns as participating in the parameter generation.</remarks>
	public IEnumerable<ColumnNamePair> GetInsertColumns(bool includeIdentityColumn = false)
	{
		for (var i = 0; i < m_Entries.Length; i++)
		{
			if (!m_Entries[i].RestrictedInsert && (m_Entries[i].UseForInsert || (includeIdentityColumn && m_Entries[i].Details.IsIdentity)) && (m_Entries[i].ParameterValue != null || m_Entries[i].ParameterColumn != null))
			{
				m_Entries[i].UseParameter = true;
				yield return new ColumnNamePair(m_Entries[i].Details.QuotedSqlNameSafe(), m_Entries[i].Details.SqlVariableName);
			}
		}
	}

	/// <summary>
	/// Gets the key columns.
	/// </summary>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	/// <remarks>This will mark the returned columns as participating in the parameter generation.</remarks>
	public IEnumerable<ColumnNamePair> GetKeyColumns()
	{
		for (var i = 0; i < m_Entries.Length; i++)
		{
			if (m_Entries[i].IsKey)
			{
				m_Entries[i].UseParameter = true;
				yield return new ColumnNamePair(m_Entries[i].Details.QuotedSqlNameSafe(), m_Entries[i].Details.SqlVariableName);
			}
		}
	}

	/// <summary>
	/// Gets the key columns.
	/// </summary>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	/// <remarks>This will mark the returned columns as participating in the second pass of parameter generation.</remarks>
	public IEnumerable<ColumnNamePair> GetKeyColumns2()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (m_Entries[i].IsKey)
			{
				m_Entries[i].UseParameter2 = true;
				yield return new ColumnNamePair(m_Entries[i].Details.QuotedSqlNameSafe(), m_Entries[i].Details.SqlVariableName);
			}
	}

	/// <summary>
	/// Gets every column with a ParameterValue.
	/// </summary>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	/// <remarks>This will mark the returned columns as participating in the parameter generation.</remarks>
	public IEnumerable<ColumnNamePair> GetParameterizedColumns()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (m_Entries[i].ParameterValue != null)
			{
				m_Entries[i].UseParameter = true;
				yield return new ColumnNamePair(m_Entries[i].Details.QuotedSqlNameSafe(), m_Entries[i].Details.SqlVariableName);
			}
	}

	/// <summary>
	/// Gets the parameters.
	/// </summary>
	/// <typeparam name="TParameter">The type of the parameter.</typeparam>
	/// <param name="parameterBuilder">The parameter builder. This should set the parameter's database specific DbType property.</param>
	/// <returns></returns>
	public List<TParameter> GetParameters<TParameter>(ParameterBuilderCallback<TParameter, TDbType> parameterBuilder)
		where TParameter : DbParameter
	{
		if (parameterBuilder == null)
			throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} is null.");

		var result = new List<TParameter>();

		//first pass
		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if ((entry.UseParameter && entry.ParameterValue != null) || entry.IsOutputParameter)
				result.Add(parameterBuilder(entry));
		}

		//second pass
		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if (entry.UseParameter2 && entry.ParameterValue != null)
				result.Add(parameterBuilder(entry));
		}

		return result;
	}

	/// <summary>
	/// Gets the parameters, but puts the keys last.
	/// </summary>
	/// <typeparam name="TParameter">The type of the parameter.</typeparam>
	/// <param name="parameterBuilder">The parameter builder. This should set the parameter's database specific DbType property.</param>
	/// <returns></returns>
	/// <remarks>This is needed for positional parameters such as MS Access</remarks>
	public List<TParameter> GetParametersKeysLast<TParameter>(ParameterBuilderCallback<TParameter, TDbType> parameterBuilder)
		where TParameter : DbParameter
	{
		if (parameterBuilder == null)
			throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} is null.");

		var result = new List<TParameter>();

		//Add non-key values first
		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if (entry.UseParameter && entry.ParameterValue != null && !entry.IsKey)
				result.Add(parameterBuilder(entry));
		}

		//Now add key values
		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if (entry.UseParameter && entry.ParameterValue != null && entry.IsKey)
				result.Add(parameterBuilder(entry));
		}

		return result;
	}

	/// <summary>
	/// Gets the select columns with metadata details.
	/// </summary>
	/// <returns>Each entry has the column's metadata</returns>
	[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "<Pending>")]
	public IEnumerable<ISqlBuilderEntryDetails<TDbType>> GetSelectColumnDetails()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (!m_Entries[i].RestrictedRead && m_Entries[i].UseForRead)
				yield return m_Entries[i].Details;
	}

	/// <summary>
	/// Gets the select columns.
	/// </summary>
	/// <returns>Each entry has the column's QuotedSqlName</returns>
	public IEnumerable<string> GetSelectColumns()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (!m_Entries[i].RestrictedRead && m_Entries[i].UseForRead)
			{
				if (!m_Entries[i].UseClrNameAsAlias)
					yield return m_Entries[i].Details.QuotedSqlNameSafe();
				else
					yield return m_Entries[i].Details.QuotedSqlNameSafe() + " AS " + m_Entries[i].Details.ClrName;
			}
	}

	/// <summary>
	/// Gets the update columns.
	/// </summary>
	/// <returns>Each pair has the column's QuotedSqlName and SqlVariableName</returns>
	/// <remarks>This will mark the returned columns as participating in the parameter generation.</remarks>
	public IEnumerable<ColumnNamePair> GetUpdateColumns()
	{
		for (var i = 0; i < m_Entries.Length; i++)
			if (!m_Entries[i].RestrictedUpdate && m_Entries[i].UseForUpdate && m_Entries[i].ParameterValue != null)
			{
				m_Entries[i].UseParameter = true;
				yield return new ColumnNamePair(m_Entries[i].Details.QuotedSqlNameSafe(), m_Entries[i].Details.SqlVariableName);
			}
	}

	/// <summary>
	/// Overrides the previous selected values with the values in the indicated object.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="operationType">The type of operation being performed.</param>
	/// <param name="argumentValue">The value.</param>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <remarks>This will not alter the IsPrimaryKey, Insert, or Update column settings.
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.</remarks>
	public void OverrideArgumentValue(IDataSource dataSource, OperationTypes operationType, object? argumentValue)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		if (argumentValue == null)
			throw new ArgumentNullException(nameof(argumentValue), $"{nameof(argumentValue)} is null.");

		if (argumentValue is IReadOnlyDictionary<string, object> readOnlyDictionary)
		{
			ApplyArgumentDictionary(readOnlyDictionary);
			return;
		}

		var found = false;

		var metadata = MetadataCache.GetMetadata(argumentValue.GetType());
		foreach (var property in metadata.Properties)
		{
			var propertyFound = false;

			if (property.MappedColumnName == null)
				continue;

			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];

				if (entry.Details.ClrName.Equals(property.MappedColumnName, StringComparison.OrdinalIgnoreCase))
				{
					found = true;
					propertyFound = true;
					if (entry.IsFormalParameter)
						entry.UseParameter = true;
					entry.ParameterValue = property.InvokeGet(argumentValue) ?? DBNull.Value;
					break;
				}
			}
			if (StrictMode && !propertyFound)
				throw new MappingException($"Strict mode was enabled, but property {property.Name} could be matched to a column in {m_Name}. Disable strict mode or mark the property as NotMapped.");
		}

		if (!found)
			throw new MappingException($"None of the properties on {argumentValue.GetType().Name} could be matched to columns in {m_Name}.");

		ApplyRules(dataSource.AuditRules, operationType, null, dataSource.UserValue);
	}

	/// <summary>
	/// Overrides the list of keys.
	/// </summary>
	/// <param name="keyColumns">The key columns in SqlName format. The column names should be normalized before calling this method.</param>
	public void OverrideKeys(ICollection<string> keyColumns)
	{
		if (keyColumns == null || keyColumns.Count == 0)
			throw new ArgumentException($"{nameof(keyColumns)} is null or empty.", nameof(keyColumns));

		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];
			entry.IsKey = keyColumns.Contains(entry.Details.SqlName ?? "");
			entry.UseForUpdate = entry.UseForUpdate && !entry.IsKey; //If are using the column as a key, we shouldn't update it
		}
	}

	/// <summary>
	/// Returns true is the primary key includes an identity column.
	/// </summary>
	/// <typeparam name="TParameter">The type of the parameter.</typeparam>
	/// <param name="parameterBuilder">The parameter builder.</param>
	/// <param name="keyParameters">The key parameters.</param>
	/// <returns></returns>
	public bool PrimaryKeyIsIdentity<TParameter>(Func<TDbType?, TParameter> parameterBuilder, out List<TParameter> keyParameters)
		 where TParameter : DbParameter
	{
		if (parameterBuilder == null)
			throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} is null.");

		var primKeyIsIdent = false;
		keyParameters = [];
		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			if (entry.IsKey && entry.Details.IsIdentity)
			{
				primKeyIsIdent = true;

				var item = parameterBuilder(entry.Details.DbType);
				item.ParameterName = entry.Details.SqlVariableName;
				item.Value = entry.ParameterValue;
				keyParameters.Add(item);
			}
		}

		return primKeyIsIdent;
	}

	/// <summary>
	/// Clones this instance so that you can modify it without affecting the cached original.
	/// </summary>
	/// <returns></returns>
	internal SqlBuilder<TDbType> Clone(bool strictMode)
	{
		return new SqlBuilder<TDbType>(m_Name, m_Entries, strictMode);
	}

	/// <summary>
	/// Applies an argument dictionary, overriding any previously applied values.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <exception cref="MappingException">This is thrown is no keys could be matched to a column. If strict mode, all keys must match columns.</exception>
	void ApplyArgumentDictionary(IReadOnlyDictionary<string, object> value)
	{
		if (value == null || value.Count == 0)
			throw new ArgumentException($"{nameof(value)} is null or empty.", nameof(value));

		var found = false;

		foreach (var item in value)
		{
			var keyFound = false;
			for (var i = 0; i < m_Entries.Length; i++)
			{
				ref var entry = ref m_Entries[i];

				if (string.Equals(entry.Details.ClrName, item.Key, StringComparison.OrdinalIgnoreCase)
					|| string.Equals(entry.Details.SqlName, item.Key, StringComparison.OrdinalIgnoreCase)
					|| string.Equals(entry.Details.SqlVariableName, item.Key, StringComparison.OrdinalIgnoreCase)
					)
				{
					if (entry.IsFormalParameter)
						entry.UseParameter = true;
					entry.ParameterValue = item.Value ?? DBNull.Value;
					found = true;
					keyFound = true;
					//break; In the case of TVFs, the same column may appear twice
				}
			}
			if (StrictMode && !keyFound)
				throw new MappingException($"Strict mode was enabled, but property {item.Key} could be matched to a column in {m_Name}. Disable strict mode or remove the item from the dictionary.");
		}

		if (!found)
			throw new MappingException($"None of the keys could be matched to columns or parameters in {m_Name}.");
	}

	/// <summary>
	/// Applies the argument value.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="operationType">The operation type.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="useObjectDefinedKeys">if set to <c>true</c> use object defined keys.</param>
	/// <param name="changedPropertiesOnly">if set to <c>true</c> filter the update list according to IPropertyChangeTracking.ChangedProperties.</param>
	/// <param name="nonQueryDelete">No result will be read from this operation.</param>
	/// <exception cref="MappingException">This is thrown is no properties could be matched to a column. If strict mode, all properties must match columns.</exception>
	/// <exception cref="ArgumentNullException">value;value is null.</exception>
	/// <remarks>
	/// If the object implements IReadOnlyDictionary[string, object], ApplyArgumentDictionary will be implicitly called instead.
	/// If the object does not implement IPropertyChangeTracking and changedPropertiesOnly is set, an error will occur.
	/// </remarks>
	void ApplyArgumentValue(IDataSource dataSource, OperationTypes operationType, object? argumentValue, bool useObjectDefinedKeys, bool changedPropertiesOnly, bool nonQueryDelete)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		if (argumentValue == null)
		{
			//only apply audit rules
		}
		else if (argumentValue is IReadOnlyDictionary<string, object> readOnlyDictionary)
		{
			ApplyArgumentDictionary(readOnlyDictionary);
		}
		else
		{
			IReadOnlyList<string> changedProperties = Array.Empty<string>();
			if (changedPropertiesOnly)
			{
				if (argumentValue is IPropertyChangeTracking propertyChangeTracking)
				{
					changedProperties = propertyChangeTracking.ChangedProperties();
					if (changedProperties.Count == 0)
						throw new ArgumentException($"Changed properties were requested, but no properties were marked as changed.");
				}
				else
					throw new ArgumentException($"Changed properties were requested, but {argumentValue.GetType().Name} does not implement IPropertyChangeTracking.");
			}

			if (useObjectDefinedKeys)
				for (var i = 0; i < m_Entries.Length; i++)
					m_Entries[i].IsKey = false;

			var found = false; //Any matching properties were found in m_Entries

			var metadata = MetadataCache.GetMetadata(argumentValue.GetType());
			foreach (var property in metadata.Properties)
			{
				var propertyFound = false; //Property exists at all in m_Entries

				var mappedColumnName = property.MappedColumnName;

				//Ignore unmapped columns
				if (mappedColumnName == null)
					continue;

				//Ignore properties we can't read. (It's probably a protected property, not meant for data binding.)
				if (!property.CanRead)
					continue;

				for (var i = 0; i < m_Entries.Length; i++)
				{
					var isMatch = false; //property is match for m_Entries[i]
					ref var entry = ref m_Entries[i];

					if (string.Equals(entry.Details.SqlName, mappedColumnName, StringComparison.OrdinalIgnoreCase))
						isMatch = true;
					else if (entry.Details.ClrName.Equals(mappedColumnName, StringComparison.OrdinalIgnoreCase))
					{
						isMatch = true;
						entry.UseClrNameAsAlias = true;
					}
					else if (string.Equals(entry.Details.QuotedSqlName, mappedColumnName, StringComparison.OrdinalIgnoreCase))
					{
						throw new MappingException($"Modify the ColumnAttribute for the desired column \"{mappedColumnName}\" to be \"{entry.Details.SqlName}\". SQL Quoted column names are not supported.");
					}

					if (isMatch)
					{
						propertyFound = true;
						found = true;

						if (useObjectDefinedKeys && property.IsKey)
							entry.IsKey = true;

						entry.ParameterValue = property.InvokeGet(argumentValue) ?? DBNull.Value;

						if (property.IgnoreOnInsert)
							entry.UseForInsert = false;

						if (property.IgnoreOnUpdate)
							entry.UseForUpdate = false;

						if (changedPropertiesOnly && !changedProperties.Contains(property.Name))
							entry.UseForUpdate = false;

						if (entry.IsFormalParameter)
							entry.UseParameter = true;

						//break; In the case of TVFs, the same column may appear twice
					}
				}
				if (StrictMode && !propertyFound && !nonQueryDelete)
				{
					//When in strict mode and a property wasn't found, throw an exception. This is to warn about possible data loss.
					//Exception: If performing a non-query delete, we don't care about the unmatched properties.
					//This can happen when using the TableAndView attribute and some columns are only on the view.
					throw new MappingException($"Strict mode was enabled, but property {property.Name} could be matched to a column in {m_Name}. Disable strict mode or mark the property as NotMapped.");
				}
			}

			if (!found)
				throw new MappingException($"None of the properties on {argumentValue.GetType().Name} could be matched to columns in {m_Name}.");
		}

		ApplyRules(dataSource.AuditRules, operationType, argumentValue, dataSource.UserValue);
	}

	/// <summary>
	/// Applies the indicated rules.
	/// </summary>
	/// <param name="rules">The rules.</param>
	/// <param name="operationType">The type of operation.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="userValue">The user value.</param>
	void ApplyRules(AuditRuleCollection rules, OperationTypes operationType, object? argumentValue, object? userValue)
	{
		if (argumentValue != null)
			rules.CheckValidation(argumentValue);

		for (var i = 0; i < m_Entries.Length; i++)
		{
			ref var entry = ref m_Entries[i];

			foreach (var rule in rules.GetRulesForColumn(entry.Details.SqlName, entry.Details.ClrName, operationType))
			{
				entry.ParameterValue = rule.GenerateValue(argumentValue, userValue, entry.ParameterValue);
				entry.ParameterColumn = null; //replaces the TVP columns

				if (rule.AppliesWhen.HasFlag(OperationTypes.Insert))
					entry.UseForInsert = true;

				//Update is used for soft deletes
				if (rule.AppliesWhen.HasFlag(OperationTypes.Update) || rule.AppliesWhen.HasFlag(OperationTypes.Delete))
					entry.UseForUpdate = true;
			}

			foreach (var rule in rules.GetRestrictionsForColumn(m_Name, entry.Details.SqlName, entry.Details.ClrName))
			{
				var ignoreRule = rule.ExceptWhen(userValue);
				if (ignoreRule)
					continue;

				if (rule.AppliesWhen.HasFlag(OperationTypes.Insert))
					entry.RestrictedInsert = true;

				if (rule.AppliesWhen.HasFlag(OperationTypes.Update))
					entry.RestrictedUpdate = true;

				if (rule.AppliesWhen.HasFlag(OperationTypes.Select))
					entry.RestrictedRead = true;
			}
		}
	}

	int? FindColumnIndexByName(string columnName)
	{
		for (var i = 0; i < m_Entries.Length; i++)
		{
			var details = m_Entries[i].Details;
			if (string.Equals(details.SqlName, columnName, StringComparison.OrdinalIgnoreCase) || details.ClrName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
			{
				return i;
			}
		}
		return null;
	}
}
