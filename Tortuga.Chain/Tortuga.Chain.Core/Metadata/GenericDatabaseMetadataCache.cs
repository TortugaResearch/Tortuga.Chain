using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Aggregation;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// The GenericDatabaseMetadataCache cannot actually return metadata. It is only used for handling materializer type conversions.
/// Implements the <see cref="Tortuga.Chain.Metadata.IDatabaseMetadataCache" />
/// </summary>
/// <seealso cref="Tortuga.Chain.Metadata.IDatabaseMetadataCache" />
sealed class GenericDatabaseMetadataCache : IDatabaseMetadataCache
{
	const string NotSupportedMessage = "This data source does not expose database metadata.";

	/// <summary>
	/// Gets the converter dictionary used by materializers.
	/// </summary>
	/// <value>The converter dictionary.</value>
	public MaterializerTypeConverter Converter { get; } = new();

	int? IDatabaseMetadataCache.MaxParameters => null;

	string IDatabaseMetadataCache.GetAggregationFunction(AggregationType aggregationType, string columnName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	StoredProcedureMetadata IDatabaseMetadataCache.GetStoredProcedure(string procedureName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	IReadOnlyCollection<StoredProcedureMetadata> IDatabaseMetadataCache.GetStoredProcedures()
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	TableFunctionMetadata IDatabaseMetadataCache.GetTableFunction(string tableFunctionName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	IReadOnlyCollection<TableFunctionMetadata> IDatabaseMetadataCache.GetTableFunctions()
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	TableOrViewMetadata IDatabaseMetadataCache.GetTableOrView(string tableName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	TableOrViewMetadata IDatabaseMetadataCache.GetTableOrViewFromClass<TObject>(OperationType operation)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	TableOrViewMetadata IDatabaseMetadataCache.GetTableOrViewFromClass(Type type, OperationType operation)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	IReadOnlyCollection<TableOrViewMetadata> IDatabaseMetadataCache.GetTablesAndViews()
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	UserDefinedTableTypeMetadata IDatabaseMetadataCache.GetUserDefinedTableType(string typeName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	IReadOnlyCollection<UserDefinedTableTypeMetadata> IDatabaseMetadataCache.GetUserDefinedTableTypes()
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	void IDatabaseMetadataCache.Preload()
	{
	}

	string IDatabaseMetadataCache.QuoteColumnName(string columnName)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	void IDatabaseMetadataCache.Reset()
	{
	}

	bool IDatabaseMetadataCache.TryGetStoredProcedure(string procedureName, [NotNullWhen(true)] out StoredProcedureMetadata? storedProcedure)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	bool IDatabaseMetadataCache.TryGetTableFunction(string tableFunctionName, [NotNullWhen(true)] out TableFunctionMetadata? tableFunction)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	bool IDatabaseMetadataCache.TryGetTableOrView(string tableName, [NotNullWhen(true)] out TableOrViewMetadata? tableOrView)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}

	bool IDatabaseMetadataCache.TryGetUserDefinedTableType(string typeName, [NotNullWhen(true)] out UserDefinedTableTypeMetadata? userDefinedTableType)
	{
		throw new NotImplementedException(NotSupportedMessage);
	}
}
