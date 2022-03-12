﻿//This file was generated by Tortuga Shipwright

namespace Tortuga.Chain
{
	partial class SqlServerDataSource: Tortuga.Chain.DataSources.IRootDataSource
	{

		private bool __TraitsRegistered;

		// These fields and/or properties hold the traits. They should not be referenced directly.
		private Traits.RootDataSourceTrait<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource, Tortuga.Chain.SqlServer.SqlServerOpenDataSource, Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient.SqlTransaction> ___Trait0 = new();
		private Traits.RootDataSourceTrait<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource, Tortuga.Chain.SqlServer.SqlServerOpenDataSource, Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient.SqlTransaction> __Trait0
		{
			get
			{
				if (!__TraitsRegistered) __RegisterTraits();
				return ___Trait0;
			}
		}

		// Explicit interface implementation Tortuga.Chain.DataSources.IRootDataSource
		Tortuga.Chain.DataSources.ITransactionalDataSource Tortuga.Chain.DataSources.IRootDataSource.BeginTransaction()
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).BeginTransaction();
		}

		System.Threading.Tasks.Task<Tortuga.Chain.DataSources.ITransactionalDataSource> Tortuga.Chain.DataSources.IRootDataSource.BeginTransactionAsync()
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).BeginTransactionAsync();
		}

		System.Data.Common.DbConnection Tortuga.Chain.DataSources.IRootDataSource.CreateConnection()
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).CreateConnection();
		}

		System.Threading.Tasks.Task<System.Data.Common.DbConnection> Tortuga.Chain.DataSources.IRootDataSource.CreateConnectionAsync()
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).CreateConnectionAsync();
		}

		Tortuga.Chain.DataSources.IOpenDataSource Tortuga.Chain.DataSources.IRootDataSource.CreateOpenDataSource()
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).CreateOpenDataSource();
		}

		Tortuga.Chain.DataSources.IOpenDataSource Tortuga.Chain.DataSources.IRootDataSource.CreateOpenDataSource(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction? transaction)
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).CreateOpenDataSource(connection, transaction);
		}

		Tortuga.Chain.DataSources.IOpenDataSource Tortuga.Chain.DataSources.IRootDataSource.CreateOpenDataSource(System.Data.IDbConnection connection, System.Data.IDbTransaction? transaction)
		{
			return ((Tortuga.Chain.DataSources.IRootDataSource)__Trait0).CreateOpenDataSource(connection, transaction);
		}

		// Exposing trait Traits.RootDataSourceTrait<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource, Tortuga.Chain.SqlServer.SqlServerOpenDataSource, Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient.SqlTransaction>

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
		public  Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource BeginTransaction()
		{
			return __Trait0.BeginTransaction();
		}

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <param name="forwardEvents"></param>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
		public  Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource BeginTransaction(System.Nullable<System.Data.IsolationLevel> isolationLevel = default, System.Boolean forwardEvents = true)
		{
			return __Trait0.BeginTransaction(isolationLevel, forwardEvents);
		}

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <param name="forwardEvents"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
		public  System.Threading.Tasks.Task<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource> BeginTransactionAsync(System.Nullable<System.Data.IsolationLevel> isolationLevel = default, System.Boolean forwardEvents = true, System.Threading.CancellationToken cancellationToken = default)
		{
			return __Trait0.BeginTransactionAsync(isolationLevel, forwardEvents, cancellationToken);
		}

		/// <summary>
		/// Creates a new transaction.
		/// </summary>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
		public  System.Threading.Tasks.Task<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource> BeginTransactionAsync()
		{
			return __Trait0.BeginTransactionAsync();
		}

		/// <summary>
		/// Creates and opens a new Access connection
		/// </summary>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the connection.</remarks>
		public  Microsoft.Data.SqlClient.SqlConnection CreateConnection()
		{
			return __Trait0.CreateConnection();
		}

		/// <summary>
		/// Creates the connection asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		/// <remarks>
		/// The caller of this method is responsible for closing the connection.
		/// </remarks>
		public  System.Threading.Tasks.Task<Microsoft.Data.SqlClient.SqlConnection> CreateConnectionAsync(System.Threading.CancellationToken cancellationToken = default)
		{
			return __Trait0.CreateConnectionAsync(cancellationToken);
		}

		/// <summary>
		/// Creates an open data source using the supplied connection and optional transaction.
		/// </summary>
		/// <param name="connection">The connection to wrap.</param>
		/// <param name="transaction">The transaction to wrap.</param>
		/// <returns>IOpenDataSource.</returns>
		/// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
		public  Tortuga.Chain.SqlServer.SqlServerOpenDataSource CreateOpenDataSource(Microsoft.Data.SqlClient.SqlConnection connection, Microsoft.Data.SqlClient.SqlTransaction? transaction = default)
		{
			return __Trait0.CreateOpenDataSource(connection, transaction);
		}

		/// <summary>
		/// Creates an open data source with a new connection.
		/// </summary>
		/// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
		public  Tortuga.Chain.SqlServer.SqlServerOpenDataSource CreateOpenDataSource()
		{
			return __Trait0.CreateOpenDataSource();
		}

		private partial Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource OnBeginTransaction(System.Nullable<System.Data.IsolationLevel> isolationLevel, System.Boolean forwardEvents );

		private partial System.Threading.Tasks.Task<Tortuga.Chain.SqlServer.SqlServerTransactionalDataSource> OnBeginTransactionAsync(System.Nullable<System.Data.IsolationLevel> isolationLevel, System.Boolean forwardEvents, System.Threading.CancellationToken cancellationToken );

		private partial Microsoft.Data.SqlClient.SqlConnection OnCreateConnection( );

		private partial System.Threading.Tasks.Task<Microsoft.Data.SqlClient.SqlConnection> OnCreateConnectionAsync(System.Threading.CancellationToken cancellationToken );

		private partial Tortuga.Chain.SqlServer.SqlServerOpenDataSource OnCreateOpenDataSource(Microsoft.Data.SqlClient.SqlConnection connection, Microsoft.Data.SqlClient.SqlTransaction? transaction );


		private void __RegisterTraits()
		{
			__TraitsRegistered = true;
			__Trait0.OnCreateConnectionAsync = OnCreateConnectionAsync;
			__Trait0.OnCreateConnection = OnCreateConnection;
			__Trait0.OnBeginTransactionAsync = OnBeginTransactionAsync;
			__Trait0.OnBeginTransaction = OnBeginTransaction;
			__Trait0.OnCreateOpenDataSource = OnCreateOpenDataSource;
		}
	}
}