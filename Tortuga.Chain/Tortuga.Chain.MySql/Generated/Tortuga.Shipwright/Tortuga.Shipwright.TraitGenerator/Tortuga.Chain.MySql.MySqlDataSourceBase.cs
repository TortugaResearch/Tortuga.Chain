﻿//This file was generated by Tortuga Shipwright

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase: Tortuga.Chain.DataSources.ISupportsDeleteAll, Tortuga.Chain.DataSources.ISupportsTruncate, Tortuga.Chain.DataSources.ISupportsSqlQueries, Tortuga.Chain.DataSources.ISupportsInsertBatch, Traits.ICommandHelper<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType>, Traits.IInsertBatchHelper<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter, Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType>
	{

		private bool __TraitsRegistered;

		// These fields and/or properties hold the traits. They should not be referenced directly.
		private Traits.SupportsDeleteAllTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType> ___Trait0 = new();
		private Traits.SupportsDeleteAllTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType> __Trait0
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait0;
			}
		}
		private Traits.SupportsTruncateTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType> ___Trait1 = new();
		private Traits.SupportsTruncateTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType> __Trait1
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait1;
			}
		}
		private Traits.SupportsSqlQueriesTrait<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> ___Trait2 = new();
		private Traits.SupportsSqlQueriesTrait<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> __Trait2
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait2;
			}
		}
		private Traits.SupportsInsertBatchTrait<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter, Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType, Tortuga.Chain.CommandBuilders.DbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter>> __Trait3 = new();

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsDeleteAll
		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsDeleteAll.DeleteAll(System.String tableName)
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteAll)__Trait0).DeleteAll(tableName);
		}

		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsDeleteAll.DeleteAll<TObject>()
		{
			return ((Tortuga.Chain.DataSources.ISupportsDeleteAll)__Trait0).DeleteAll<TObject>();
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsInsertBatch
		Tortuga.Chain.CommandBuilders.IDbCommandBuilder Tortuga.Chain.DataSources.ISupportsInsertBatch.InsertBatch<TObject>(System.Collections.Generic.IEnumerable<TObject> objects, Tortuga.Chain.InsertOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsInsertBatch)__Trait3).InsertBatch<TObject>(objects, options);
		}

		Tortuga.Chain.ILink<int> Tortuga.Chain.DataSources.ISupportsInsertBatch.InsertMultipleBatch<TObject>(System.String tableName, System.Collections.Generic.IReadOnlyList<TObject> objects, Tortuga.Chain.InsertOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsInsertBatch)__Trait3).InsertMultipleBatch<TObject>(tableName, objects, options);
		}

		Tortuga.Chain.ILink<int> Tortuga.Chain.DataSources.ISupportsInsertBatch.InsertMultipleBatch<TObject>(System.Collections.Generic.IReadOnlyList<TObject> objects, Tortuga.Chain.InsertOptions options)
		{
			return ((Tortuga.Chain.DataSources.ISupportsInsertBatch)__Trait3).InsertMultipleBatch<TObject>(objects, options);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsSqlQueries
		Tortuga.Chain.CommandBuilders.IMultipleTableDbCommandBuilder Tortuga.Chain.DataSources.ISupportsSqlQueries.Sql(System.String sqlStatement, System.Object argumentValue)
		{
			return ((Tortuga.Chain.DataSources.ISupportsSqlQueries)__Trait2).Sql(sqlStatement, argumentValue);
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.ISupportsTruncate
		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsTruncate.Truncate(System.String tableName)
		{
			return ((Tortuga.Chain.DataSources.ISupportsTruncate)__Trait1).Truncate(tableName);
		}

		Tortuga.Chain.ILink<int?> Tortuga.Chain.DataSources.ISupportsTruncate.Truncate<TObject>()
		{
			return ((Tortuga.Chain.DataSources.ISupportsTruncate)__Trait1).Truncate<TObject>();
		}

		// Exposing trait Traits.SupportsDeleteAllTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType>

		/// <summary>Deletes all records in the specified table.</summary>
		/// <param name="tableName">Name of the table to clear.</param>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public Tortuga.Chain.ILink<int?> DeleteAll(Tortuga.Chain.MySql.MySqlObjectName tableName)
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

		// Exposing trait Traits.SupportsInsertBatchTrait<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter, Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType, Tortuga.Chain.CommandBuilders.DbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter>>

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public Tortuga.Chain.CommandBuilders.DbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> InsertBatch<TObject>(Tortuga.Chain.MySql.MySqlObjectName tableName, System.Collections.Generic.IEnumerable<TObject> objects, Tortuga.Chain.InsertOptions options = 0)where TObject : class
		{
			return __Trait3.InsertBatch<TObject>(tableName, objects, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public Tortuga.Chain.CommandBuilders.DbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> InsertBatch<TObject>(System.Collections.Generic.IEnumerable<TObject> objects, Tortuga.Chain.InsertOptions options = 0)where TObject : class
		{
			return __Trait3.InsertBatch<TObject>(objects, options);
		}

		/// <summary>
		/// Performs a series of batch inserts.
		/// </summary>
		/// <typeparam name="TObject">The type of the t object.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>Tortuga.Chain.ILink&lt;System.Int32&gt;.</returns>
		/// <remarks>This operation is not atomic. It should be wrapped in a transaction.</remarks>
		public Tortuga.Chain.ILink<int> InsertMultipleBatch<TObject>(Tortuga.Chain.MySql.MySqlObjectName tableName, System.Collections.Generic.IEnumerable<TObject> objects, Tortuga.Chain.InsertOptions options = 0)where TObject : class
		{
			return __Trait3.InsertMultipleBatch<TObject>(tableName, objects, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="objects">The objects to insert.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public Tortuga.Chain.ILink<int> InsertMultipleBatch<TObject>(System.Collections.Generic.IReadOnlyList<TObject> objects, Tortuga.Chain.InsertOptions options = 0)where TObject : class
		{
			return __Trait3.InsertMultipleBatch<TObject>(objects, options);
		}

		// Exposing trait Traits.SupportsSqlQueriesTrait<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter>

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <returns></returns>
		public Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> Sql(System.String sqlStatement)
		{
			return __Trait2.Sql(sqlStatement);
		}

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <returns>SqlServerSqlCall.</returns>
		public Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> Sql(System.String sqlStatement, System.Object argumentValue)
		{
			return __Trait2.Sql(sqlStatement, argumentValue);
		}

		// Exposing trait Traits.SupportsTruncateTrait<Tortuga.Chain.MySql.MySqlObjectName, MySqlConnector.MySqlDbType>

		/// <summary>Truncates the specified table.</summary>
		/// <param name="tableName">Name of the table to Truncate.</param>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public Tortuga.Chain.ILink<int?> Truncate(Tortuga.Chain.MySql.MySqlObjectName tableName)
		{
			return __Trait1.Truncate(tableName);
		}

		/// <summary>Truncates the specified table.</summary>
		/// <typeparam name="TObject">This class used to determine which table to Truncate</typeparam>
		/// <returns>The number of rows deleted or null if the database doesn't provide that information.</returns>
		public Tortuga.Chain.ILink<int?> Truncate<TObject>()where TObject : class
		{
			return __Trait1.Truncate<TObject>();
		}

		private partial Tortuga.Chain.ILink<int?> OnDeleteAll(Tortuga.Chain.MySql.MySqlObjectName tableName );

		private partial Tortuga.Chain.CommandBuilders.MultipleTableDbCommandBuilder<MySqlConnector.MySqlCommand, MySqlConnector.MySqlParameter> OnSql(System.String sqlStatement, System.Object? argumentValue );

		private partial Tortuga.Chain.ILink<int?> OnTruncate(Tortuga.Chain.MySql.MySqlObjectName tableName );


		private void __RegisterTraits()
		{
			__TraitsRegistered = true;
			__Trait0.OnDeleteAll = OnDeleteAll;
			__Trait0.DataSource = this;
			__Trait1.OnTruncate = OnTruncate;
			__Trait1.DataSource = this;
			__Trait2.OnSql = OnSql;
			__Trait3.DataSource = this;
		}
	}
}
