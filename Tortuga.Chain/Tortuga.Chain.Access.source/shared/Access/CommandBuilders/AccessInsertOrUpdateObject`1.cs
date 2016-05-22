//using System;
//using System.Data.OleDb;
//using System.Linq;
//using System.Text;
//using Tortuga.Chain.Core;
//using Tortuga.Chain.Materializers;


//namespace Tortuga.Chain.Access.CommandBuilders
//{
//    /// <summary>
//    /// Class AccessInsertOrUpdateObject
//    /// </summary>
//    internal sealed class AccessInsertOrUpdateObject<TArgument> : AccessObjectCommand<TArgument>
//        where TArgument : class
//    {
//        readonly UpsertOptions m_Options;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="AccessInsertOrUpdateObject{TArgument}"/> class.
//        /// </summary>
//        /// <param name="dataSource"></param>
//        /// <param name="tableName"></param>
//        /// <param name="argumentValue"></param>
//        /// <param name="options"></param>
//        public AccessInsertOrUpdateObject(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue, UpsertOptions options)
//            : base(dataSource, tableName, argumentValue)
//        {
//            m_Options = options;
//        }

//        public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
