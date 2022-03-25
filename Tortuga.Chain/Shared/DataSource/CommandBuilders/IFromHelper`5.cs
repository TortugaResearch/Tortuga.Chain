using System.Data.Common;
using Tortuga.Chain;
using Tortuga.Chain.CommandBuilders;

namespace Traits
{
	interface IFromHelper<TCommand, TParameter, TObjectName, TDbType, TLimitOption> : ICommandHelper<TCommand, TParameter, TObjectName, TDbType>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObjectName : struct
		where TDbType : struct
		where TLimitOption : struct
	{
		TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> OnFromTableOrView<TObject>(TObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class;
		TableDbCommandBuilder<TCommand, TParameter, TLimitOption, TObject> OnFromTableOrView<TObject>(TObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class;

	}
}



