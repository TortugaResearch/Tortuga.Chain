using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.Oracle;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Use for scalar functions.
    /// </summary>
    /// <seealso cref="ScalarDbCommandBuilder{OracleCommand, OracleParameter}" />
    internal class OracleScalarFunction : ScalarDbCommandBuilder<OracleCommand, OracleParameter>
    {
        readonly ScalarFunctionMetadata<OracleObjectName, OracleDbType> m_Function;
        readonly object m_FunctionArgumentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleTableFunction" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <param name="functionArgumentValue">The function argument.</param>
        public OracleScalarFunction(OracleDataSourceBase dataSource, OracleObjectName scalarFunctionName, object functionArgumentValue) : base(dataSource)
        {
            m_Function = dataSource.DatabaseMetadata.GetScalarFunction(scalarFunctionName);
            m_FunctionArgumentValue = functionArgumentValue;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>
        /// ExecutionToken&lt;TCommand&gt;.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override CommandExecutionToken<OracleCommand, OracleParameter> Prepare(Materializer<OracleCommand, OracleParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

            var sqlBuilder = m_Function.CreateSqlBuilder(StrictMode);
            sqlBuilder.ApplyRulesForSelect(DataSource);

            if (m_FunctionArgumentValue != null)
                sqlBuilder.ApplyArgumentValue(DataSource, OperationTypes.None, m_FunctionArgumentValue);

            var sql = new StringBuilder();
            sqlBuilder.BuildFromFunctionClause(sql, $"SELECT {m_Function.Name.ToQuotedString()} (", " )");
            sql.Append(";");

            List<OracleParameter> parameters;
            parameters = sqlBuilder.GetParameters();

            return new OracleCommandExecutionToken(DataSource, "Query Function " + m_Function.Name, sql.ToString(), parameters);
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new OracleDataSourceBase DataSource
        {
            get { return (OracleDataSourceBase)base.DataSource; }
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>ColumnMetadata.</returns>
        /// <remarks>Always returns null since this command builder has no columns</remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return null;
        }


    }
}
