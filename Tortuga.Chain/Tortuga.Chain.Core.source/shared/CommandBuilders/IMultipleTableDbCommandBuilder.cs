using System;
using System.Collections.Generic;

#if !DataTable_Missing
using System.Data;
#endif

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This command builder may return multiple record sets
    /// </summary>
    /// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new method will be added over time.</remarks>
    public interface IMultipleTableDbCommandBuilder : IMultipleRowDbCommandBuilder
    {
        /// <summary>
        /// To the collection set.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st collection.</typeparam>
        /// <typeparam name="T2">The type of the 2nd collection.</typeparam>
        /// <returns></returns>
        ILink<Tuple<List<T1>, List<T2>>> ToCollectionSet<T1, T2>()
            where T1 : class, new()
            where T2 : class, new();

        /// <summary>
        /// To the collection set.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st collection.</typeparam>
        /// <typeparam name="T2">The type of the 2nd collection.</typeparam>
        /// <typeparam name="T3">The type of the 3rd collection.</typeparam>
        /// <returns></returns>
        ILink<Tuple<List<T1>, List<T2>, List<T3>>> ToCollectionSet<T1, T2, T3>()
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new();

        /// <summary>
        /// To the collection set.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st collection.</typeparam>
        /// <typeparam name="T2">The type of the 2nd collection.</typeparam>
        /// <typeparam name="T3">The type of the 3rd collection.</typeparam>
        /// <typeparam name="T4">The type of the 4th collection.</typeparam>
        /// <returns></returns>
        ILink<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> ToCollectionSet<T1, T2, T3, T4>()
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new();

#if !DataTable_Missing
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        ILink<DataSet> ToDataSet(params string[] tableNames);
#endif

        /// <summary>
        /// To the collection set.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st collection.</typeparam>
        /// <typeparam name="T2">The type of the 2nd collection.</typeparam>
        /// <typeparam name="T3">The type of the 3rd collection.</typeparam>
        /// <typeparam name="T4">The type of the 4th collection.</typeparam>
        /// <typeparam name="T5">The type of the 5th collection.</typeparam>
        /// <returns></returns>
        ILink<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>> ToCollectionSet<T1, T2, T3, T4, T5>()
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
            where T5 : class, new();

        /// <summary>
        /// Indicates the results should be materialized as a set of tables.
        /// </summary>
        ILink<TableSet> ToTableSet(params string[] tableNames);
    }
}


