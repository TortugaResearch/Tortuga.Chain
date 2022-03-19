﻿//This file was generated by Tortuga Shipwright

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase: Tortuga.Chain.DataSources.ISupportsDeleteAll, Tortuga.Chain.DataSources.ISupportsDeleteByKeyList, Tortuga.Chain.DataSources.ISupportsDeleteByKey, Tortuga.Chain.DataSources.ISupportsUpdate, Tortuga.Chain.DataSources.ISupportsDelete, Tortuga.Chain.DataSources.ISupportsSqlQueries, Tortuga.Chain.DataSources.ISupportsUpdateByKey, Tortuga.Chain.DataSources.ISupportsUpdateByKeyList, Tortuga.Chain.DataSources.ISupportsInsert, Traits.ICommandHelper<Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>, Traits.IUpdateDeleteByKeyHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>, Traits.IUpdateDeleteHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>, Traits.IInsertHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>
	{

		private bool __TraitsRegistered;

		// These fields and/or properties hold the traits. They should not be referenced directly.
		private Traits.SupportsDeleteAllTrait<Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait0 = new();
		private Traits.SupportsDeleteAllTrait<Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait0
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait0;
			}
		}
		private Traits.SupportsDeleteByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait1 = new();
		private Traits.SupportsDeleteByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait1
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait1;
			}
		}
		private Traits.SupportsUpdateTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait2 = new();
		private Traits.SupportsUpdateTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait2
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait2;
			}
		}
		private Traits.SupportsDeleteTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait3 = new();
		private Traits.SupportsDeleteTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait3
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait3;
			}
		}
		private Traits.SupportsSqlQueriesTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> ___Trait4 = new();
		private Traits.SupportsSqlQueriesTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> __Trait4
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait4;
			}
		}
		private Traits.SupportsUpdateByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait5 = new();
		private Traits.SupportsUpdateByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait5
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait5;
			}
		}
		private Traits.SupportsInsertTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> ___Trait6 = new();
		private Traits.SupportsInsertTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType> __Trait6
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait6;
			}
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsDelete
		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsDelete.Delete<TArgument>(System.String tableName, TArgument argumentValue, Tortuga.Chain.DeleteOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDelete)__Trait3).Delete<TArgument>(tableName, argumentValue, options);
		}

		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsDelete.Delete<TArgument>(TArgument argumentValue, Tortuga.Chain.DeleteOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDelete)__Trait3).Delete<TArgument>(argumentValue, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsDeleteAll
		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsDeleteAll.DeleteAll(System.String tableName)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteAll)__Trait0).DeleteAll(tableName);
		}

		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsDeleteAll.DeleteAll<TObject>()
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteAll)__Trait0).DeleteAll<TObject>();
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsDeleteByKey
		Tortuga.Chain.CommandBuilders.ISingleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsDeleteByKey.DeleteByKey<TKey>(System.String tableName, TKey key, Tortuga.Chain.DeleteOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteByKey)__Trait1).DeleteByKey<TKey>(tableName, key, options);
		}

		Tortuga.Chain.CommandBuilders.ISingleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsDeleteByKey.DeleteByKey(System.String tableName, System.String key, Tortuga.Chain.DeleteOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteByKey)__Trait1).DeleteByKey(tableName, key, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsDeleteByKeyList
		Tortuga.Chain.CommandBuilders.IMultipleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsDeleteByKeyList.DeleteByKeyList<TKey>(System.String tableName, System.Collections.Generic.IEnumerable<TKey> keys, Tortuga.Chain.DeleteOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteByKeyList)__Trait1).DeleteByKeyList<TKey>(tableName, keys, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsInsert
		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsInsert.Insert<TArgument>(System.String tableName, TArgument argumentValue, Tortuga.Chain.InsertOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsInsert)__Trait6).Insert<TArgument>(tableName, argumentValue, options);
		}

		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsInsert.Insert<TArgument>(TArgument argumentValue, Tortuga.Chain.InsertOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsInsert)__Trait6).Insert<TArgument>(argumentValue, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsSqlQueries
		Tortuga.Chain.CommandBuilders.IMultipleTableDbCommandBuilder Tortuga.Chain.DataSources.ISupportsSqlQueries.Sql(System.String sqlStatement, System.Object argumentValue)
		{
			return ((Tortuga.Chain.DataSources.ISupportsSqlQueries)__Trait4).Sql(sqlStatement, argumentValue);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsUpdate
		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsUpdate.Update<TArgument>(System.String tableName, TArgument argumentValue, Tortuga.Chain.UpdateOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsUpdate)__Trait2).Update<TArgument>(tableName, argumentValue, options);
		}

		Tortuga.Chain.CommandBuilders.IObjectDbCommandBuilder<TArgument> Tortuga.Chain.DataSources.ISupportsUpdate.Update<TArgument>(TArgument argumentValue, Tortuga.Chain.UpdateOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsUpdate)__Trait2).Update<TArgument>(argumentValue, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsUpdateByKey
		Tortuga.Chain.CommandBuilders.ISingleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsUpdateByKey.UpdateByKey<TArgument, TKey>(System.String tableName, TArgument newValues, TKey key, Tortuga.Chain.UpdateOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsUpdateByKey)__Trait5).UpdateByKey<TArgument, TKey>(tableName, newValues, key, options);
		}

		Tortuga.Chain.CommandBuilders.ISingleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsUpdateByKey.UpdateByKey<TArgument>(System.String tableName, TArgument newValues, System.String key, Tortuga.Chain.UpdateOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsUpdateByKey)__Trait5).UpdateByKey<TArgument>(tableName, newValues, key, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsUpdateByKeyList
		Tortuga.Chain.CommandBuilders.IMultipleRowDbCommandBuilder Tortuga.Chain.DataSources.ISupportsUpdateByKeyList.UpdateByKeyList<TArgument, TKey>(System.String tableName, TArgument newValues, System.Collections.Generic.IEnumerable<TKey> keys, Tortuga.Chain.UpdateOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsUpdateByKeyList)__Trait5).UpdateByKeyList<TArgument, TKey>(tableName, newValues, keys, options);
		}

		// Exposing trait Traits.SupportsDeleteAllTrait<Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		/// <summary>Deletes all records in the specified table.</summary>
		/// <param name="tableName">Name of the table to clear.</param>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public Tortuga.Chain.ILink<int?> DeleteAll(Tortuga.Chain.Access.AccessObjectName tableName)
		{
			return __Trait0.DeleteAll(tableName);
		}

		/// <summary>Deletes all records in the specified table.</summary>
		/// <typeparam name="TObject">This class used to determine which table to clear</typeparam>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public Tortuga.Chain.ILink<int?> DeleteAll<TObject>()where TObject : class
		{
			return __Trait0.DeleteAll<TObject>();
		}

		// Exposing trait Traits.SupportsDeleteByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;TCommand, TParameter&gt;.</returns>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey<T>(Tortuga.Chain.Access.AccessObjectName tableName, T key, Tortuga.Chain.DeleteOptions options = 0)where T : struct
		{
			return __Trait1.DeleteByKey<T>(tableName, key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey(Tortuga.Chain.Access.AccessObjectName tableName, System.String key, Tortuga.Chain.DeleteOptions options = 0)
		{
			return __Trait1.DeleteByKey(tableName, key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey<TObject>(System.Guid key, Tortuga.Chain.DeleteOptions options = 0)where TObject : class
		{
			return __Trait1.DeleteByKey<TObject>(key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey<TObject>(System.Int64 key, Tortuga.Chain.DeleteOptions options = 0)where TObject : class
		{
			return __Trait1.DeleteByKey<TObject>(key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey<TObject>(System.Int32 key, Tortuga.Chain.DeleteOptions options = 0)where TObject : class
		{
			return __Trait1.DeleteByKey<TObject>(key, options);
		}

		/// <summary>
		/// Delete a record by its primary key.
		/// </summary>
		/// <typeparam name="TObject">Used to determine the table name</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKey<TObject>(System.String key, Tortuga.Chain.DeleteOptions options = 0)where TObject : class
		{
			return __Trait1.DeleteByKey<TObject>(key, options);
		}

		/// <summary>
		/// Delete multiple rows by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Delete options.</param>
		/// <exception cref="T:Tortuga.Chain.MappingException"></exception>
		public Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> DeleteByKeyList<TKey>(Tortuga.Chain.Access.AccessObjectName tableName, System.Collections.Generic.IEnumerable<TKey> keys, Tortuga.Chain.DeleteOptions options = 0)
		{
			return __Trait1.DeleteByKeyList<TKey>(tableName, keys, options);
		}

		// Exposing trait Traits.SupportsDeleteTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		/// <summary>
		/// Creates a command to perform a delete operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.ObjectDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, TArgument> Delete<TArgument>(Tortuga.Chain.Access.AccessObjectName tableName, TArgument argumentValue, Tortuga.Chain.DeleteOptions options = 0)where TArgument : class
		{
			return __Trait3.Delete<TArgument>(tableName, argumentValue, options);
		}

		/// <summary>
		/// Delete an object model from the table indicated by the class's Table attribute.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The delete options.</param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.ObjectDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, TArgument> Delete<TArgument>(TArgument argumentValue, Tortuga.Chain.DeleteOptions options = 0)where TArgument : class
		{
			return __Trait3.Delete<TArgument>(argumentValue, options);
		}

		// Exposing trait Traits.SupportsInsertTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		// Exposing trait Traits.SupportsSqlQueriesTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter>

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> Sql(System.String sqlStatement)
		{
			return __Trait4.Sql(sqlStatement);
		}

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <returns>SqlServerSqlCall.</returns>
		public Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> Sql(System.String sqlStatement, System.Object argumentValue)
		{
			return __Trait4.Sql(sqlStatement, argumentValue);
		}

		// Exposing trait Traits.SupportsUpdateByKeyListTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		/// <summary>
		/// Update a record by its primary key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;AbstractCommand, AbstractParameter&gt;.</returns>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> UpdateByKey<TArgument, TKey>(Tortuga.Chain.Access.AccessObjectName tableName, TArgument newValues, TKey key, Tortuga.Chain.UpdateOptions options = 0)where TKey : struct
		{
			return __Trait5.UpdateByKey<TArgument, TKey>(tableName, newValues, key, options);
		}

		/// <summary>
		/// Update a record by its primary key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
		public Tortuga.Chain.CommandBuilders.SingleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> UpdateByKey<TArgument>(Tortuga.Chain.Access.AccessObjectName tableName, TArgument newValues, System.String key, Tortuga.Chain.UpdateOptions options = 0)
		{
			return __Trait5.UpdateByKey<TArgument>(tableName, newValues, key, options);
		}

		/// <summary>
		/// Update multiple rows by key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
		/// <exception cref="T:Tortuga.Chain.MappingException"></exception>
		public Tortuga.Chain.CommandBuilders.MultipleRowDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> UpdateByKeyList<TArgument, TKey>(Tortuga.Chain.Access.AccessObjectName tableName, TArgument newValues, System.Collections.Generic.IEnumerable<TKey> keys, Tortuga.Chain.UpdateOptions options = 0)
		{
			return __Trait5.UpdateByKeyList<TArgument, TKey>(tableName, newValues, keys, options);
		}

		// Exposing trait Traits.SupportsUpdateTrait<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>

		/// <summary>
		/// Update an object in the specified table.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The update options.</param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.ObjectDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, TArgument> Update<TArgument>(TArgument argumentValue, Tortuga.Chain.UpdateOptions options = 0)where TArgument : class
		{
			return __Trait2.Update<TArgument>(argumentValue, options);
		}

		/// <summary>
		/// Update an object in the specified table.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.ObjectDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, TArgument> Update<TArgument>(Tortuga.Chain.Access.AccessObjectName tableName, TArgument argumentValue, Tortuga.Chain.UpdateOptions options = 0)where TArgument : class
		{
			return __Trait2.Update<TArgument>(tableName, argumentValue, options);
		}

		private partial Tortuga.Chain.ILink<int?> OnDeleteAll(Tortuga.Chain.Access.AccessObjectName tableName );

		private partial Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter> OnSql(System.String sqlStatement, System.Object? argumentValue );

		private void __RegisterTraits()
		{
			__TraitsRegistered = true;
			__Trait0.OnDeleteAll = OnDeleteAll;
			__Trait0.DataSource = this as Traits.ICommandHelper<Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
			__Trait1.DataSource = this as Traits.IUpdateDeleteByKeyHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
			__Trait2.DataSource = this as Traits.IUpdateDeleteHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
			__Trait3.DataSource = this as Traits.IUpdateDeleteHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
			__Trait4.OnSql = OnSql;
			__Trait5.DataSource = this as Traits.IUpdateDeleteByKeyHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
			__Trait6.DataSource = this as Traits.IInsertHelper<System.Data.OleDb.OleDbCommand, System.Data.OleDb.OleDbParameter, Tortuga.Chain.Access.AccessObjectName, System.Data.OleDb.OleDbType>;
		}

	}
}
