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
    /// Materializes the result set as a dynamic object.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    internal sealed class DynamicObjectMaterializer<TCommand, TParameter> : Materializer<TCommand, TParameter, dynamic> where TCommand : DbCommand
        where TParameter : DbParameter
    {
        readonly RowOptions m_RowOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectMaterializer{TCommand, TParameter}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        /// <param name="rowOptions">The row options.</param>
        public DynamicObjectMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder, RowOptions rowOptions)
            : base(commandBuilder)
        {
            m_RowOptions = rowOptions;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public override dynamic Execute(object state = null)
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

            if (result.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new MissingDataException("No rows were returned");
            }
            else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException("Expected 1 row but received " + result.Count + " rows");
            }

            return result[0];
        }



        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<dynamic> ExecuteAsync(CancellationToken cancellationToken, object state = null)
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

            if (result.Count == 0)
            {
                if (m_RowOptions.HasFlag(RowOptions.AllowEmptyResults))
                    return null;
                else
                    throw new MissingDataException("No rows were returned");
            }
            else if (result.Count > 1 && !m_RowOptions.HasFlag(RowOptions.DiscardExtraRows))
            {
                throw new UnexpectedDataException("Expected 1 row but received " + result.Count + " rows");
            }

            return result[0];
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
