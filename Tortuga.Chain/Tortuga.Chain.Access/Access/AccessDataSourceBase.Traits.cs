using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.Access
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsDeleteByKeyList<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	partial class AccessDataSourceBase //: ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>
	{
		DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

		List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

		/// <summary>
		/// Delete multiple rows by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
		MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IDeleteByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteByKeyList<TKey>(AccessObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count != 1)
				throw new MappingException($"DeleteByKey/DeleteByKeyList operation isn't allowed on {tableName} because it doesn't have a single primary key.");

			var keyList = keys.AsList();
			var columnMetadata = primaryKeys.Single();
			string where;
			if (keys.Count() > 1)
				where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
			else
				where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<OleDbParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new OleDbParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.OleDbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return new AccessDeleteMany(this, tableName, where, parameters, parameters.Count, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

			if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
				effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions |= UpdateOptions.UseKeyAttribute;

			return new AccessUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
		}

		AbstractObjectName ICommandHelper<AbstractObjectName, AbstractDbType>.ParseObjectName(string objectName) => new(objectName);

		private partial ILink<int?> OnDeleteAll(AccessObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}

		private partial MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new AccessSqlCall(this, sqlStatement, argumentValue);
		}
	}
}

