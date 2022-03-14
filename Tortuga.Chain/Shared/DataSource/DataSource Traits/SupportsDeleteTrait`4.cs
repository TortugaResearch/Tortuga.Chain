using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits
{
	[Trait]
	class SupportsDeleteTrait<TCommand, TParameter, TObjectName, TDbType> : ISupportsDelete
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObjectName : struct
		where TDbType : struct
	{

		[Container(RegisterInterface = true)]
		internal IUpdateDeleteHelper<TCommand, TParameter, TObjectName, TDbType> DataSource { get; set; } = null!;

		IObjectDbCommandBuilder<TArgument> ISupportsDelete.Delete<TArgument>(string tableName, TArgument argumentValue, DeleteOptions options)
			=> DataSource.OnDeleteObject(DataSource.ParseObjectName(tableName), argumentValue, options);

		IObjectDbCommandBuilder<TArgument> ISupportsDelete.Delete<TArgument>(TArgument argumentValue, DeleteOptions options)
			=> DataSource.OnDeleteObject(DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>().Name, argumentValue, options);

		/// <summary>
		/// Creates a command to perform a delete operation.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="argumentValue"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		[Expose]
		public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Delete<TArgument>(TObjectName tableName, TArgument argumentValue, DeleteOptions options = DeleteOptions.None)
		where TArgument : class
		{
			var table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
			if (!DataSource.AuditRules.UseSoftDelete(table))
				return DataSource.OnDeleteObject(tableName, argumentValue, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

			return DataSource.OnUpdateObject(tableName, argumentValue, effectiveOptions);
		}

		/// <summary>
		/// Delete an object model from the table indicated by the class's Table attribute.
		/// </summary>
		/// <typeparam name="TArgument"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The delete options.</param>
		/// <returns></returns>
		[Expose]
		public ObjectDbCommandBuilder<TCommand, TParameter, TArgument> Delete<TArgument>(TArgument argumentValue, DeleteOptions options = DeleteOptions.None) where TArgument : class
		{
			var table = DataSource.DatabaseMetadata.GetTableOrViewFromClass<TArgument>();

			if (!DataSource.AuditRules.UseSoftDelete(table))
				return DataSource.OnDeleteObject(table.Name, argumentValue, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;
			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

			return DataSource.OnUpdateObject(table.Name, argumentValue, effectiveOptions);
		}
	}
}



