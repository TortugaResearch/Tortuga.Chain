using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Appenders
{
    internal sealed class KeyJoinAppender<T1, T2, TKey> : Appender<Tuple<List<T1>, List<T2>>, List<T1>>
    {
        readonly Func<T2, TKey> m_ForeignKeyExpression;
        readonly JoinOptions m_JoinOptions;
        readonly Func<T1, TKey> m_PrimaryKeyExpression;
        readonly Func<T1, ICollection<T2>> m_TargetCollectionExpression;
        public KeyJoinAppender(ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, TKey> primaryKeyExpression, Func<T2, TKey> foreignKeyExpression, Func<T1, ICollection<T2>> targetCollectionExpression, JoinOptions joinOptions) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException(nameof(previousLink), $"{nameof(previousLink)} is null.");
            if (primaryKeyExpression == null)
                throw new ArgumentNullException(nameof(primaryKeyExpression), $"{nameof(primaryKeyExpression)} is null.");
            if (foreignKeyExpression == null)
                throw new ArgumentNullException(nameof(foreignKeyExpression), $"{nameof(foreignKeyExpression)} is null.");
            if (targetCollectionExpression == null)
                throw new ArgumentNullException(nameof(targetCollectionExpression), $"{nameof(targetCollectionExpression)} is null.");

            m_ForeignKeyExpression = foreignKeyExpression;
            m_PrimaryKeyExpression = primaryKeyExpression;
            m_TargetCollectionExpression = targetCollectionExpression;
            m_JoinOptions = joinOptions;
        }

        public KeyJoinAppender(ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, TKey> primaryKeyExpression, Func<T2, TKey> foreignKeyExpression, string targetCollectionName, JoinOptions joinOptions) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
            if (primaryKeyExpression == null)
                throw new ArgumentNullException("primaryKeyExpression", "primaryKeyExpression is null.");
            if (foreignKeyExpression == null)
                throw new ArgumentNullException("foreignKeyExpression", "foreignKeyExpression is null.");
            if (string.IsNullOrEmpty(targetCollectionName))
                throw new ArgumentException("targetCollectionName is null or empty.", "targetCollectionName");

            var targetPropertyStub = MetadataCache.GetMetadata(typeof(T1)).Properties[targetCollectionName]; //don't inline this variable.
            m_TargetCollectionExpression = (p) => (ICollection<T2>)targetPropertyStub.InvokeGet(p);


            m_ForeignKeyExpression = foreignKeyExpression;
            m_PrimaryKeyExpression = primaryKeyExpression;
            m_JoinOptions = joinOptions;
        }

        public KeyJoinAppender(ILink<Tuple<List<T1>, List<T2>>> previousLink, string primaryKeyName, string foreignKeyName, string targetCollectionName, JoinOptions joinOptions) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName is null or empty.", "primaryKeyName");
            if (string.IsNullOrEmpty(foreignKeyName))
                throw new ArgumentException("foreignKeyName is null or empty.", "foreignKeyName");
            if (string.IsNullOrEmpty(targetCollectionName))
                throw new ArgumentException("targetCollectionName is null or empty.", "targetCollectionName");


            var primaryKeyStub = MetadataCache.GetMetadata(typeof(T1)).Properties[primaryKeyName]; //don't inline this variable.
            m_PrimaryKeyExpression = (p) => (TKey)primaryKeyStub.InvokeGet(p);

            var foreignKeyStub = MetadataCache.GetMetadata(typeof(T1)).Properties[foreignKeyName]; //don't inline this variable.
            m_ForeignKeyExpression = (p) => (TKey)foreignKeyStub.InvokeGet(p);

            var targetPropertyStub = MetadataCache.GetMetadata(typeof(T1)).Properties[targetCollectionName]; //don't inline this variable.
            m_TargetCollectionExpression = (p) => (ICollection<T2>)targetPropertyStub.InvokeGet(p);

            m_JoinOptions = joinOptions;
        }


        public override List<T1> Execute(object state = null)
        {
            var result = PreviousLink.Execute(state);
            Match(result);
            return result.Item1;
        }

        public override async Task<List<T1>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = await PreviousLink.ExecuteAsync(state);
            Match(result);
            return result.Item1;
        }

#if Parallel_Missing
        void Match(Tuple<List<T1>, List<T2>> result)
        {
            var ignoreUnmatchedChildren = m_JoinOptions.HasFlag(JoinOptions.IgnoreUnmatchedChildren);
            var multipleParents = m_JoinOptions.HasFlag(JoinOptions.MultipleParents);

            if (multipleParents)
                MultiMatchSerial(result, ignoreUnmatchedChildren);
            else
                MatchSerial(result, ignoreUnmatchedChildren);
        }
#else
        void Match(Tuple<List<T1>, List<T2>> result)
        {
            var ignoreUnmatchedChildren = m_JoinOptions.HasFlag(JoinOptions.IgnoreUnmatchedChildren);
            var multipleParents = m_JoinOptions.HasFlag(JoinOptions.MultipleParents);

            var parallel = m_JoinOptions.HasFlag(JoinOptions.Parallel);

            if (multipleParents)
            {
                if (parallel)
                    MultiMatchParallel(result, ignoreUnmatchedChildren);
                else
                    MultiMatchSerial(result, ignoreUnmatchedChildren);
            }
            else
            {
                if (parallel)
                    MatchParallel(result, ignoreUnmatchedChildren);
                else
                    MatchSerial(result, ignoreUnmatchedChildren);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChildObject")]
        void MatchParallel(Tuple<List<T1>, List<T2>> result, bool ignoreUnmatchedChildren)
        {
            //build the dictionary
            var parents = result.Item1.AsParallel().ToDictionary(m_PrimaryKeyExpression, m_TargetCollectionExpression);

            Parallel.ForEach(result.Item2, child =>
                {
                    var fk = m_ForeignKeyExpression(child);
                    if (parents.TryGetValue(fk, out var targetCollection))
                    {
                        lock (targetCollection)
                            targetCollection.Add(child);
                    }
                    else if (!ignoreUnmatchedChildren)
                    {
                        var ex = new UnexpectedDataException($"Found child object with the foreign key \"{ fk }\" that couldn't be matched to a parent. See Exception.Data[\"ChildObject\"] for details.");
                        ex.Data["ForeignKey"] = fk;
                        ex.Data["ChildObject"] = child;
                        throw ex;
                    }
                });
        }
#endif

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChildObject")]
        void MatchSerial(Tuple<List<T1>, List<T2>> result, bool ignoreUnmatchedChildren)
        {
            //build the dictionary
            var parents = result.Item1.ToDictionary(m_PrimaryKeyExpression, m_TargetCollectionExpression);

            foreach (var child in result.Item2)
            {
                var fk = m_ForeignKeyExpression(child);
                if (parents.TryGetValue(fk, out var targetCollection))
                {
                    targetCollection.Add(child);
                }
                else if (!ignoreUnmatchedChildren)
                {
                    var ex = new UnexpectedDataException($"Found child object with the foreign key \"{ fk }\" that couldn't be matched to a parent. See Exception.Data[\"ChildObject\"] for details.");
                    ex.Data["ForeignKey"] = fk;
                    ex.Data["ChildObject"] = child;
                    throw ex;
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChildObject")]
        void MultiMatchSerial(Tuple<List<T1>, List<T2>> result, bool ignoreUnmatchedChildren)
        {
            //build the dictionary
            var parents1 = from p in result.Item1 group m_TargetCollectionExpression(p) by m_PrimaryKeyExpression(p) into g select g;
            var parents2 = parents1.ToDictionary(p => p.Key);

            foreach (var child in result.Item2)
            {
                var fk = m_ForeignKeyExpression(child);
                if (parents2.TryGetValue(fk, out var targetCollections))
                {
                    foreach (var targetCollection in targetCollections)
                        targetCollection.Add(child);
                }
                else if (!ignoreUnmatchedChildren)
                {
                    var ex = new UnexpectedDataException($"Found child object with the foreign key \"{ fk }\" that couldn't be matched to a parent. See Exception.Data[\"ChildObject\"] for details.");
                    ex.Data["ForeignKey"] = fk;
                    ex.Data["ChildObject"] = child;
                    throw ex;
                }
            }
        }

#if !Parallel_Missing
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChildObject")]
        void MultiMatchParallel(Tuple<List<T1>, List<T2>> result, bool ignoreUnmatchedChildren)
        {
            //build the dictionary
            var parents1 = from p in result.Item1.AsParallel() group m_TargetCollectionExpression(p) by m_PrimaryKeyExpression(p) into g select g;
            var parents2 = parents1.ToDictionary(p => p.Key);

            Parallel.ForEach(result.Item2, child =>
                {
                    var fk = m_ForeignKeyExpression(child);
                    if (parents2.TryGetValue(fk, out var targetCollections))
                    {
                        foreach (var targetCollection in targetCollections)
                            lock (targetCollection)
                                targetCollection.Add(child);
                    }
                    else
    if (!ignoreUnmatchedChildren)
                    {
                        var ex = new UnexpectedDataException($"Found child object with the foreign key \"{ fk }\" that couldn't be matched to a parent. See Exception.Data[\"ChildObject\"] for details.");
                        ex.Data["ForeignKey"] = fk;
                        ex.Data["ChildObject"] = child;
                        throw ex;
                    }
                });
        }
#endif

    }
}
