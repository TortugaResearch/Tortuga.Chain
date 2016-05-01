using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// Materializes the result set as a List of dynamic objects.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DynamicCollectionMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, List<dynamic>> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCollectionMaterializer{TCommand, TParameter}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        public DynamicCollectionMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder)
            : base(commandBuilder)
        { }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override List<dynamic> Execute(object state = null)
        {
            var result = new List<dynamic>();
            Prepare().Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        IDictionary<string, object> item = new ExpandoObject();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i))
                                item[reader.GetName(i)] = null;
                            else
                                item[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(item);

                    }
                    return result.Count;
                }
            }, state);

            return result;
        }



        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<List<dynamic>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = new List<dynamic>();

            await Prepare().ExecuteAsync(async cmd =>
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        IDictionary<string, object> item = new ExpandoObject();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i))
                                item[reader.GetName(i)] = null;
                            else
                                item[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(item);

                    }
                    return result.Count;
                }
            }, cancellationToken, state);

            return result;
        }

        /// <summary>
        /// Returns the list of columns the materializer would like to have.
        /// </summary>
        /// <returns>
        /// IReadOnlyList&lt;System.String&gt;.
        /// </returns>
        /// <remarks>
        /// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which columns to return. If NoColumns is returned, the command builder should omit the SELECT/OUTPUT clause.
        /// </remarks>
        public override IReadOnlyList<string> DesiredColumns()
        {
            return AllColumns;
        }
    }
}
