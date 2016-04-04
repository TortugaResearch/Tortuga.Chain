using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Helper functions for buildering SQL statements.
    /// </summary>
    public static class SqlBuilder
    {

        /// <summary>
        /// Gets parameters from an argument value.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="argumentValue">The argument value .</param>
        /// <returns></returns>
        public static List<TParameter> GetParameters<TParameter>(object argumentValue)
    where TParameter : DbParameter, new()
        {
            return GetParameters(argumentValue, () => new TParameter());
        }

        /// <summary>
        /// Gets parameters from an argument value.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="argumentValue">The argument value .</param>
        /// <param name="parameterBuilder">The parameter builder. This should set the parameter's database specific DbType property.</param>
        /// <returns></returns>
        public static List<TParameter> GetParameters<TParameter>(object argumentValue, Func<TParameter> parameterBuilder)
            where TParameter : DbParameter
        {
            var result = new List<TParameter>();

            if (argumentValue is IEnumerable<TParameter>)
                foreach (var param in (IEnumerable<TParameter>)argumentValue)
                    result.Add(param);
            else if (argumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)argumentValue)
                {
                    var param = parameterBuilder();
                    param.ParameterName = (item.Key.StartsWith("@", StringComparison.OrdinalIgnoreCase)) ? item.Key : "@" + item.Key;
                    param.Value = item.Value ?? DBNull.Value;
                    result.Add(param);
                }
            else if (argumentValue != null)
                foreach (var property in MetadataCache.GetMetadata(argumentValue.GetType()).Properties.Where(x => x.MappedColumnName != null))
                {
                    var param = parameterBuilder();
                    param.ParameterName = (property.MappedColumnName.StartsWith("@", StringComparison.OrdinalIgnoreCase)) ? property.MappedColumnName : "@" + property.MappedColumnName;
                    param.Value = property.InvokeGet(argumentValue) ?? DBNull.Value;
                    result.Add(param);
                }

            return result;
        }

    }


}
