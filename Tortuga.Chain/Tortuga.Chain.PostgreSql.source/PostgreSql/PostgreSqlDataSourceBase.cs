using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlDataSourceBase.
    /// </summary>
    public abstract class PostgreSqlDataSourceBase : DataSource<NpgsqlCommand, NpgsqlParameter>, IClass1DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        public PostgreSqlDataSourceBase(DataSourceSettings settings) : base(settings)
        {
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        public abstract PostgreSqlMetadataCache DatabaseMetadata { get; }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }

        public IDbCommandBuilder Delete(string tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            throw new NotImplementedException();
        }

        public IMultipleRowDbCommandBuilder From(string tableOrViewName)
        {
            throw new NotImplementedException();
        }

        public IMultipleRowDbCommandBuilder From(string tableOrViewName, object filterValue)
        {
            throw new NotImplementedException();
        }

        public IMultipleRowDbCommandBuilder From(string tableOrViewName, string whereClause)
        {
            throw new NotImplementedException();
        }

        public IMultipleRowDbCommandBuilder From(string tableOrViewName, string whereClause, object argumentValue)
        {
            throw new NotImplementedException();
        }

        public ISingleRowDbCommandBuilder Insert(string tableName, object argumentValue, InsertOptions options = InsertOptions.None)
        {
            throw new NotImplementedException();
        }

        public IMultipleTableDbCommandBuilder Sql(string sqlStatement, object argumentValue)
        {
            throw new NotImplementedException();
        }

        public ISingleRowDbCommandBuilder Update(string tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            throw new NotImplementedException();
        }

        public ISingleRowDbCommandBuilder Upsert(string tableName, object argumentValue, UpsertOptions options = UpsertOptions.None)
        {
            throw new NotImplementedException();
        }

        IDbCommandBuilder IClass1DataSource.Delete(string tableName, object argumentValue, DeleteOptions options)
        {
            throw new NotImplementedException();
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            throw new NotImplementedException();
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            throw new NotImplementedException();
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            throw new NotImplementedException();
        }

        ITableDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            throw new NotImplementedException();
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, params T[] keys)
        {
            throw new NotImplementedException();
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, IEnumerable<T> keys)
        {
            throw new NotImplementedException();
        }

        ISingleRowDbCommandBuilder IClass1DataSource.GetByKey<T>(string tableName, T key)
        {
            throw new NotImplementedException();
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue, InsertOptions options)
        {
            throw new NotImplementedException();
        }

        IMultipleTableDbCommandBuilder IClass0DataSource.Sql(string sqlStatement, object argumentValue)
        {
            throw new NotImplementedException();
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Update(string tableName, object argumentValue, UpdateOptions options)
        {
            throw new NotImplementedException();
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Upsert(string tableName, object argumentValue, UpsertOptions options)
        {
            throw new NotImplementedException();
        }

        //TODO: implement ClassXMetadata

    }
}
